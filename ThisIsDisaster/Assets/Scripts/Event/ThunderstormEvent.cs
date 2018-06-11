using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    이벤트 발생 시 비오게
    시간 약간 지나면 화면 어두워지게
    20초 지나면 낙뢰 멈추고
    이벤트 끝나면 비 점점 안오게
*/
public class ThunderstormEvent : EventBase
{
	GameObject rainObject = null;
	GameObject darkObject = null;
	GameObject blinkObject = null;      //  깜빡이
	GameObject lightningObject = null;     // 낙뢰 효과

    Timer _lifeTimeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;

    Timer _blinkTimer = new Timer();
    Timer _damageTimer = new Timer();
    //public float damageHealthPerSec = 0.5f;
    //public float damageEnergePerSec = 2f;
    public float damageHealthPerSec = 0f;
    public float damageEnergePerSec = 0f;
    public float damageTime = 1f;

    Timer _thunderTimer = new Timer();
    const float _THUNDER_FREQ_MIN = 5f;
    const float _THUNDER_FREQ_MAX = 10f;

    Timer _worldDarkEffectTimer = new Timer();
    const float _WORLD_DARK_EFFECT_TIME = 10f;
    const float _DARK_ALPHA_MAX = 0.5f;

    const float _THUNDER_HIT_DAMAGE = 30f;
    const int _RANGE_MIN = -2;
    const int _RANGE_MAX = 2;

    private Queue<TileUnit> _thunderFallPos = new Queue<TileUnit>();
    int thunderFallCount = 6;

    ThunderRenderEffect ThunderEffect
    {
        get
        {
            return _thunderRender.GetComponent<ThunderRenderEffect>();
        }
    }
    GameObject _thunderRender;

    public ThunderstormEvent()
	{
		type = WeatherType.Thunderstorm;
	}

	public override void OnGenerated()
	{
		//rainObject = EventManager.Manager.MakeWorldRain();
		darkObject = EventManager.Manager.MakeWorldDark();
        blinkObject = EventManager.Manager.MakeWorldBlink();
        lightningObject = EventManager.Manager.MakeWorldLightning();
        
	}

    GameObject MakeThunderHit(TileUnit pos) {
        GameObject copy = Prefab.LoadPrefab("Events/ThunderHitEffect");
        copy.transform.SetParent(EventManager.Manager.transform);
        copy.transform.position = pos.transform.position;
        copy.transform.localScale = Vector3.one;
        copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
        return copy;
    }

	public override void OnStart()
	{
		//rainObject.SetActive(true);
		darkObject.SetActive(true);
        //rainObject.SetActive(true);
        

        _lifeTimeTimer.StartTimer(lifeTime);
        _damageTimer.StartTimer(damageTime);
        //_thunderTimer.StartTimer(UnityEngine.Random.Range(_THUNDER_FREQ_MIN, _THUNDER_FREQ_MAX));

        _worldDarkEffectTimer.StartTimer(_WORLD_DARK_EFFECT_TIME);
        LoadThunderRender();

        for (int i = 0; i < thunderFallCount; i++) {
            var pos = RandomMapGenerator.Instance.GetRandomTileByHeight_Sync(3);
            _thunderFallPos.Enqueue(pos);
            pos.spriteRenderer.color = Color.red;
        }
        //StartDamageByEvent(eventDamage, damageTime);
    }

    public override void OnExecute()
    {
        if (_worldDarkEffectTimer.started)
        {
            float alpha = _worldDarkEffectTimer.Rate;
            if (_worldDarkEffectTimer.RunTimer())
            {
                alpha = 1f;

                //_rainForceTimer.StartTimer(_RAIN_FORCE_TIME);
                    StartThunderTimer();
            }
            //SetRainAlpha(alpha);

            var rend = darkObject.GetComponent<SpriteRenderer>();
            var color = rend.color;
            color.r = color.g = color.b = 0f;
            color.a = alpha * _DARK_ALPHA_MAX;
            rend.color = color;
        }

        if (_thunderTimer.RunTimer())
        {
            InvokeThunder();
            if (_thunderFallPos.Count > 0)
                StartThunderTimer();
        }

        if (_lifeTimeTimer.started)
        {
            _lifeTimeTimer.RunTimer();
            
/*
            if (_lifeTimeTimer.elapsed < 5) // 어둠, 비 이펙트 시작
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
            if (_lifeTimeTimer.elapsed > 10 && _lifeTimeTimer.elapsed < 20) // 비 이펙트 끝
            {
                //var rainParticle = rainObject.GetComponent<ParticleSystem>();
                //if (rainParticle != null)
                //{
                //        rainParticle.maxParticles -= 5;
                //}
                //damageEnergePerSec = 0;
            }
            if (_lifeTimeTimer.elapsed > _lifeTimeTimer.maxTime - 10f) // 어둠 이펙트 끝
            {

                SpriteRenderer renderer = darkObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Color color = renderer.color;
                    if (color.a > 0)
                        color.a -= 0.002f;
                    renderer.color = color;
                }
            }

        */


        }

        if(_damageTimer.started)
        {

            //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1f;
                float staminaDamageRate = 1f;
                var player = CharacterModel.Instance;
                if (player.GetPlayerModel().IsInShelter())
                {
                    healthDamageRate *= 0.5f;
                    staminaDamageRate *= 0.5f;
                }

                OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergePerSec * staminaDamageRate);

                _damageTimer.StartTimer(damageTime);
            }
                
        }
    }

    private void StartLightning()
    {
        lightningObject.SetActive(true);
    }

    /*public void StartDamageByEvent(float damage, float time)
    {
        eventDamage = damage;
        damageTime = time;
        _damageTimer.StartTimer(time);
    }*/

    public override void OnEnd()
	{
		//rainObject.SetActive(false);
		darkObject.SetActive(false);

	}

	public override void OnDestroy()
	{

    }

    void StartThunderTimer()
    {
        _thunderTimer.StartTimer(UnityEngine.Random.Range(_THUNDER_FREQ_MIN, _THUNDER_FREQ_MAX));
    }

    void LoadThunderRender() {
        _thunderRender = Prefab.LoadPrefab("Events/ThunderRenderEffect");
        _thunderRender.transform.SetParent(Camera.main.transform);
        _thunderRender.transform.localPosition = new Vector3(0f, 0f, 1f);
        _thunderRender.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _thunderRender.transform.localScale = new Vector3(10f, 10f, 1f);
        _thunderRender.gameObject.SetActive(false);

        ThunderEffect.SetEndEvent(OnEndThunder);
    }

    void InvokeThunder()
    {
        darkObject.SetActive(false);
        ThunderEffect.StartEffect(UnityEngine.Random.Range(1f, 1.5f));

        TileUnit pos = _thunderFallPos.Dequeue();
        if (pos != null) {
            MakeThunderHit(pos);
            for (int i = _RANGE_MIN; i <= _RANGE_MAX; i++) {
                for (int j = _RANGE_MIN; j <= _RANGE_MAX; j++) {
                    TileUnit tile = RandomMapGenerator.Instance.GetTile(pos.x + i, pos.y + j);
                    if (tile != null) {
                        foreach (var unit in tile._currentEnteredUnits) {
                            if (unit is NPC.NPCModel) {
                                NPC.NPCModel model = (unit as NPC.NPCModel);
                                if (model.CurrentHp > 0f) {
                                    model.OnTakeDamage(null, _THUNDER_HIT_DAMAGE);
                                }
                            }
                            else if (unit is PlayerModel)
                            {
                                OnGiveDamageToPlayer(_THUNDER_HIT_DAMAGE);
                            }
                            else {
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnEndThunder()
    {
        darkObject.SetActive(true);
        _thunderRender.SetActive(false);
    }

}  // 낙뢰 이벤트
