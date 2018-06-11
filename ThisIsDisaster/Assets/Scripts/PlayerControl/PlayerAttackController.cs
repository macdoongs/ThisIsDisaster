using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {
    public AttackSender Sender;
    public float NextAttackDelay = 1f;
    Timer _attackDelayTimer = new Timer();

    public bool isDebuggEnabled = false;
    public SpriteRenderer _DebugRenderer;
    public PlayerMoveController MoveController;
    public AnimatorEventHandler animatorEventHandler;

	// Use this for initialization
	void Start () {
        animatorEventHandler.SetEventCalled(Sender.OnAttack);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_attackDelayTimer.started)
        {
            if (!_attackDelayTimer.RunTimer())
            {
                return;
            }
        }

        //if (IsAttackCommandInput()) {
        //    if (IsAttackable()) {
        //        Sender.OnAttack();
        //        if (MoveController) {
        //            AnimatorUtil.SetInteger(MoveController.PlayerMovementCTRL, "AttackType", 
        //                (Sender.Owner as PlayerModel).GetAttackAnimType());
        //            AnimatorUtil.SetTrigger(MoveController.PlayerMovementCTRL, "Attack");
        //        }
        //        _attackDelayTimer.StartTimer(NextAttackDelay);
        //    }
        //}

        if (isDebuggEnabled && _DebugRenderer) {
            if (Sender.IsAttacking())
            {
                if (!_DebugRenderer.enabled) {
                    _DebugRenderer.enabled = true;
                }
            }
            else {
                if (_DebugRenderer.enabled) {
                    _DebugRenderer.enabled = false;
                }
            }
        }
	}

    public void OnAttackClicked() {
        if (IsAttackable())
        {
            CharacterModel.Instance.SubtractStamina(5f);
            //Sender.OnAttack();
            if (MoveController)
            {
                AnimatorUtil.SetInteger(MoveController.PlayerMovementCTRL, "AttackType",
                    (Sender.Owner as PlayerModel).GetAttackAnimType());
                AnimatorUtil.SetTrigger(MoveController.PlayerMovementCTRL, "Attack");

                if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi) {
                    NetworkComponents.GameServer.Instance.SendPlayerAnimTrigger("Attack");
                }
            }

            SoundLayer.CurrentLayer.PlaySound("se_attack_common");
            _attackDelayTimer.StartTimer(NextAttackDelay);
        }
    }

        public bool IsAttackCommandInput() {
        //일단은 좌클릭으로 입력체크
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
    
    public bool IsAttackable() {
        if (CharacterModel.Instance.IsDead()) return false;
        return !_attackDelayTimer.started && CharacterModel.Instance.CurrentStats.Stamina > 5f;
    }

    public void SetAttackRange(float x, float y)
    {
        float xValue = x / 5f;
        float yValue = y / 5f;
        var col = Sender.GetComponent<BoxCollider2D>();
        col.size = new Vector2(xValue, yValue);
    }
}
