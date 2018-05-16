using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarCalculator {
    public class PathInfo {
        public TileUnit Origin = null;
        public TileUnit Destination = null;
        public List<TileUnit> path = null;
    }

    public class Calculation
    {
        public class OpenTile {
            public TileUnit tile;
            public int Cost_F = 0;
            public int Cost_G = 0;
            public int Total { get { return Cost_F + Cost_G; } }
            
        }
        const int INDEX_MAX = 3;
        const int DIAGONALCOST = 14;
        const int NORMALCOST = 10;

        static int[] _indexer = { -1, 0, 1 };
        public Dictionary<int, OpenTile> opened = new Dictionary<int, OpenTile>();
        public Dictionary<int, TileUnit> closed = new Dictionary<int, TileUnit>();

        public TileUnit Origin = null;
        public TileUnit Destination = null;

        public int MinimalHeight = 0;

        /*
         F = G + H;
         F - 총 비용 (_totalCost)
         G - 시작점에서 지금까지의 비용 (_currentCost)
         H - 현재 위치에서 도착점까지의 비용 (_destCost)
        */
        int _totalCost = 0;
        int _currentCost = 0;
        int _destCost = 0;

        List<TileUnit> selectedAsPath = new List<TileUnit>();

        public List<TileUnit> CalculateAstar(TileUnit origin, TileUnit dest) {
            Origin = origin;
            Destination = dest;

            CalculateTile(0, origin);

            //result is selectedAsPath
            return selectedAsPath;
        }

        OpenTile GetOpenTile(int hash) {
            return opened[hash];
        }

        void CalculateTile(int fCost, TileUnit current) {

            while (true) {



                break;
            }

            int currentX = current.x;
            int currentY = current.y;

            int curHash = GetTileIndex(current);
            closed.Add(curHash, current);
            opened.Remove(curHash);

            selectedAsPath.Add(current);
            
            for (int i = 0; i < INDEX_MAX; i++)
            {
                int index_x = _indexer[i];
                int x = currentX + index_x;

                for (int j = 0; j < INDEX_MAX; j++) {
                    int index_y = _indexer[j];
                    int cost = GetDistValue(index_x, index_y);
                    if (cost == 0) continue;//본인제외

                    int y = currentY + index_y;

                    TileUnit tile = Instance.GetTile(x, y);
                    
                    if (tile == null) continue;
                    if (tile == Destination) {
                        //arrived
                        selectedAsPath.Add(Destination);
                        return;
                    }

                    int tileHash = GetTileIndex(tile);
                    if (closed.ContainsKey(tileHash)) continue;
                    if (opened.ContainsKey(tileHash)) continue;

                    if (tile.HeightLevel <= MinimalHeight) {
                        closed.Add(tileHash, tile);
                        continue;
                    }

                    int current_F_cost = fCost + cost;
                    int current_G_cost = GetHeuristicCost(tile);

                    OpenTile open = new OpenTile() {
                        tile = tile,
                        Cost_F = current_F_cost,
                        Cost_G = current_G_cost
                    };

                    opened.Add(tileHash, open);
                }
            }
            Debug.Log(opened.Count);

            int selected = 0;
            int minCost = int.MaxValue;
            foreach (var v in opened) {
                int cost = v.Value.Total;
                if (cost < minCost) {
                    selected = v.Key;
                    minCost = cost;
                }
            }

            OpenTile select = opened[selected];
            CalculateTile(select.Cost_F, select.tile);
        }

        static int GetDistValue(int i, int j) {
            if (i == 0 && i == j)
            {
                return 0;
            }
            int value = Mathf.Abs(i) + Mathf.Abs(j);
            if (value == 1) return NORMALCOST;
            if (value == 2) return DIAGONALCOST;
            return 0;
        }

        /// <summary>
        /// 맨하탄
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        int GetHeuristicCost(TileUnit current) {
            return (Mathf.Abs(current.x - Destination.x) + Mathf.Abs(current.y - Destination.y)) * NORMALCOST;
        }
    }

    private static AstarCalculator _instance = null;
    public static AstarCalculator Instance {
        get {
            if (_instance == null) {
                _instance = new AstarCalculator();
            }
            return _instance;
        }
    }

    private Dictionary<int, List<TileUnit>> _worldMap = null;
    public int Width = 0;
    public int Height = 0;

    public bool IsEnabled {
        get;
        private set;
    }

    /// <summary>
    /// Call After RandomMapGenerator Generated Map
    /// </summary>
    public void Init(Dictionary<int,List<TileUnit>> map, int width, int height) {
        IsEnabled = true;
        _worldMap = map;
        Width = width;
        Height = height;
    }

    public PathInfo GetDestinationPath(TileUnit origin, TileUnit dest) {
        Calculation calculation = new Calculation();
        var list = calculation.CalculateAstar(origin, dest);
        PathInfo output = new PathInfo
        {
            Origin = origin,
            Destination = dest,
            path = list
        };
        return output;
    }

    public static int GetTileIndex(TileUnit tile) {
        return tile.x * 1000 + tile.y;
    }

    public TileUnit GetTile(int x, int y) {
        if (x < 0 || x >= Width) return null;
        if (y < 0 || y >= Height) return null;
        return _worldMap[x][y];
    }
}
