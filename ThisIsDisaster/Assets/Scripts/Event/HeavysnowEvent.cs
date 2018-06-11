using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HeavysnowEvent : EventBase
{
	int max = -1;//max level of this event
    Timer _lifeTimeTimer = new Timer();
    float lifeTime = GameManager.StageClockInfo.EVENT_RUN_TIME;
    SnowEffect snowEffect = null;
	GameObject cloudEffect = null;
    BlurFilter EffectFilter
    {
        get
        {
            if (_effectFilter == null)
            {
                _effectFilter = Camera.main.gameObject.GetComponent<BlurFilter>();
                if (_effectFilter == null)
                {
                    _effectFilter = Camera.main.gameObject.AddComponent<BlurFilter>();
                }
            }
            return _effectFilter;
        }
    }
    BlurFilter _effectFilter = null;

    Timer _damageTimer = new Timer();
    public float damageHealthPerSec = 1f;
    public float damageEnergyPerSec = 1f;
    public float damageTime = 1f;

    Timer _effectDelayTimer = new Timer();
    private const float _EFFECT_DELAYTIME = 10f;
    private bool _isEnd = false;
    private const float _EFFECT_ALPHA_MAX = 0.35f;

    private List<SpriteRenderer> _addedTiles = new List<SpriteRenderer>();
    private List<TileUnit> zeroTile = new List<TileUnit>();
    SoundPlayer _sound = null;

    public HeavysnowEvent()
	{
		type = WeatherType.Heavysnow;
	}

	public override void OnGenerated()
	{
		snowEffect = EventManager.Manager.GetSnowEffect();
		cloudEffect = EventManager.Manager.GetCloudyEffect();
        cloudEffect.SetActive(false);
        EffectFilter.enabled = false;

        zeroTile = RandomMapGenerator.Instance.GetTilesByHeight(0);
    }

	public override void OnStart()
	{
        _isEnd = false;
        snowEffect.SetActive(true);
        cloudEffect.SetActive(true);
        snowEffect.SetLevel(5);
		//EventManager.Manager.SetWorldFilterColor(new Color(71f/255f, 73f/255f, 73f/255f, 98f/255f));

		//Debug.Log(RandomMapGenerator.Instance.GetRandomTileByHeight (1));

        _lifeTimeTimer.StartTimer(lifeTime);
        _damageTimer.StartTimer(damageTime);
        EffectFilter.enabled = true;
        EffectFilter.Amount = 9f;
        EffectFilter.Glow = 0.1f;
        //cloudEffect.SetActive(true);

        _effectDelayTimer.StartTimer(_EFFECT_DELAYTIME);
        AddSnowSprite();
        _sound = PlaySoundCamera("event_Heavysnow");
        _sound.PlayLoop();
    }

    void SetSnowAlpha(float alpha) {
        Color c = new Color(1f, 1f, 1f, alpha);
        foreach (var rend in _addedTiles) {
            rend.color = c;
        }
    }

    void AddSnowSprite() {
        SpriteRenderer render = new GameObject("snow").AddComponent<SpriteRenderer>();
        render.sprite = Resources.Load<Sprite>("Sprites/2D_Iso_Tile_Pack_Starter/Sprites/Snow/ISO_Tile_Snow_01");
        render.color = new Color(1f, 1f, 1f, 0f);
        foreach (var tile in zeroTile) {
            GameObject copy = GameObject.Instantiate(render.gameObject);
            copy.transform.SetParent(tile.transform);
            copy.transform.localScale = Vector3.one;
            copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
            copy.transform.localPosition = new Vector3(0f, 0.125f, 0f);
            var rend = copy.GetComponent<SpriteRenderer>();
            rend.sortingOrder = tile.spriteRenderer.sortingOrder;
            _addedTiles.Add(rend);
        }

        GameObject.Destroy(render);
    }

	public override void OnEnd()
	{
		snowEffect.SetActive(false);
		cloudEffect.SetActive(false);
        EffectFilter.enabled = false;
        if (_sound != null) {
            _sound.Stop();
        }
    }

	public override void OnDestroy()
	{

	}


    public override void OnExecute()
    {
        if (_lifeTimeTimer.started)
        {
            if(_lifeTimeTimer.RunTimer());
            //이벤트 끝내는 함수 필요
        }

        if (_effectDelayTimer.started) {
            float rate = _effectDelayTimer.Rate;
            float snowRate = _effectDelayTimer.Rate;
            if (_effectDelayTimer.RunTimer()) {
                rate = 1f;
                snowRate = 1f;
            }
            rate *= _EFFECT_ALPHA_MAX;
            if (_isEnd)
            {
                rate = _EFFECT_ALPHA_MAX - rate;
            }

            SpriteRenderer rend = cloudEffect.GetComponent<SpriteRenderer>();
            if (rend != null) {
                Color c = rend.color;
                c.a = rate;
                rend.color = c;
            }
            SetSnowAlpha(snowRate);
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

                if (player.GetPlayerModel().IsInShelter())
                {
                    healthDamageRate *= 0.5f;
                    staminaDamageRate *= 0.5f;
                    speedDownRate = 0f;
                }

                //CharacterModel.Instance.SubtractHealth(damageHealthPerSec * healthDamageRate);
                OnGiveDamageToPlayer(damageHealthPerSec * healthDamageRate);
                CharacterModel.Instance.SubtractStamina(damageEnergyPerSec * staminaDamageRate);
                CharacterModel.Instance.SetSpeedFactor(1f - speedDownRate);

                _damageTimer.StartTimer(damageTime);
            }
        }
    }
}  // 폭설 이벤트
	

