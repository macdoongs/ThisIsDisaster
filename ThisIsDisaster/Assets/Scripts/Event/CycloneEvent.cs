using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CycloneEvent : EventBase {
	CycloneEffect cycloneObject = null;
	GameObject darkObject = null;

	public CycloneEvent() {
		type = WeatherType.Cyclone;
	}

	public override void OnGenerated()
	{
		cycloneObject = EventManager.Manager.GetCycloneEffect();
		darkObject = EventManager.Manager.MakeWorldDark();


	}

	public override void OnStart()
	{
		cycloneObject.SetActive(true);
		darkObject.SetActive(true);
	}

	public override void OnEnd()
	{
		cycloneObject.SetActive(false);
		darkObject.SetActive(false);
	}

	public override void OnDestroy()
	{

	}
}      // 태풍 이벤트
	
