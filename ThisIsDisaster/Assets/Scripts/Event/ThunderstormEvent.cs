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
    Timer _blinkTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;

	public ThunderstormEvent()
	{
		type = WeatherType.Thunderstorm;
	}

	public override void OnGenerated()
	{
		rainObject = EventManager.Manager.MakeWorldRain();
		darkObject = EventManager.Manager.MakeWorldDark();
        blinkObject = EventManager.Manager.MakeWorldBlink();
        
		lightningObject = null;
	}

	public override void OnStart()
	{
		//rainObject.SetActive(true);
		darkObject.SetActive(true);
        rainObject.SetActive(true);

		blinkObject = null;
		lightningObject = null;

        _lifeTimeTimer.StartTimer(lifeTime);
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

                return;
            }
            if (_lifeTimeTimer.elapsed > 10 && _lifeTimeTimer.elapsed < 20)
            {
                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a > 0)
                        color.a -= 0.002f;
                    renderer.color = color;
                }
                var rainParticle = rainObject.GetComponent<ParticleSystem>();
                if (rainParticle != null)
                {
                        rainParticle.maxParticles -= 50;
                }

                return;
            }
        }
    }

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
