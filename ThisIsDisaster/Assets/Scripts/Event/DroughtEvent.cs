using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroughtEvent : EventBase
{
    GameObject dryObject = null;         // 맵 건조
	GameObject crackObject = null;      // 공통1 (바닥 갈라짐)


    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 4f;
    public float damageTime = 1f;

    Timer _timer = new Timer();
    float _lifetime = 5f;

	public DroughtEvent()
	{
		type = WeatherType.Drought;
	}

	public override void OnGenerated()
	{
        dryObject = EventManager.Manager.MakeWorldDry();
        crackObject = EventManager.Manager.MakeWorldCrack();
	}

	public override void OnStart()
	{
		dryObject.SetActive(true);
		crackObject.SetActive(true);

        _timer.StartTimer(_lifetime);
        _damageTimer.StartTimer(damageTime);
    }

    public override void OnExecute()
    {
        if (_timer.started) {
            if (_timer.RunTimer()) {
                //timer expired
                //Debug.Log("End Drought Event");
                //EventManager.Manager.EndEvent(this.type);
            }
        }

        if (_damageTimer.started)
        {
            //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1f;
                float staminaDamageRate = 3f;

                CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);

                _damageTimer.StartTimer(damageTime);
            }

        }
    }

    public override void OnEnd()
	{
		dryObject = null;
		crackObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 가뭄 이벤트

