using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EarthquakeEvent : EventBase
{
	GameObject quakeObject = null;          // 공통2  (맵흔들림)
	GameObject crackObject = null;          // 공통1 (갈라짐)

    TileUnit _originTile = null;
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
		_effect.SetActive(true);

        //_effect.SetEarthquakeType(EarthquakeEffect.EarthquakeType.Main, 3);
        //_effect.StartWave();

        TileUnit originTile = RandomMapGenerator.Instance.GetRandomTileByHeight(0);
        //synchronization need
        _originTile = originTile;
        _effect.SetEndEvent(EndEvent);
        _effect.SetOriginTile(originTile);
        _effect.StartEarthquakeEffect(60f);
	}

    void EndEvent() {
        EventManager.Manager.EndEvent(this.type);
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


