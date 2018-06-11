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
    private Dictionary<int, List<SpriteRenderer>> addedDics = new Dictionary<int, List<SpriteRenderer>>();
    private Timer _alphaTimer = new Timer();
    private int _currentAlphaLevel = 0;
    private const float _ALPHA_TIME = 10f;
    private const float _ALPHA_VALUE = 0.5f;

    private bool _isInvert = false;

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

    public void SetListAlpha(int level, float alpha) {
        if (addedDics.ContainsKey(level) == false) return;
        Color c = new Color(1f, 1f, 1f, alpha);
        foreach (var r in addedDics[level])
        {
            r.color = c;
        }
    }

    public void ClearWaters() {
        foreach (var g in addedDics) {
            foreach (var r in g.Value) {
                Destroy(r.gameObject);
            }
        }
        addedDics.Clear();
    }

    public void AddHalf(int level) {
        _currentAlphaLevel = level;
        if (level == 0)
        {
            AddHalfWaterSprite(level, _zero);
        }
        else {
            AddHalfWaterSprite(level, _first);
        }
    }

    public void DisappearWater() {
        _alphaTimer.StartTimer(_ALPHA_TIME);
        _isInvert = true;
    }

    public void AddHalfWaterSprite(int level, List<TileUnit> target) {
        _alphaTimer.StartTimer(_ALPHA_TIME);

        SpriteRenderer renderer = new GameObject("water").AddComponent<SpriteRenderer>();
        renderer.sprite = waterLevel;
        renderer.color = new Color(1f, 1f, 1f, 0f);
        List<SpriteRenderer> currentList = new List<SpriteRenderer>();
        foreach (var tile in target) {
            GameObject copy = Instantiate(renderer.gameObject);
            copy.transform.SetParent(tile.transform);
            copy.transform.localScale = Vector3.one;
            copy.transform.localRotation = Quaternion.Euler(Vector3.zero);
            copy.transform.localPosition = new Vector3(0f, 0.125f, 0f);
            var rend = copy.GetComponent<SpriteRenderer>();
            rend.sortingOrder = tile.spriteRenderer.sortingOrder;
            currentList.Add(rend);
        }
        addedDics.Add(level, currentList);

        Destroy(renderer.gameObject);
    }

	// Update is called once per frame
	void Update()
	{
        if (_alphaTimer.started) {
            float alpha = _alphaTimer.Rate * _ALPHA_VALUE;
            if (_alphaTimer.RunTimer()) {
                alpha = _ALPHA_VALUE;
            }

            if (_isInvert)
            {
                alpha = _ALPHA_VALUE - alpha;
                SetListAlpha(0, alpha);
                SetListAlpha(1, alpha);
            }
            else {

                SetListAlpha(_currentAlphaLevel, alpha);
            }

        }
		//float speed = 1.0f;
		//float step = speed * Time.deltaTime;
		//tsunamiObject.transform.position = Vector3.MoveTowards(tsunamiObject.transform.position, Vector3.one, step);
	}
}

