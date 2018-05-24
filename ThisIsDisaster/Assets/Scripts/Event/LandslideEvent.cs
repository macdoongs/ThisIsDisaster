using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandslideEvent : EventBase
{
	GameObject landslideObject = null;
	GameObject quackObject = null;    // 공통2  (맵 흔들림)
	GameObject collapseObject = null;      // 맵 이벤트
	LandslideEffect _effect = null;

	public LandslideEvent()
	{
		type = WeatherType.Landslide;
	}

	public override void OnGenerated(){
		_effect = EventManager.Manager.GetLandslideEffect();

		quackObject = null;
		collapseObject = null;
	}

	public override void OnStart()
	{
		_effect.SetActive (true);

		quackObject = null;
		collapseObject = null;
	}

	public override void OnEnd()
	{
		_effect.SetActive (false);
		quackObject = null;
		collapseObject = null;
	}

	public override void OnDestroy()
	{

	}
}  // 산사태 이벤트

