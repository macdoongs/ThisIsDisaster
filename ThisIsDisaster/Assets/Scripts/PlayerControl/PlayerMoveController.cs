using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorUtil {
    public static bool HasParameter(Animator animator, string paramName) {
        foreach (var v in animator.parameters) {
            if (v.name == paramName) {
                return true;
            }
        }
        return false;
    }

    public static void SetInteger(Animator animator, string paramName, int value) {
        if (HasParameter(animator, paramName))
        {
            animator.SetInteger(paramName, value);
        }
        else {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }

    public static void SetBool(Animator animator, string paramName, bool value)
    {
        if (HasParameter(animator, paramName))
        {
            animator.SetBool(paramName, value);
        }
        else
        {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }

    public static void SetTrigger(Animator animator, string paramName)
    {
        if (HasParameter(animator, paramName))
        {
            animator.SetTrigger(paramName);
        }
        else
        {
#if UNITY_EDITOR
            string log = string.Format("Animator of [{0}] Doesn't has Parameter [{1}]", animator.gameObject.name, paramName);
            Debug.LogError(log);
#endif
        }
    }
}

[System.Serializable]
public class Timer
{
    public delegate void OnTimerRunningEnd();

    public float elapsed = 0f;
    public float maxTime = 0f;
    public bool started = false;
    public bool autoStop = true;
    public float Rate
    {
        get
        {
            if (maxTime == 0f) return 0f;
            return elapsed / maxTime;
        }
    }

    public OnTimerRunningEnd endCmd = null;

    public virtual void StartTimer(float time)
    {
        this.maxTime = time;

        if (this.started)
        {
            this.elapsed = 0f;
        }
        else
            this.started = true;
    }

    public virtual void SetEndCmd(OnTimerRunningEnd cmd)
    {
        this.endCmd = cmd;
    }

    public virtual bool RunTimer()
    {
        if (!this.started)
        {
            return false;
        }
        elapsed += Time.deltaTime;
        if (elapsed >= maxTime)
        {
            if (autoStop)
            {
                elapsed = 0f;
                started = false;

                if (endCmd != null)
                {
                    endCmd();
                    endCmd = null;
                }
            }
            return true;
        }
        return false;
    }

    public virtual void StartTimer()
    {
        this.maxTime = float.MaxValue;

        if (this.started)
        {
            this.elapsed = 0f;
        }
        else
            this.started = true;
    }

    public virtual void StopTimer()
    {
        this.started = false;
        this.elapsed = 0f;
        this.maxTime = 0f;
    }

    public override string ToString()
    {
        return base.ToString() + "::[State]" + started + " [Max Time]" + maxTime + " [Elapsed]" + elapsed;
    }

    public virtual float GetRate()
    {
        try
        {
            return elapsed / maxTime;
        }
        catch (System.ArithmeticException e)
        {
            return 1f;
        }
    }
}

public class PlayerMoveController : MonoBehaviour {
    public float moveSpeed = 1f;
    public float jumpDelay = 1f;
    public Animator PlayerMovementCTRL;
    public Transform FlipPivot;

    float CurrentPivotXScale { get { return FlipPivot.transform.localScale.x; } }

    Timer _jumpDelayTimer = new Timer();

    void Update() {
        Vector3 currentPos = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            MoveUp(ref currentPos);
        }

        if (Input.GetKey(KeyCode.S))
        {
            MoveDown(ref currentPos);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MoveLeft(ref currentPos);
        }

        if (Input.GetKey(KeyCode.D))
        {
            MoveRight(ref currentPos);
        }

        //비효율적, 후에 바꿀것
        Vector3 changed = transform.position;
        Vector2 dist = changed - currentPos;
        if (dist != Vector2.zero)
        {
            AnimatorUtil.SetBool(PlayerMovementCTRL, "Move", true);
        }
        else
        {
            AnimatorUtil.SetBool(PlayerMovementCTRL, "Move", false);
        }

        if (dist.x > 0f)
        {
            if (CurrentPivotXScale < 0f) {
                Flip();
            }
        }
        else if(dist.x < 0f) {
            if (CurrentPivotXScale > 0f) {
                Flip();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        if (_jumpDelayTimer.started) {
            _jumpDelayTimer.RunTimer();
        }
    }

    void Flip() {
        var scale = FlipPivot.transform.localScale;
        scale.x *= -1f;
        FlipPivot.transform.localScale = scale;
    }

    /// <summary>
    /// Player Jump
    /// </summary>
    /// <param name="input">사용자 입력에 의한 점프인가</param>
    void Jump(bool input = true) {
        //position update needed?

        if (input && _jumpDelayTimer.started) return;

        AnimatorUtil.SetTrigger(PlayerMovementCTRL, "Jump");

        if (input) {
            _jumpDelayTimer.StartTimer(jumpDelay);
        }
    }

    //deltaTime : 프레임에 렉이 걸린만큼 값이 커져 프레임렉을 보정
    void MoveUp(ref Vector3 pos)
    {
        transform.Translate(0,moveSpeed * Time.deltaTime, 0);
    }

    void MoveDown(ref Vector3 pos)
    {

        transform.Translate(0, -moveSpeed * Time.deltaTime, 0);
    }

    void MoveLeft(ref Vector3 pos)
    {

        transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
    }

    void MoveRight(ref Vector3 pos)
    {

        transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
    }
}