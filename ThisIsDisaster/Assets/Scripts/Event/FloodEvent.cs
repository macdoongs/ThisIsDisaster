using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloodEvent : EventBase {
	GameObject rainObject = null;
	GameObject darkObject = null;
	GameObject tsunamiObject = null;
	GameObject fillObject = null;    // 맵에 물차오르는거
	FloodEffect _effect = null;


	public FloodEvent() {
		type = WeatherType.Flood;
	}

	public override void OnGenerated()
	{
		_effect = EventManager.Manager.GetFloodEffect();

		rainObject = EventManager.Manager.MakeWorldRain();
		darkObject = EventManager.Manager.MakeWorldDark();
	}

	public override void OnStart()
	{
		_effect.SetActive (true);

		rainObject.SetActive(true);
		darkObject.SetActive(true);
	}

	public override void OnEnd()
	{
		_effect.SetActive (false);

		rainObject.SetActive(false);
		darkObject.SetActive(false);

		rainObject = null;
		darkObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 해일 이벤트

