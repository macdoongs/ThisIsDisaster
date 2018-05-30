using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMapGenerator : MonoBehaviour {

    public const float _xDelta = 0.5f;
    public const float _yDelta = 0.25f;
    public const float _zDelta = 0.1f;

    public int Width = 20;
    public int Height = 20;
    public GameObject _tileUnit;
    public Transform pivot;
    public List<Sprite> sprites = new List<Sprite>();
    public int[,] worldMap = null;

    Dictionary<int, List<LobbyTile>> dic = new Dictionary<int, List<LobbyTile>>();
    Dictionary<int, List<LobbyTile>> _levelDic = new Dictionary<int, List<LobbyTile>>();
    public int Anchor = 4;

    public void MakeField() {
        ClearMap();
        worldMap = new int[Width, Height];
        for (int x = 0; x < Width; x++)
        {

            for (int y = 0; y < Height; y++)
            {
                int height = 1;
                if (x < Anchor || x > Width-1 - Anchor)
                {
                    height = 0;
                }
                if (y < Anchor || y > Height-1 - Anchor)
                {
                    height = 0;
                }

                GameObject copy = Instantiate(_tileUnit);
                copy.transform.SetParent(transform);

                Sprite sprite = GetSprite(height);

                TempTileModel model = new TempTileModel()
                {
                    xPos = x,
                    yPos = y,
                    spriteName = sprite.name
                };

                string name = string.Format("{0}|{1}", x, y);
                copy.name = name;

                var unit = copy.GetComponent<LobbyTile>();
                unit.SetModel(model);
                unit.spriteRenderer.sprite = sprite;

                var list = GetVertical(x);
                list.Add(unit);

                worldMap[x, y] = height;
            }
        }
        UpdateFieldPosition();
    }

    List<LobbyTile> GetVertical(int x)
    {
        List<LobbyTile> output = null;
        if (dic.TryGetValue(x, out output))
        {
            return output;
        }
        output = new List<LobbyTile>();
        dic.Add(x, output);
        return output;
    }


    public void ClearMap() {
        foreach (var list in dic.Values)
        {
            foreach (var u in list)
            {
                if (u.gameObject == pivot.gameObject) continue;
                DestroyImmediate(u.gameObject);
            }
        }
        dic.Clear();
        _levelDic.Clear();

        foreach (Transform t in transform)
        {
            if (t.gameObject == pivot.gameObject) continue;
            DestroyImmediate(t.gameObject);
        }
    }
    
    Sprite GetSprite(int heightLevel)
    {
        return sprites[heightLevel];
    }

    private void Start()
    {
        MakeField();
    }

    void UpdateFieldPosition()
    {
        float xInitial = 0f;
        float yInitial = 0f;
        float zInitial = 0f;

        for (int xInd = 0; xInd < Width; xInd++)
        {
            List<LobbyTile> list = GetVertical(xInd);

            for (int yInd = 0; yInd < Height; yInd++)
            {
                LobbyTile curTile = list[yInd];
                float xPos = 0f;
                float yPos = 0f;
                float zPos = 0f;

                xPos = xInitial + yInd * _xDelta;
                yPos = yInitial - yInd * _yDelta;
                zPos = zInitial - yInd * _zDelta;

                curTile.SetPosition(new Vector3(xPos, yPos, zPos));
                curTile.SetCoord(xInd, yInd);
                curTile.SetHeight(worldMap[xInd, yInd]);

                List<LobbyTile> levelList = null;
                if (!_levelDic.TryGetValue(curTile.HeightLevel, out levelList))
                {
                    levelList = new List<LobbyTile>();
                    _levelDic.Add(curTile.HeightLevel, levelList);
                }
                levelList.Add(curTile);
            }
            xInitial += _xDelta;
            yInitial += _yDelta;
            zInitial += _zDelta;
        }
        transform.localPosition = new Vector3(-Width * 0.5f + GameStaticInfo.TileWidth_Half, 0f, 10f);
    }
}
