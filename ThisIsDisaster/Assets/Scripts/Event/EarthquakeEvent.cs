using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EarthquakeEvent : EventBase
{
	GameObject quakeObject = null;          // 공통2  (맵흔들림)
	GameObject crackObject = null;          // 공통1 (갈라짐)

	EarthquakeEffect _effect = null;

	public EarthquakeEvent()
	{
		type = WeatherType.Earthquake;
	}

	public override void OnGenerated()
	{
		quakeObject = null;
		crackObject = null;

		_effect = EventManager.Manager.GetEarthquakeEffect();
	}

	public override void OnStart()
	{
		quakeObject = null;
		crackObject = null;

		_effect.SetActive(true);
		_effect.SetEarthquakeType(EarthquakeEffect.EarthquakeType.Main, 3);
		_effect.StartWave();
	}

	public override void OnEnd()
	{
		quakeObject = null;
		crackObject = null;
		_effect.ReturnTiles();
		_effect.SetActive(false);
	}

	public override void OnDestroy()
	{

	}
}  // 지진 이벤트


