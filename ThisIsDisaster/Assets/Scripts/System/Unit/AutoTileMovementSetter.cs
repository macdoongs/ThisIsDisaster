using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileMovementSetter : MonoBehaviour {
    const float _heightDelta = 0.25f;
    public delegate void OnTileChanged(TileUnit current);
    
    RandomMapGenerator _map;

    public RenderLayerChanger _changer = null;
    public Transform HeightPivot;

    OnTileChanged _changedAction = null;

    TileUnit _currentTile = null;
    Timer _heightChangeTimer = new Timer();

    public float _heightAscendTime = 0.1f;
    public float _heightDescendTime = 0.25f;

    float _targetHeight = 0f;
    float _initialHeight = 0f;

    // Use this for initialization
    void Start () {
        _map = RandomMapGenerator.Instance;
        if (_map == null)
        {
            enabled = false;
            return;
        }
        _currentTile = _map.GetTile(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
        var cur = _map.GetTile(transform.position);
        if (cur != _currentTile) {
            if (!cur) return;
            if (_changedAction != null) {
                _changedAction(cur);
            }
            _currentTile = cur;
            RenderOrderChange(_currentTile);
            HeightChange(_currentTile);
        }

        if (_heightChangeTimer.started) {
            float rate = Mathf.Lerp(_initialHeight, _targetHeight, _heightChangeTimer.Rate);
            var lp = HeightPivot.transform.localPosition;
            if (_heightChangeTimer.RunTimer()) {
                rate = _targetHeight;
            }
            lp.y = rate;
            HeightPivot.transform.localPosition = lp;
        }
	}

    public void SetChangeAction(OnTileChanged action) {
        _changedAction = action;
    }

    public void RenderOrderChange(TileUnit tile) {
        if (_changer)
            _changer.UpdateLayerInfo(tile.GetSpriteOrder() + 3);
    }

    public void HeightChange(TileUnit tile) {
        _targetHeight = tile.HeightLevel * _heightDelta;
        _initialHeight = HeightPivot.transform.localPosition.y;
        if (_targetHeight != _initialHeight) {
            float time = _targetHeight > _initialHeight ? _heightAscendTime : _heightDescendTime;
            _heightChangeTimer.StartTimer(time);
        }
    }

    public bool IsHeightChanging() {
        return _heightChangeTimer.started;
    }


}
