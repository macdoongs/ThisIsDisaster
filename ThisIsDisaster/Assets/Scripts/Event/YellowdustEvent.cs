using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YellowdustEvent : EventBase
{
	SandEffect _effect = null;     // 모래입자
	GameObject YellowObject = null;          // 노란 화면
	public YellowdustEvent()
	{
		type = WeatherType.Yellowdust;
	}

	public override void OnGenerated()
	{
		_effect = EventManager.Manager.GetSandEffect();
		YellowObject = null;
	}

	public override void OnStart()
	{
		_effect.SetActive(true);

		YellowObject = null;
	}

	public override void OnEnd()
	{
		_effect.SetActive(false);
		YellowObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 황사 이벤트
