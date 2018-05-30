using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthquakeEffect : MonoBehaviour
{
    public enum EarthquakeType {
        Prev,//전조
        Main,//본진
        After//여진 
    }
    private const float _xyMin = 0.005f;
    private const float _xyMax = 0.025f;
    private const float _speedMin = 10f;
    private const float _speedMax = 40f;

    private const int _sinfreq = 3;
    private const float _waveHeight = 0.4f;

    public const int MaxLevel = 8;
    [Range(0, MaxLevel)]
    public int level = 0;

    public AnimationCurve EarthquakeForceCurve;

    public delegate void EndEvent();

    EndEvent _endEvent = null;

    EarthquakeFilter Filter {
        get {
            if (_filter == null)
            {
                _filter = Camera.main.gameObject.AddComponent<EarthquakeFilter>();
            }
            return _filter;
        }
    }
    EarthquakeFilter _filter = null;

    public bool _debug = false;
    Timer _lifeTimeTimer = new Timer();
    Timer _waveTimer = new Timer();
    int _waveCount = 0;
    float _waveFreq = 0.1f;

#if MIDDLE_PRES
    bool _injury = false;
#endif


    TileUnit _originTile = null;

    public void SetEndEvent(EndEvent e) {
        this._endEvent = e;
    }

    public void SetLevel(int level) {
        this.level = level;
        float f = level / (float)MaxLevel;
        float _xy = Mathf.Lerp(_xyMin, _xyMax, f);
        float _speed = Mathf.Lerp(_speedMin, _speedMax, f);
        Filter.Speed = _speed;
        Filter.x = Filter.y = _xy;
    }

    public void SetLevel(float factor) {
        factor = Mathf.Clamp(factor, 0f, 1f);
        float _xy = Mathf.Lerp(_xyMin, _xyMax, factor);
        float _speed = Mathf.Lerp(_speedMin, _speedMax, factor);
        Filter.Speed = _speed;
        Filter.x = Filter.y = _xy;
    }

    public float GetEarthquakeForce(float rate) {
        return EarthquakeForceCurve.Evaluate(rate);
    }

    public void StartEarthquakeEffect(float lifetime) {
        _lifeTimeTimer.StartTimer(lifetime);
        StartWave();
    }

    //public void SetEarthquakeType(EarthquakeType type, int Level) {
    //    if (level == 0)
    //    {
    //        //SetActive(false);
    //    }
    //    else {
    //        //SetActive(true);
    //    }
    //    switch (type) {
    //        case EarthquakeType.Prev:
    //            SetLevel(1);
    //            break;
    //        case EarthquakeType.Main:
    //            //본 지진은 최소 3레벨에서 시작
    //            int lv = Mathf.Max(3, Level);
    //            SetLevel(lv);
    //            break;
    //        case EarthquakeType.After:
    //            SetLevel(2);
    //            break;
    //    }
    //}

    public void SetActive(bool state) {
        gameObject.SetActive(state);
        Filter.enabled = state;
    }

    private void Start()
    {
        //SetActive(false);
    }

    public void StartWave() {
        //SetOriginTile();
        SetWaveParams(50);

        _waveCount = 0;
        _waveTimer.StartTimer(_waveFreq);
    }

#if MIDDLE_PRES
    void MakeInjury() {
        CharacterModel character = GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<CharacterModel>();
        character.GetDisorder(Disorder.DisorderType.injury);
        InGameUIScript.Instance.DisorderNotice(Disorder.DisorderType.injury);
    }
#endif

    private void Update()
    {
        if (_lifeTimeTimer.started)
        {
            float rate = GetEarthquakeForce((_lifeTimeTimer.Rate));
#if MIDDLE_PRES
            if (!_injury) {
                if (rate >= 0.6f) {
                    MakeInjury();
                    _injury = true;
                }
            }
#endif

            if (_waveTimer.started)
            {
                if (_waveTimer.RunTimer())
                {
                    MakeWaveEffect(rate);
                    _waveCount++;
                    _waveTimer.StartTimer(_waveFreq);
                }
            }

            SetLevel(rate);

            if (_lifeTimeTimer.RunTimer()) {
                if (_endEvent != null) {
                    _endEvent();
                    _endEvent = null;
                }
            }
        }
        //if (_debug) {
        //    if (_lifeTimeTimer.started == false)
        //    {
        //        _lifeTimeTimer.StartTimer(15f);
        //        //SetActive(true);
        //    }
        //    else {
        //        float rate = _lifeTimeTimer.Rate;
        //        SetLevel(rate);

        //        if (_lifeTimeTimer.RunTimer()) {
        //            _debug = false;
        //            //SetActive(false);
        //        }
        //    }
        //}
    }

    private void LateUpdate()
    {
        if (_originTile != null) {

        }
    }

    public void SetOriginTile(TileUnit tile) {
        _originTile = tile;

    }

    private int _maxDistance;
    public void SetWaveParams(int maxDist) {
        _maxDistance = maxDist;
    }

    public void MakeWaveEffect(float factor) {
        for (int i = -_maxDistance; i <= _maxDistance; i++) {
            int x = _originTile.x + i;

            for (int j = -_maxDistance; j <= _maxDistance; j++) {
                int y = _originTile.y + j;
                TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);
                if (tile == null) continue;

                int dist = Mathf.Max(Mathf.Abs(i), Mathf.Abs(j));

                float sin = CalcSin(dist + _waveCount* _sinfreq);
                tile.AddHeight(sin * _waveHeight * factor);
            }
        }
    }

    public void ReturnTiles()
    {
        for (int i = -_maxDistance; i <= _maxDistance; i++)
        {
            int x = _originTile.x + i;

            for (int j = -_maxDistance; j <= _maxDistance; j++)
            {
                int y = _originTile.y + j;
                TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);
                if (tile == null) continue;
                tile.AddHeight(0f);
            }
        }
        _originTile = null;
    }

    /// <summary>
    /// Dist will be normal
    /// </summary>
    /// <param name="dist"></param>
    /// <returns></returns>
    float CalcSin(int dist) {
        return Mathf.Sin(dist * 30 * Mathf.Deg2Rad);
    }
}
