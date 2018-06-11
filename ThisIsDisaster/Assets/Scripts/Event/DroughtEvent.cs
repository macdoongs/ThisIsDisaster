using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroughtEvent : EventBase
{
    GameObject dryObject = null;         // 맵 건조
	GameObject crackObject = null;      // 공통1 (바닥 갈라짐)

    const float _BLUR_AMOUNT_MAX = 10f;
    const float _BLUR_GLOW_MAX = 0.05f;

    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 4f;
    public float damageTime = 1f;

    Timer _timer = new Timer();

    Timer _effectTimer = new Timer();
    const float _EFFECT_TIME = 20f;
    BlurFilter EffectFilter {
        get {
            if (_effectFilter == null) {
                _effectFilter = Camera.main.gameObject.GetComponent<BlurFilter>();
                if (_effectFilter == null)
                {
                    _effectFilter = Camera.main.gameObject.AddComponent<BlurFilter>();
                }
            }
            return _effectFilter;
        }
    }
    BlurFilter _effectFilter = null;
    

    float _lifetime = 5f;

	public DroughtEvent()
	{
		type = WeatherType.Drought;
	}

	public override void OnGenerated()
	{
        dryObject = EventManager.Manager.MakeWorldDry();
        //crackObject = EventManager.Manager.MakeWorldCrack();

        EffectFilter.enabled = false;
	}

	public override void OnStart()
	{
		dryObject.SetActive(true);
		crackObject.SetActive(true);

        _timer.StartTimer(_lifetime);
        _damageTimer.StartTimer(damageTime);
        _effectTimer.StartTimer(_EFFECT_TIME);
        EffectFilter.enabled = true;
        EffectFilter.Amount = 0f;
        EffectFilter.Glow = 0f;
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

        if (_effectTimer.started)
        {
            float rate = _effectTimer.Rate;
            if (_effectTimer.RunTimer())
            {
                rate = 1f;
            }

            float glow = Mathf.Lerp(0f, _BLUR_GLOW_MAX, rate);
            float amount = Mathf.Lerp(0f, _BLUR_AMOUNT_MAX, rate);
            EffectFilter.Glow = glow;
            EffectFilter.Amount = amount;
        }

        if (_damageTimer.started)
        {
            //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1f;
                float staminaDamageRate = 3f;

                if (CharacterModel.Instance.GetPlayerModel().IsInShelter())
                {
                    OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate * 0.5f);
                    CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate * 0.5f);
                }
                else
                {
                    OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                    CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);
                }
                
                _damageTimer.StartTimer(damageTime);
            }

        }
    }

    public override void OnEnd()
	{
        dryObject.SetActive(false);
        
        EffectFilter.enabled = false;
	}

	public override void OnDestroy()
	{

	}
}  // 가뭄 이벤트

