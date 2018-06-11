using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloodEffect : MonoBehaviour
{
	GameObject tsunamiObject = null;
	public int level = 0;
    private Sprite _zeroInitailSprite = null;
    private Sprite _firstInitialSprite = null;

    List<TileUnit> _zero = null;
    List<TileUnit> _first = null;
    private List<GameObject> addedTiles = new List<GameObject>();

    public Sprite waterLevel;

	public void SetLevel(int level)
	{
		this.level = level;
	}

	public void SetActive(bool state)
	{
		gameObject.SetActive(state);
	}

    public void Init() {

        var zero = RandomMapGenerator.Instance.GetTilesByHeight(0);
        var first = RandomMapGenerator.Instance.GetTilesByHeight(1);

        _zero = zero;
        _first = first;
    }

	// Use this for initialization
	void Start()
	{
        //tsunamiObject = EventManager.Manager.MakeWorldTsunami(this.transform);
        //tsunamiObject.SetActive (true);

        
    }

    public void ClearWaters() {
        foreach (var g in addedTiles) {
            GameObject.Destroy(g);
        }
        addedTiles.Clear();
    }

    public void AddHalf(int level) {
        if (level == 0)
        {
            AddHalfWaterSprite(_zero);
        }
        else {
            AddHalfWaterSprite(_first);
        }
    }

    public void AddHalfWaterSprite(List<TileUnit> target) {
        SpriteRenderer renderer = new GameObject("water").AddComponent<SpriteRenderer>();
        renderer.sprite = waterLevel;
        renderer.color = new Color(1f, 1f, 1f, 0.5f);
        foreach (var tile in target) {
            GameObject copy = Instantiate(renderer.gameObject);
            copy.transform.SetParent(tile.transform);
            copy.transform.localScale = Vector3.one;
            copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
            copy.transform.localPosition = new Vector3(0f, 0.125f, 0f);
            copy.GetComponent<SpriteRenderer>().sortingOrder = tile.spriteRenderer.sortingOrder;

            addedTiles.Add(copy);
        }
        Destroy(renderer.gameObject);
    }

	// Update is called once per frame
	void Update()
	{
		//float speed = 1.0f;
		//float step = speed * Time.deltaTime;
		//tsunamiObject.transform.position = Vector3.MoveTowards(tsunamiObject.transform.position, Vector3.one, step);
	}
}

