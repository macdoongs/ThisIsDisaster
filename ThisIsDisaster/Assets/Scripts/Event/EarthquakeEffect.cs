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

    public const int MaxLevel = 8;
    [Range(0, MaxLevel)]
    public int level = 0;

    EarthquakeFilter _filter = null;

    public bool _debug = false;
    Timer _lifeTimeTimer = new Timer();

    public void SetLevel(int level) {
        this.level = level;
        float f = level / (float)MaxLevel;
        float _xy = Mathf.Lerp(_xyMin, _xyMax, f);
        float _speed = Mathf.Lerp(_speedMin, _speedMax, f);
        _filter.Speed = _speed;
        _filter.x = _filter.y = _xy;
    }

    public void SetLevel(float factor) {
        factor = Mathf.Clamp(factor, 0f, 1f);
        float _xy = Mathf.Lerp(_xyMin, _xyMax, factor);
        float _speed = Mathf.Lerp(_speedMin, _speedMax, factor);
        _filter.Speed = _speed;
        _filter.x = _filter.y = _xy;
    }

    public void SetEarthquakeType(EarthquakeType type, int Level) {
        if (level == 0)
        {
            SetActive(false);
        }
        else {
            SetActive(true);
        }
        switch (type) {
            case EarthquakeType.Prev:
                SetLevel(1);
                break;
            case EarthquakeType.Main:
                //본 지진은 최소 3레벨에서 시작
                int lv = Mathf.Max(3, Level);
                SetLevel(lv);
                break;
            case EarthquakeType.After:
                SetLevel(2);
                break;
        }
    }

    public void SetActive(bool state) {
        _filter.enabled = state;
    }

    private void Start()
    {
        if (_filter == null) {
            _filter = Camera.main.gameObject.AddComponent<EarthquakeFilter>();
        }
        SetActive(false);
    }

    private void Update()
    {
        if (_debug) {
            if (_lifeTimeTimer.started == false)
            {
                _lifeTimeTimer.StartTimer(15f);
                SetActive(true);
            }
            else {
                float rate = _lifeTimeTimer.Rate;
                SetLevel(rate);

                if (_lifeTimeTimer.RunTimer()) {
                    _debug = false;
                    SetActive(false);
                }
            }
        }
    }
}
