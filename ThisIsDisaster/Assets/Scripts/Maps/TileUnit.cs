using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    WATER,
    GROUND_WALKABLE,
    GROUND_UNWALKABLE
}

public class TileUnit : MonoBehaviour {
    public TileType type;
    public TempTileModel _model;
    public SpriteRenderer spriteRenderer;

    public int HeightLevel = 0;
    public Vector3 originalPosition;

    public bool isNearWater = false;
    public void SetModel(TempTileModel model){
        _model = model;
        //spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SetPosition(Vector3 pos) {
        originalPosition = pos;
        transform.localPosition = pos;
    }

    public void SetHeight(int heightLevel) {
        HeightLevel = heightLevel;
        spriteRenderer.transform.localPosition = new Vector3(0, 0.25f * heightLevel, 0f);
    }
}
