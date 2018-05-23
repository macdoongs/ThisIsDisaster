using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    WATER,
    GROUND_WALKABLE,
    GROUND_UNWALKABLE
}

public class TileUnit : MonoBehaviour {
    private const float _arrivalDiff = 0.2f;
    public const float _DEF_HEIGHT = -0.5f + 0.135f;
    public TileType type;
    public TempTileModel _model;
    public SpriteRenderer spriteRenderer;
    public new Collider2D collider;

    public int HeightLevel = 0;
    public Vector3 originalPosition;
    public bool isNearWater = false;

    public int x = 0, y = 0;

    public delegate void OnTileEnter(UnitModel target);
    private OnTileEnter _enter = null;

    public void SetModel(TempTileModel model) {
        _model = model;
    }

    public void SetPosition(Vector3 pos) {
        originalPosition = pos;
        transform.localPosition = pos;
        Vector3 colPos = collider.transform.position;
        colPos.z = RandomMapGenerator.Instance.transform.position.z;
        collider.transform.position = colPos;
    }

    public void SetCoord(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetHeight(int heightLevel) {
        HeightLevel = heightLevel;
        spriteRenderer.transform.localPosition = new Vector3(0, 0.25f * heightLevel + _DEF_HEIGHT, 0f);

        spriteRenderer.sortingOrder = -(x - y) * RandomMapGenerator.SPRITE_ORDER_INTERVAL + HeightLevel;
        //_text.text = GetSpriteOrder().ToString();
    }

    public void AddHeight(float height) {
        spriteRenderer.transform.localPosition = new Vector3(0, 0.25f * this.HeightLevel + _DEF_HEIGHT + height, 0f);

    }

    public void SetRendererAlpha(float alpha) {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    public bool IsPassable(UnitModel passTarget) {
        if (passTarget is PlayerModel) {
            return true;
        }
        if (passTarget is NPC.NPCModel) {
            return HeightLevel > 0;
        }
        return false;
    }

    public void SetEnterAction(OnTileEnter enter) {
        _enter = enter;
    }

    public void OnEnterTile(UnitModel target) {
        if (_enter != null) {
            _enter(target);
        }
    }

    public int GetSpriteOrder() { return spriteRenderer.sortingOrder; }

    public bool IsArrived(Vector3 pos)
    {
        return Vector2.Distance(pos, transform.position) <= _arrivalDiff;
    }

    public override string ToString()
    {
        return "Tile[" + x + " , " + y + "]";
    }
}
