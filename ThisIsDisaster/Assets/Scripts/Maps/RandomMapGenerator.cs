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
    public static RandomMapGenerator Instance { get; private set; }

    //x, list(y)
    Dictionary<int, List<TileUnit>> dic = new Dictionary<int, List<TileUnit>>();
    [ReadOnly]
    public int Width = 0;
    [ReadOnly]
    public int Height = 0;

    public GameObject _tileUnit;
    public List<Sprite> _randomTileSprites;
    public int roomThresholdSize = 50;
    const char sepMatch = '|';
    const float _xDelta = 0.5f;
    const float _yDelta = 0.25f;
    public float _zDelta = 0.1f;

    

    private void Awake()
    {
        Instance = this;
    }

    public void ClearMap() {
        foreach (var list in dic.Values) {
            foreach (var u in list) {
                DestroyImmediate(u.gameObject);
            }
        }
        dic.Clear();

        foreach (Transform t in transform) {
            DestroyImmediate(t.gameObject);
        }
    }

    public void UpdatePosition()
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

                curTile.transform.localPosition = new Vector3(xPos, yPos, zPos);
            }
            xInitial += _xDelta;
            yInitial += _yDelta;
            zInitial += _zDelta;
        }

        transform.localPosition = new Vector3(-Width * 0.5f + GameStaticInfo.TileWidth_Half, 0f, 0f);
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

    public void GenerateMapByAlogrithm(int[,] map, int w, int h) {
        ClearMap();
        this.Width = w;
        this.Height = h;
        
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
            }
        }

        UpdatePosition();
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