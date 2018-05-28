using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeavysnowEvent : EventBase
{
	int max = -1;//max level of this event
	SnowEffect snowEffect = null;
	CloudEffect cloudEffect = null;  

	public HeavysnowEvent()
	{
		type = WeatherType.Heavysnow;
	}

	public override void OnGenerated()
	{
		snowEffect = EventManager.Manager.GetSnowEffect();

		cloudEffect = EventManager.Manager.GetCloudyEffect();
	}

	public override void OnStart()
	{
		snowEffect.SetActive(true);
		snowEffect.SetLevel(5);
		EventManager.Manager.SetWorldFilterColor(new Color(71f/255f, 73f/255f, 73f/255f, 98f/255f));
		for (int i = 0; i < 10; i++) {
			NPCManager.Manager.MakeNPC (0);
		}

		Debug.Log(RandomMapGenerator.Instance.GetRandomTileByHeight (1));
		//cloudEffect.SetActive(true);

	}

	public override void OnEnd()
	{
		snowEffect.SetActive(false);
		cloudEffect.SetActive(false);
	}

	public override void OnDestroy()
	{

	}
}  // 폭설 이벤트
	

