﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {
    public AttackSender Sender;
    public float NextAttackDelay = 1f;
    Timer _attackDelayTimer = new Timer();

    public bool isDebuggEnabled = false;
    public SpriteRenderer _DebugRenderer;

	// Use this for initialization
	void Start () {
		
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

        if (IsAttackCommandInput()) {
            if (IsAttackable()) {
                Sender.OnAttack();
                _attackDelayTimer.StartTimer(NextAttackDelay);
            }
        }

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

    public bool IsAttackCommandInput() {
        //일단은 좌클릭으로 입력체크
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
        return false;
    }
    
    public bool IsAttackable() {
        return !_attackDelayTimer.started;
    }
}