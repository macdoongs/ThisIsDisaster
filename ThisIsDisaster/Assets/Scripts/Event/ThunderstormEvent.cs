using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderstormEvent : EventBase
{
	GameObject rainObject = null;
	GameObject darkObject = null;
	GameObject blinkObject = null;      //  깜빡이
	GameObject lightningObject = null;     // 낙뢰 효과
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
		rainObject.SetActive(true);
		darkObject.SetActive(true);

		blinkObject = null;
		lightningObject = null;
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
