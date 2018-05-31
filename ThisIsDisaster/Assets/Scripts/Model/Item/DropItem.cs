using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour {
    public ItemModel ItemModel { private set; get; }
    public SpriteRenderer ItemRenderer;

    private TileUnit _currentTile = null;
    public AutoTileMovementSetter TileSetter;
    private Timer _acquireDelayTimer = new Timer();
    const float _acquireDelay = 1f;

    public void SetModel(ItemModel item) {
        ItemModel = item;

        Sprite sprite = Resources.Load<Sprite>(item.metaInfo.spriteSrc);
        if (sprite != null) {
            float px = sprite.texture.width;
            int scaleInv = (int)(px / 64);
            if (scaleInv == 0) {
                scaleInv = 1;
            }
            float pixelFactor = 1f;
            if (sprite.pixelsPerUnit > 100) {
                pixelFactor = sprite.pixelsPerUnit / 100;
            }
            ItemRenderer.transform.localScale = Vector3.one / scaleInv * pixelFactor;
            ItemRenderer.sprite = sprite;
        }
    }

    public void SetTile(TileUnit tile) {
        _currentTile = tile;
        transform.position = tile.transform.position;
        TileSetter.SetCurrentTileForcely(tile);
    }

	// Use this for initialization
	void Start ()
    {
        _acquireDelayTimer.StartTimer(_acquireDelay);
    }
	
	// Update is called once per frame
	void Update () {

        if (_acquireDelayTimer.RunTimer())
        {

        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_acquireDelayTimer.started) return;
        PlayerSensor ps = collision.GetComponent<PlayerSensor>();
        if (!ps) return;
        if (ItemManager.Manager.OnTryAcquireItem(this, ps.Owner as PlayerModel))
            gameObject.SetActive(false);
    }
}
