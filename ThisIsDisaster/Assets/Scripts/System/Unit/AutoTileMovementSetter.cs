using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTileMovementSetter : MonoBehaviour {
    const float _heightDelta = 0.25f;
    public delegate void OnTileChanged(TileUnit current);
    public delegate void OnHeightChanged();
    
    RandomMapGenerator _map;

    public RenderLayerChanger _changer = null;
    public Transform HeightPivot;
    public UnitModel owner;

    OnTileChanged _changedAction = null;
    OnHeightChanged _heightChangeAction = null;

    TileUnit _currentTile = null;
    Timer _heightChangeTimer = new Timer();

    public float _heightAscendTime = 0.1f;
    public float _heightDescendTime = 0.25f;

    float _targetHeight = 0f;
    float _initialHeight = 0f;
    public bool DisplayStandingTile = false;

    // Use this for initialization
    void Start () {
        _map = RandomMapGenerator.Instance;
        if (_map == null)
        {
            enabled = false;
            return;
        }
        ChangeTile(_map.GetTile(transform.position));
        //RenderOrderChange(_currentTile);
        //HeightChange(_currentTile);
	}

    public void ChangeTile(TileUnit tile)
    {
        if (_changedAction != null)
        {
            _changedAction(tile);
        }
        if (DisplayStandingTile) {
            _currentTile.spriteRenderer.color = Color.white;
            tile.spriteRenderer.color = Color.blue;
        }

        _currentTile = tile;
        RenderOrderChange(_currentTile);
        HeightChange(_currentTile);
    }
	
	// Update is called once per frame
	void Update () {
        var cur = _map.GetTile(transform.position);
        if (cur != _currentTile) {
            if (!cur) return;
            
            ChangeTile(cur);
            if (owner != null) {
                cur.OnEnterTile(owner);
            }
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
        if (_currentTile != null)
            _changedAction(_currentTile);
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

        if (_heightChangeAction != null) {
            _heightChangeAction();
        }
    }

    public bool IsHeightChanging() {
        return _heightChangeTimer.started;
    }

    public void SetCurrentTileForcely(TileUnit tile) {
        ChangeTile(tile);
    }

    public void SetHeightChangeAction(OnHeightChanged height) {
        _heightChangeAction = height;
    }

    public TileUnit GetCurrentTile() { return _currentTile; }
}
