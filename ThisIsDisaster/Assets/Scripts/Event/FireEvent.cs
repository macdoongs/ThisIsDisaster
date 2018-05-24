using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireEvent : EventBase
{
	GameObject fireObject = null;

	public FireEvent()
	{
		type = WeatherType.Fire;
	}

	public override void OnGenerated()
	{
		fireObject = null;
	}
	public override void OnStart()
	{
		fireObject = null;
	}

	public override void OnEnd()
	{
		fireObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 화재 이벤트

