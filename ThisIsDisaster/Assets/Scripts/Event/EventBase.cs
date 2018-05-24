using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EventBase {
	public WeatherType type = WeatherType.None;

	protected bool _isStarted = false;
	public bool IsStarted { get { return _isStarted; } set { _isStarted = value; } }
	public int Level = 0;

	public EventBase() {
		type = WeatherType.None;
	}

	public virtual void OnGenerated() { }

	public virtual void OnStart() {
	}

	public virtual void OnExecute() { }

	public virtual void OnEnd() { }

	public virtual void OnDestroy() { }

	public virtual void OnGiveDamage(UnitModel target, float DamageValue) { }
}
