using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YellowdustEvent : EventBase
{
	SandEffect _effect = null;     // 모래입자
	GameObject YellowObject = null;          // 노란 화면

    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1.5f;
    public float damageEnergyPerSec = 2f;
    public float damageTime = 1f;

    public YellowdustEvent()
	{
		type = WeatherType.Yellowdust;
	}

	public override void OnGenerated()
	{
		_effect = EventManager.Manager.GetSandEffect();
		YellowObject = null;
	}

	public override void OnStart()
	{
		_effect.SetActive(true);
        _damageTimer.StartTimer(damageTime);

		YellowObject = null;
	}

	public override void OnEnd()
	{
		_effect.SetActive(false);
		YellowObject = null;
	}

    public override void OnExecute()
    {
        if (_damageTimer.started)
        {
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1.5f;
                float staminaDamageRate = 2f;
                var player = CharacterModel.Instance;

                if (player.toolSlot.metaInfo.metaId.Equals(31005))
                {
                    healthDamageRate -= 1f;
                }

                if (player.GetPlayerModel().IsInShelter())
                {
                    healthDamageRate *= 0.5f;
                    staminaDamageRate *= 0.5f;
                }

                //CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);

                _damageTimer.StartTimer(damageTime);
            }

        }
    }

    public override void OnDestroy()
	{

	}
}  // 황사 이벤트
