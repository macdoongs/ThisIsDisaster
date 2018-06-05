using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    이벤트 발생 시 비오게
    시간 약간 지나면 화면 어두워지게
    20초 지나면 낙뢰 멈추고
    이벤트 끝나면 비 점점 안오게
*/
public class ThunderstormEvent : EventBase
{
	GameObject rainObject = null;
	GameObject darkObject = null;
	GameObject blinkObject = null;      //  깜빡이
	GameObject lightningObject = null;     // 낙뢰 효과

    Timer _lifeTimeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;

    Timer _blinkTimer = new Timer();
    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 3f;
    public float damageTime = 1f;

    Timer _thunderTimer = new Timer();
    const float _THUNDER_FREQ_MIN = 5f;
    const float _THUNDER_FREQ_MAX = 10f;
    


	public ThunderstormEvent()
	{
		type = WeatherType.Thunderstorm;
	}

	public override void OnGenerated()
	{
		rainObject = EventManager.Manager.MakeWorldRain();
		darkObject = EventManager.Manager.MakeWorldDark();
        blinkObject = EventManager.Manager.MakeWorldBlink();
        lightningObject = EventManager.Manager.MakeWorldLightning();
        
	}

	public override void OnStart()
	{
		//rainObject.SetActive(true);
		darkObject.SetActive(true);
        rainObject.SetActive(true);

		blinkObject = null;
		lightningObject = null;

        _lifeTimeTimer.StartTimer(lifeTime);
        _damageTimer.StartTimer(damageTime);
        _thunderTimer.StartTimer(UnityEngine.Random.Range(_THUNDER_FREQ_MIN, _THUNDER_FREQ_MAX));
        //StartDamageByEvent(eventDamage, damageTime);
    }


    public override void OnExecute()
    {
        if (_lifeTimeTimer.started)
        {
            _lifeTimeTimer.RunTimer();
            

            if (_lifeTimeTimer.elapsed < 5)
            {
                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a < 0.5f)
                        color.a += 0.01f;
                    renderer.color = color;
                }
            }
            if (_lifeTimeTimer.elapsed > 10 && _lifeTimeTimer.elapsed < 20)
            {
                var rainParticle = rainObject.GetComponent<ParticleSystem>();
                if (rainParticle != null)
                {
                        rainParticle.maxParticles -= 5;
                }
            }
            if (_lifeTimeTimer.elapsed > _lifeTimeTimer.maxTime - 10f)
            {

                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a > 0)
                        color.a -= 0.002f;
                    renderer.color = color;
                }
            }


            if (_thunderTimer.RunTimer())
            {
                StartLightning();
                _thunderTimer.StartTimer(UnityEngine.Random.Range(_THUNDER_FREQ_MIN, _THUNDER_FREQ_MAX));
            }

        }

        if(_damageTimer.started)
        {

                //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
                if (_damageTimer.RunTimer())
                {
                    float healthDamageRate = 1f;

                    CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);

                    _damageTimer.StartTimer(damageTime);
                }
                
        }
    }

    private void StartLightning()
    {
        lightningObject.SetActive(true);
    }

    /*public void StartDamageByEvent(float damage, float time)
    {
        eventDamage = damage;
        damageTime = time;
        _damageTimer.StartTimer(time);
    }*/

    public override void OnEnd()
	{
		rainObject.SetActive(false);
		darkObject.SetActive(false);

		blinkObject = null;
		lightningObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 낙뢰 이벤트
