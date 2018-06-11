using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloodEvent : EventBase {
	GameObject rainObject = null;
	GameObject darkObject = null;
	FloodEffect _effect = null;
    
    Timer _lifeTimeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;

    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 1f;
    public float damageTime = 1f;

    
    public FloodEvent() {
		type = WeatherType.Flood;
	}

	public override void OnGenerated()
	{
		_effect = EventManager.Manager.GetFloodEffect();

		rainObject = EventManager.Manager.MakeWorldRain();
		darkObject = EventManager.Manager.MakeWorldDark();
        _effect.Init();
	}

	public override void OnStart()
	{
		_effect.SetActive (true);

		rainObject.SetActive(true);
		darkObject.SetActive(true);

        _damageTimer.StartTimer(damageTime);
        _lifeTimeTimer.StartTimer(lifeTime);

        _effect.AddHalf(0);
        _effect.AddHalf(1);
	}

	public override void OnEnd()
	{
		_effect.SetActive (false);

		rainObject.SetActive(false);
		darkObject.SetActive(false);

        _effect.ClearWaters();
		rainObject = null;
		darkObject = null;
	}

    public override void OnExecute()
    {
        if (_lifeTimeTimer.started)
        {
            _lifeTimeTimer.RunTimer();


            if (_lifeTimeTimer.elapsed < 5)
            {
                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a < 0.5f)
                        color.a += 0.01f;
                    renderer.color = color;
                }
                
            }
            if (_lifeTimeTimer.elapsed > _lifeTimeTimer.maxTime - 10f)
            {

                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a > 0)
                        color.a -= 0.002f;
                    renderer.color = color;
                }

                var rainParticle = rainObject.GetComponent<ParticleSystem>();
                if (rainParticle != null)
                {
                    var main = rainParticle.main;
                    main.maxParticles -= 5;
                    //rainParticle.maxParticles -= 5;
                }
            }

            if (_damageTimer.started)
            {
                if (_damageTimer.RunTimer())
                {
                    float healthDamageRate = 0.5f;
                    float staminaDamageRate = 1f;
                    float speedDownRate = 0.3f;

                    var player = CharacterModel.Instance;
                    

                    PlayerModel pm = player.GetPlayerModel();
                    if (pm != null) {
                        if (pm.IsInShelter())
                        {
                            //ignore all damage
                        }
                        else {
                            TileUnit current = pm.GetCurrentTile();
                            switch (current.HeightLevel) {
                                case 0:
                                case 1:
                                    healthDamageRate = 2;
                                    staminaDamageRate = 2;
                                    speedDownRate = 0.5f;
                                    break;
                                case 2:
                                    healthDamageRate = 1;
                                    staminaDamageRate = 2;
                                    speedDownRate = 0.3f;
                                    break;
                                case 3:
                                    healthDamageRate = 1;
                                    staminaDamageRate = 2;
                                    speedDownRate = 0.3f;
                                    break;
                            }

                            player.SetSpeedFactor(1f - speedDownRate);
                            OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                            player.SubtractStamina(damageEnergyPerSec * staminaDamageRate);
                        }
                    }

                   // CharacterModel.Instance.SetSpeedFactor(1f - speedDownRate);
                    

                    //CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                    //OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                    //CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);

                    _damageTimer.StartTimer(damageTime);
                }
            }

        }
    }

    public override void OnDestroy()
	{

	}
}  // 해일 이벤트

