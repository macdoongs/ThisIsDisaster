using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeavysnowEvent : EventBase
{
	int max = -1;//max level of this event
    Timer _lifeTimeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;
    SnowEffect snowEffect = null;
	CloudEffect cloudEffect = null;


    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 1f;
    public float damageTime = 1f;

    public HeavysnowEvent()
	{
		type = WeatherType.Heavysnow;
	}

	public override void OnGenerated()
	{
		snowEffect = EventManager.Manager.GetSnowEffect();

		cloudEffect = EventManager.Manager.GetCloudyEffect();
	}

	public override void OnStart()
	{
		snowEffect.SetActive(true);
		snowEffect.SetLevel(5);
		EventManager.Manager.SetWorldFilterColor(new Color(71f/255f, 73f/255f, 73f/255f, 98f/255f));

		Debug.Log(RandomMapGenerator.Instance.GetRandomTileByHeight (1));

        _lifeTimeTimer.StartTimer(lifeTime);
        _damageTimer.StartTimer(damageTime);
        //cloudEffect.SetActive(true);

    }

	public override void OnEnd()
	{
		snowEffect.SetActive(false);
		cloudEffect.SetActive(false);
	}

	public override void OnDestroy()
	{

	}


    public override void OnExecute()
    {
        if (_lifeTimeTimer.started)
        {
            if(_lifeTimeTimer.RunTimer());
            //이벤트 끝내는 함수 필요
        }
        if (_damageTimer.started)
        {
            //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1f;
                float staminaDamageRate = 1f;
                float speedDownRate = 0.3f;

                var player = CharacterModel.Instance;

                if (player.GetPlayerModel().IsInShelter())
                {
                    healthDamageRate *= 0.5f;
                    staminaDamageRate *= 0.5f;
                    speedDownRate = 0f;
                }

                //CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);
                CharacterModel.Instance.SetSpeedFactor(1f - speedDownRate);

                _damageTimer.StartTimer(damageTime);
            }
        }
    }
}  // 폭설 이벤트
	

