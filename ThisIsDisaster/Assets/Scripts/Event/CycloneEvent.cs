using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CycloneEvent : EventBase, IObserver
{
    CycloneEffect cycloneObject = null;
    GameObject darkObject = null;

    Timer _worldDarkEffectTimer = new Timer();
    const float _WORLD_DARK_EFFECT_TIME = 10f;
    const float _DARK_ALPHA_MAX = 0.5f;
    ParticleSystem rainObject = null;
    Timer _rainForceTimer = new Timer();
    const float _RAIN_FORCE_TIME = 15f;
    const float _RAIN_FORCE_MAX = 10f;

    Timer _thunderTimer = new Timer();
    const float _THUNDER_FREQ_MIN = 5f;
    const float _THUNDER_FREQ_MAX = 10f;


    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 2f;
    public float damageTime = 1f;

    GameObject _thunderRender = null;
    ThunderRenderEffect ThunderEffect {
        get {
            return _thunderRender.GetComponent<ThunderRenderEffect>();
        }
    }

    public CycloneEvent() {
        type = WeatherType.Cyclone;
    }

    public override void OnGenerated()
    {
        cycloneObject = EventManager.Manager.GetCycloneEffect();
        darkObject = EventManager.Manager.MakeWorldDark();
        
    }

    public override void OnStart()
    {
        ObserveNotices();
        cycloneObject.SetActive(true);
        darkObject.SetActive(true);

        //Make Thunder Sound
        StartRain();
        SetRainAlpha(0f);
        _worldDarkEffectTimer.StartTimer(_WORLD_DARK_EFFECT_TIME);
        _damageTimer.StartTimer(damageTime);

        LoadThunderRender();

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

    void InvokeThunder() {
        darkObject.SetActive(false);
        ThunderEffect.StartEffect(UnityEngine.Random.Range(0.5f, 1f));
    }

    public void OnEndThunder() {
        darkObject.SetActive(true);
        _thunderRender.SetActive(false);
    }

    public void StartRain() {
        GameObject rain = Prefab.LoadPrefab("Events/CycloneRain");
        rain.transform.SetParent(Camera.main.transform);
        rain.transform.localPosition = new Vector3(-2f, 9f, 10f);
        rain.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        rain.transform.localScale = Vector3.one;
        this.rainObject = rain.GetComponent<ParticleSystem>();

        if (CharacterModel.Instance) {
            if (CharacterModel.Instance.GetPlayerModel().IsInShelter()) {
                var rend = rainObject.GetComponent<ParticleSystemRenderer>();
                rend.enabled = false;
            }
        }
    }

    public void SetRainAlpha(float alpha)
    {
        var ps = rainObject;
        var module = ps.main;
        var color = module.startColor;
        color.color = new Color(1f, 1f, 1f, Mathf.Clamp(alpha, 0f, 1f));
        module.startColor = color;
    }

    public void SetRainForce(float rate) {
        var forceModule = rainObject.forceOverLifetime;
        forceModule.xMultiplier = rate * _RAIN_FORCE_MAX;
        
    }

    void StartThunderTimer() {
        _thunderTimer.StartTimer(UnityEngine.Random.Range(_THUNDER_FREQ_MIN, _THUNDER_FREQ_MAX));
    }

    public override void OnExecute()
    {
        if (_worldDarkEffectTimer.started) {
            float alpha = _worldDarkEffectTimer.Rate;
            if (_worldDarkEffectTimer.RunTimer()) {
                alpha = 1f;

                _rainForceTimer.StartTimer(_RAIN_FORCE_TIME);
                StartThunderTimer();
            }
            SetRainAlpha(alpha);

            var rend = darkObject.GetComponent<SpriteRenderer>();
            var color = rend.color;
            color.r = color.g = color.b = 0f;
            color.a = alpha * _DARK_ALPHA_MAX;
            rend.color = color;
        }

        if (_rainForceTimer.started) {
            float force = _rainForceTimer.Rate;
            if (_rainForceTimer.RunTimer()) {
                force = 1f;
            }
            SetRainForce(force);
        }

        if (_thunderTimer.RunTimer()) {
            StartThunderTimer();
            InvokeThunder();
        }

        if (_damageTimer.started)
        {
            //피난처 안에 있을 경우, 데미지가 반감되게 추가해야함.
            if (_damageTimer.RunTimer())
            {
                float healthDamageRate = 1f;
                float staminaDamageRate = 1f;
                float speedDownRate = 0.3f;

                var player = CharacterModel.Instance;

                if (player.HasItem(17)) {
                    healthDamageRate -= 0.2f;
                }

                if (player.HasItem(20004)) {
                    healthDamageRate -= 0.1f;
                }

                if (player.GetPlayerModel().IsInShelter()) {
                    healthDamageRate *= 0.5f;
                    staminaDamageRate *= 0.5f;
                    speedDownRate = 0f;
                }

                CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);
                CharacterModel.Instance.SetSpeedFactor(1f - speedDownRate);

                _damageTimer.StartTimer(damageTime);
            }
            
        }
    }

    public override void OnEnd()
	{
        RemoveNotices();
		cycloneObject.SetActive(false);
		darkObject.SetActive(false);
        ThunderEffect.gameObject.SetActive(false);
        rainObject.gameObject.SetActive(false);
        CharacterModel.Instance.SetSpeedFactor();
	}

	public override void OnDestroy()
	{

	}

    public void OnNotice(string notice, params object[] param)
    {
        if (NoticeName.OnPlayerEnterShelter == notice)
        {
            var rend = rainObject.GetComponent<ParticleSystemRenderer>();
            rend.enabled = false;
        }
        else {
            var rend = rainObject.GetComponent<ParticleSystemRenderer>();
            rend.enabled = true;
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.OnPlayerEnterShelter, this);
        Notice.Instance.Observe(NoticeName.OnPlayerExitShelter, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.OnPlayerEnterShelter, this);
        Notice.Instance.Remove(NoticeName.OnPlayerExitShelter, this);
    }
}      // 태풍 이벤트
	
