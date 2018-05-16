using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("Utils/MapGenerator")]
[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class RandomMapGenerator : MonoBehaviour
{
    public const int SPRITE_ORDER_INTERVAL = 3;//타일 높이 카운트 개수
    public static RandomMapGenerator Instance { get; private set; }

    //x, list(y)
    Dictionary<int, List<TileUnit>> dic = new Dictionary<int, List<TileUnit>>();
    [ReadOnly]
    public int Width = 0;
    [ReadOnly]
    public int Height = 0;

    public GameObject _tileUnit;
    public Transform pivot;
    public List<Sprite> _randomTileSprites;
    public RectTransform _uiPivot;
    public int roomThresholdSize = 50;
    public bool useRandomSeed;
    public bool debugTest;
    const char sepMatch = '|';
    const float _xDelta = 0.5f;
    const float _yDelta = 0.25f;
    public float _zDelta = 0.1f;
    public float _zCastDist = 0f;

    public float _zPos = 10f;

    public int[,] worldMap = null;
     

    private void Awake()
    {
        Instance = this;
    }

    public void ClearMap() {
        foreach (var list in dic.Values) {
            foreach (var u in list)
            {
                if (u.gameObject == pivot.gameObject) continue;
                DestroyImmediate(u.gameObject);
            }
        }
        dic.Clear();

        foreach (Transform t in transform) {
            if (t.gameObject == pivot.gameObject) continue;
            DestroyImmediate(t.gameObject);
        }
    }

    public void UpdatePosition(int[,] map)
    {

        float xInitial = 0f;
        float yInitial = 0f;
        float zInitial = 0f;

        for (int xInd = 0; xInd < Width; xInd++)
        {
            List<TileUnit> list = GetVertical(xInd);

            for (int yInd = 0; yInd < Height; yInd++)
            {
                TileUnit curTile = list[yInd];
                float xPos = 0f;
                float yPos = 0f;
                float zPos = 0f;

                xPos = xInitial + yInd * _xDelta;
                yPos = yInitial - yInd * _yDelta;
                zPos = zInitial - yInd * _zDelta;

                curTile.SetPosition(new Vector3(xPos, yPos, zPos));
                curTile.SetCoord(xInd, yInd);
                curTile.SetHeight(map[xInd, yInd]);
            }
            xInitial += _xDelta;
            yInitial += _yDelta;
            zInitial += _zDelta;
        }
        transform.localPosition = new Vector3(-Width * 0.5f + GameStaticInfo.TileWidth_Half, 0f, _zPos);
    }

    List<TileUnit> GetVertical(int x)
    {
        List<TileUnit> output = null;
        if (dic.TryGetValue(x, out output))
        {
            return output;
        }
        output = new List<TileUnit>();
        dic.Add(x, output);
        return output;
    }

    Sprite GetRandomSprite() {
        return _randomTileSprites[UnityEngine.Random.Range(0, _randomTileSprites.Count)];
    }

    Sprite GetSprite(int isMovable) {
        return _randomTileSprites[isMovable];
        //return isMovable ? _randomTileSprites[0] : _randomTileSprites[1];
    }

    public void GenerateMapByAlgorithm(int[,] map, int w, int h) {
        ClearMap();
        this.Width = w;
        this.Height = h;

        worldMap = new int[w, h];
        
        for (int x = 0; x < Width; x++)
        {

            for (int y = 0; y < Height; y++)
            {
                GameObject copy = Instantiate(_tileUnit);
                copy.transform.SetParent(transform);

                Sprite sprite = GetSprite(map[x, y]);

                TempTileModel model = new TempTileModel()
                {
                    xPos = x,
                    yPos = y,
                    spriteName = sprite.name
                };

                string name = string.Format("{0}|{1}", x, y);
                copy.name = name;

                var unit = copy.GetComponent<TileUnit>();
                unit.SetModel(model);
                unit.spriteRenderer.sprite = sprite;

                var list = GetVertical(x);
                list.Add(unit);

                worldMap[x, y] = map[x, y];
            }
        }

        UpdatePosition(map);

        AstarCalculator.Instance.Init(dic, w, h);
    }

    public int GetDepth(Vector3 globalPosition)
    {
        TileUnit tileUnit = GetTile(globalPosition);
        if (tileUnit == null) {
            return -1;
        }
        return worldMap[tileUnit.x, tileUnit.y];
    }

    public int GetDepth(int x, int y) {
        return worldMap[x, y];
    }

    public TileUnit GetTile(int x, int y) {
        if (x < 0 || x >= Width) return null;
        if (y < 0 || y >= Height) return null;

        return dic[x][y];
    }

    public TileUnit GetTile(Vector3 globalPosition) {
        TileUnit output = null;

        pivot.transform.position = new Vector3(globalPosition.x, globalPosition.y, pivot.transform.position.z);

        Collider2D[] cs = Physics2D.OverlapPointAll(pivot.transform.position);
        
        if (cs != null && cs.Length > 0)
        {
            Collider2D tile = null;
            foreach (var h in cs) {
                if (h.tag == "Tile") {
                    tile = h;
                    break;
                }
            }
            if (tile) {
                TileUnit unit = tile.transform.parent.GetComponent<TileUnit>();

                output = unit;
            }
        }

        return output;
    }
    
    private void Update()
    {

    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(RandomMapGenerator)), CanEditMultipleObjects]
//public class MapGeneratorEditor : Editor
//{
//    private SerializedObject m_object;
//    RandomMapGenerator Generator { get { return target as RandomMapGenerator; } }

//    private void OnEnable()
//    {
//        m_object = new SerializedObject(targets);
//    }

//    public override void OnInspectorGUI()
//    {
//        m_object.Update();
//        DrawDefaultInspector();

//        Generator.Width = EditorGUILayout.IntField("Width", Generator.Width);
//        Generator.Height = EditorGUILayout.IntField("Width", Generator.Height);
        
//        if (GUILayout.Button("Generate"))
//        {
//            Generator.GenerateMap();
//        }

//        if (GUILayout.Button("Clear")) {
//            Generator.ClearMap();
//        }
        
//    }


//}
//#endif