using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[System.Serializable]
public class TempTileModel {
    public string spriteName;
    public float currentheight;

    public int xPos = 0, yPos = 0;
}

[ExecuteInEditMode]
[AddComponentMenu("Utils/TempMapSetter")]
[System.Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class TempMapSetter : MonoBehaviour {

    //x, list(y)
    Dictionary<int, List<TileUnit>> dic = new Dictionary<int, List<TileUnit>>();

    [ReadOnly]
    public int Width = 0;
    [ReadOnly]
    public int Height = 0;
    public GameObject _tileCollider;
    //Collider Info
    public Vector3 _col_localpos = Vector3.zero;
    public Vector3 _col_localrot = new Vector3(60f, 0f, 45f);
    public Vector3 _col_localscl = new Vector3(0.7f, 0.7f, 0.9f);

    const char sepMatch = '|';
    const float _xDelta = 0.5f;
    const float _yDelta = 0.25f;
    const float _zDelta = 0.1f;

    // Use this for initialization
    void Start () {
        _tileCollider = Resources.Load<GameObject>("Map/TileCubeCollider");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ReadChild()
    {
        dic.Clear();
        Width = Height = -1;

        foreach (Transform t in transform)
        {
            string name = t.gameObject.name;
            var sep = name.Split(sepMatch);

            int left = 0, right = 0;
            left = int.Parse(sep[0]);
            right = int.Parse(sep[1]);

            var unit = t.GetComponent<TileUnit>();
            if (unit == null) {
                unit = t.gameObject.AddComponent<TileUnit>();
            }

            TempTileModel model = new TempTileModel() {
                spriteName = name,
                xPos = left,
                yPos = right
            };

            if (left > Width) {
                Width = left;
            }

            if (right > Height) {
                Height = right;
            }

            unit.SetModel(model);
            var list = GetVertical(left);
            list.Add(unit);
        }

        Width++;
        Height++;
    }

    public void UpdatePosition() {

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
                Debug.Log(string.Format("{0}:{1}", curTile._model.spriteName, curTile.transform.localPosition));

                if (_tileCollider) {
                    GameObject copy = Instantiate(_tileCollider);
                    copy.transform.SetParent(curTile.transform);
                    copy.transform.localPosition = Vector3.zero;
                    copy.transform.localScale = new Vector3(0.7f, 0.7f, 0.9f);
                    copy.transform.localRotation = Quaternion.Euler(60f, 0f, 45f);
                    if (curTile.transform.childCount != 0) {
                        int max = curTile.transform.childCount;
                        for (int i = max - 1; i >= 0; i--) {
                            DestroyImmediate(curTile.transform.GetChild(i).gameObject);
                        }
                    }
                }
            }
            xInitial += _xDelta;
            yInitial += _yDelta;
            zInitial += _zDelta;
        }
        
    }

    List<TileUnit> GetVertical(int x) {
        List<TileUnit> output = null;
        if (dic.TryGetValue(x, out output)) {
            return output;
        }
        output = new List<TileUnit>();
        dic.Add(x, output);
        return output;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TempMapSetter)), CanEditMultipleObjects]
public class TempMapEditor : Editor {
    private SerializedObject m_object;

    TempMapSetter Setter { get { return target as TempMapSetter; } }

    private void OnEnable()
    {
        m_object = new SerializedObject(targets);
    }

    public override void OnInspectorGUI()
    {
        m_object.Update();
        DrawDefaultInspector();

        if (GUILayout.Button("Read")) {
            Setter.ReadChild();
        }

        if (GUILayout.Button("Update")) {
            Setter.UpdatePosition();
        }

    }

    
}
#endif
