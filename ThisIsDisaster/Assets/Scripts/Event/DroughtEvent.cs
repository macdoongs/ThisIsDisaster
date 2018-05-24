using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroughtEvent : EventBase
{
	GameObject dryObject = null;         // 맵 건조
	GameObject crackObject = null;      // 공통1 (바닥 갈라짐)

	public DroughtEvent()
	{
		type = WeatherType.Drought;
	}

	public override void OnGenerated()
	{
		dryObject = null;
		crackObject = null;
	}

	public override void OnStart()
	{
		dryObject = null;
		crackObject = null;
	}

	public override void OnEnd()
	{
		dryObject = null;
		crackObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 가뭄 이벤트

