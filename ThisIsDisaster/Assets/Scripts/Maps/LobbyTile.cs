using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTile : MonoBehaviour
{
    public const float _DEF_HEIGHT = -0.5f + 0.135f;
    public TempTileModel model;
    public SpriteRenderer spriteRenderer;
    public Vector3 originalPosition;

    public int x = 0, y = 0;
    public int HeightLevel = 0;

    public void SetPosition(Vector3 pos) {
        originalPosition = pos;
        transform.localPosition = pos;
    }

    public void SetCoord(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void SetHeight(int heightLevel)
    {
        HeightLevel = heightLevel;
        spriteRenderer.transform.localPosition = new Vector3(0, 0.25f * heightLevel + _DEF_HEIGHT, 0f);

        spriteRenderer.sortingOrder = -(x - y) * 33 + HeightLevel;
    }

    public void SetModel(TempTileModel model) {
        this.model = model;
    }
}
