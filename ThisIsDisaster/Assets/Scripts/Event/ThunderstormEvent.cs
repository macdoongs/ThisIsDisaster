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

	public ThunderstormEvent()
	{
		type = WeatherType.Thunderstorm;
	}

	public override void OnGenerated()
	{
		rainObject = EventManager.Manager.MakeWorldThunderstorm();
		darkObject = EventManager.Manager.MakeWorldDark();

		blinkObject = null;
		lightningObject = null;
	}

	public override void OnStart()
	{
		//rainObject.SetActive(true);
		darkObject.SetActive(true);

		blinkObject = null;
		lightningObject = null;

        _lifeTimeTimer.StartTimer(lifeTime);
    }


    public override void OnExecute()
    {
        if (_lifeTimeTimer.started)
        {
            if (_lifeTimeTimer.elapsed < 5)
            {
                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    Debug.Log(color);
                    if (color.a < 0.5f)
                        color.a += 0.01f;
                    renderer.color = color;
                }

                return;
            }

            if (_lifeTimeTimer.RunTimer())
            {
                

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
