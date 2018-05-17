using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        {
            this.started = true;
        }
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
#if UNITY_EDITOR
            Debug.Log(e);
#endif
            return 1f;
        }
    }
}