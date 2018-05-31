using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EarthquakeEvent : EventBase
{
    GameObject quakeObject = null;          // 공통2  (맵흔들림)
    GameObject crackObject = null;          // 공통1 (갈라짐)

    TileUnit _originTile = null;
    EarthquakeEffect _effect = null;
    
    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 2f;
    public float damageTime = 1f;

    public EarthquakeEvent()
    {
        type = WeatherType.Earthquake;
    }

    public override void OnGenerated()
    {
        quakeObject = null;
        crackObject = null;

        _effect = EventManager.Manager.GetEarthquakeEffect();

        SetOriginTile();
        MakeBirdEffect();
    }

    public void MakeBirdEffect() {
        GameObject effect = Prefab.LoadPrefab("Events/BirdFlyingEffect");
        effect.transform.SetParent(EventManager.Manager.transform);
        Vector3 pos = _originTile.transform.position;
        effect.transform.position = pos;

        //effect.transform.localRotation = Quaternion.Euler(30f, 90f, 0f);
        float value = pos.x / (Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y));
        float c = Mathf.Acos(value) * Mathf.Rad2Deg;
        Debug.Log(c);
        Vector3 current = new Vector3(0f, 0f, c - 180f);
        //current.y = 0f;
        effect.transform.localRotation = Quaternion.Euler(current);
        effect.transform.localScale = Vector3.one;
        Debug.Log("Make Bird");
    }

    public void SetOriginTile() {
        TileUnit originTile = RandomMapGenerator.Instance.GetRandomTileByHeight(0);
        _originTile = originTile;
        _effect.SetOriginTile(originTile);
    }

	public override void OnStart()
	{
		_effect.SetActive(true);

        //_effect.SetEarthquakeType(EarthquakeEffect.EarthquakeType.Main, 3);
        //_effect.StartWave();
        //synchronization need
        _effect.SetEndEvent(EndEvent);
        _effect.StartEarthquakeEffect(GameManager.StageClockInfo.EVENT_RUN_TIME);
        _damageTimer.StartTimer(damageTime);
	}

    void EndEvent() {
        EventManager.Manager.EndEvent(this.type);
    }

    public override void OnExecute()
    {
        if (_damageTimer.started)
        {
            if (_damageTimer.RunTimer())
            {
                float speedDownValue = 0.4f;
                CharacterModel.Instance.SubtractHealth(damageHealthPerSec);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec);
                CharacterModel.Instance.SetSpeedFactor(1f - speedDownValue);
                _damageTimer.StartTimer();
            }
        }
    }
    public override void OnEnd()
	{
		quakeObject = null;
		crackObject = null;
		_effect.ReturnTiles();
		_effect.SetActive(false);

        CharacterModel.Instance.SetSpeedFactor();
	}

	public override void OnDestroy()
	{

	}
}  // 지진 이벤트


