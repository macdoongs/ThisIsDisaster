using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtoType
{
    public enum EnvironmentType {
        Grass,
        Glacier,
        Desert,
    }
    public class PrototypeMapController : MonoBehaviour
    {
        [System.Serializable]
        public class SpriteSet {
            public EnvironmentType type;
            public Sprite WaterLevel;
            public Sprite GroundLevel1;
            public Sprite GroundLevel2;
        }
        const char sepMatch = '|';
        public static PrototypeMapController Controller
        {
            private set;
            get;
        }

        Dictionary<int, List<TileUnit>> map = null;
        int _xSize = 0;
        int _ySize = 0;

        #region Inspector
        public Transform mapRoot;
        public Sprite waterReference;
        public Sprite groundLevel1;
        public Sprite groundLevel2;

        public List<SpriteSet> spriteSets;
        public EnvironmentType currentType;
        #endregion

        void Awake()
        {
            if (Controller != null)
            {
                Destroy(Controller.gameObject);
            }
            Controller = this;
        }

        // Use this for initialization
        void Start()
        {
            ReadMap();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ApplyEnviroment() {
            SpriteSet currentSet = spriteSets[(int)currentType];

            for (int i = 0; i < _xSize; i++) {
                for (int j = 0; j < _ySize; j++) {
                    var current = map[i][j];
                    if (current.type == TileType.GROUND_UNWALKABLE)
                    {
                        current.spriteRenderer.sprite = currentSet.GroundLevel1;
                    }
                    else if (current.type == TileType.GROUND_WALKABLE)
                    {
                        current.spriteRenderer.sprite = currentSet.GroundLevel2;
                    }
                    else {
                        current.spriteRenderer.sprite = currentSet.WaterLevel;
                    }
                }
            }
        }

        void ReadMap()
        {
            
            int xSize = 0;
            int ySize = 0;
            Dictionary<int, List<TileUnit>> invertData = new Dictionary<int, List<TileUnit>>();
            foreach (Transform tileTransform in mapRoot) {
                TileUnit tile = tileTransform.GetComponent<TileUnit>();
                string tileName = tile.gameObject.name;
                string[] splitted = tileName.Split(sepMatch);

                int x = 0, y = 0;
                if (!int.TryParse(splitted[0].Trim(), out x)) continue;
                if (!int.TryParse(splitted[1].Trim(), out y)) continue;

                if (tile.spriteRenderer.sprite == waterReference)
                {
                    tile.type = TileType.WATER;
                }
                else {
                    tile.type = TileType.GROUND_UNWALKABLE;
                    var local = tile.transform.localPosition;
                    //local.y += 0.25f;
                    tile.transform.localPosition = local;
                }

                List<TileUnit> vertical = null;
                if (!invertData.TryGetValue(x, out vertical)) {
                    vertical = new List<TileUnit>();
                    invertData.Add(x, vertical);
                }
                vertical.Add(tile);

                if (x > xSize) {
                    xSize = x;
                }

                if (y > ySize) {
                    ySize = y;
                }
            }

            _xSize = ++xSize;
            _ySize = ++ySize;

            map = invertData;

            for (int xInd = 0; xInd < xSize; xInd++) {
                for (int yInd = 0; yInd < ySize; yInd++) {
                    TileUnit current = map[xInd][yInd];
                    if (current.type == TileType.WATER) continue;
                    ReadIsNearWater(current, xInd, yInd);

                    if (!current.isNearWater) {
                        current.type = TileType.GROUND_WALKABLE;
                        current.spriteRenderer.sprite = groundLevel2;
                        var local = current.transform.localPosition;
                        //local.y += 0.25f;
                        current.transform.localPosition = local;
                    }
                }
            }

            ApplyEnviroment();
        }

        void ReadIsNearWater(TileUnit current, int x, int y) {
            
            List<int> read_x = new List<int>();
            List<int> read_y = new List<int>();
            if (x != 0) {
                read_x.Add(x - 1);
            }
            read_x.Add(x);

            if (x != _xSize - 1) {
                read_x.Add(x + 1);
            }

            if (y != 0) {
                read_y.Add(y - 1);
            }
            read_y.Add(y);
            if (y != _ySize - 1) {
                read_y.Add(y + 1);
            }

            for (int i = 0; i < read_x.Count; i++) {
                for (int j = 0; j < read_y.Count; j++) {
                    int xPos = read_x[i];
                    int yPos = read_y[j];
                    if (xPos == x && yPos == y) continue;

                    TileUnit targetTile = map[xPos][yPos];
                    if (targetTile.type == TileType.WATER) {
                        current.isNearWater = true;
                        return;
                    }
                }

            }

        }
    }
}