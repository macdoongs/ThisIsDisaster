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
        public class Node {
            public TileUnit tile;
            public TileUnit parent;

            /// <summary>
            /// To Destination, Heuristic
            /// </summary>
            public int Cost_H = 0;

            /// <summary>
            /// From Origin
            /// </summary>
            public int Cost_O = 0;

            /// <summary>
            /// F = O + H
            /// </summary>
            public int Cost_F { get { return Cost_H + Cost_O; } }
        }
        const int INDEX_MAX = 3;
        const int DIAGONALCOST = 14;
        const int NORMALCOST = 10;

        static int[] _indexer = { -1, 0, 1 };
        public Dictionary<int, Node> opened = new Dictionary<int, Node>();
        public Dictionary<int, TileUnit> closed = new Dictionary<int, TileUnit>();

        public TileUnit Origin = null;
        public TileUnit Destination = null;

        public int MinimalHeight = 0;

        /*
         F = O + H;
         F - 총 비용 (_totalCost)
         O - 시작점에서 지금까지의 비용 (_currentCost)
         H - 현재 위치에서 도착점까지의 비용 (_destCost)
        */
        int _totalCost = 0;
        int _currentCost = 0;
        int _destCost = 0;

        void CalculateAstar2(TileUnit origin, TileUnit dest) {
            Origin = origin;
            Destination = dest;

        }

        List<TileUnit> selectedAsPath = new List<TileUnit>();

        public List<TileUnit> CalculateAstar(TileUnit origin, TileUnit dest) {
            Origin = origin;
            Destination = dest;

            //CalculateTile(origin, 0);

            //result is selectedAsPath
            return selectedAsPath;
        }

        Node GetOpenTile(int hash) {
            return opened[hash];
        }
        
        void CalculateTile(TileUnit current, int oCost ) {
            int currentX = current.x;
            int currentY = current.y;
            int curIndex = GetTileIndex(current);
            
            for (int i = 0; i < INDEX_MAX; i++) {
                int index_x = _indexer[i];
                int x = current.x + index_x;
                for (int j = 0; j < INDEX_MAX; j++) {
                    if (i == 1 && i == j) continue;//current
                    int index_y = _indexer[j];
                    int y = current.y + index_y;
                    TileUnit tile = Instance.GetTile(x, y);
                    int tileIndex = GetTileIndex(tile);

                    if (closed.ContainsKey(tileIndex)) continue;
                    if (opened.ContainsKey(tileIndex)) continue;

                    if (tile.HeightLevel <= MinimalHeight) {
                        //check movable
                        closed.Add(tileIndex, tile);
                        continue;
                    }

                    int cost_h = GetHeuristicCost(tile);
                    int cost_o = oCost + GetDistValue(index_x, index_y);

                    Node openTile = new Node()
                    {
                        Cost_H = cost_h,
                        Cost_O = cost_o,
                        tile = tile,
                        parent = current
                    };
                    opened.Add(tileIndex, openTile);
                }
            }
            int minimal = int.MaxValue;
            int selectedIndex = -1;
            foreach (var kv in opened) {
                if (kv.Value.Cost_F < minimal) {
                    minimal = kv.Value.Cost_F;
                    selectedIndex = kv.Key;
                }
            }
            Node open = opened[selectedIndex];



            //opened.Remove(selectedIndex);
            //closed.Add(selectedIndex, open.tile);

            //selectedAsPath.Add(open.tile);
            //CalculateTile(open.Cost_F, open.tile);

            /*
            int currentX = current.x;
            int currentY = current.y;

            int curHash = GetTileIndex(current);
            if (!closed.ContainsKey(curHash)) {
                closed.Add(curHash, current);
            }
            OpenTile currentOpen = null;
            if (opened.ContainsKey(curHash)) {
                currentOpen = opened[curHash];
                opened.Remove(curHash);
            }

            selectedAsPath.Add(current);
            List<TileUnit> near = new List<TileUnit>();
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
                    if (tile.HeightLevel <= MinimalHeight) {
                        closed.Add(tileHash, tile);
                        continue;
                    }
                    int current_F_cost = fCost + cost;
                    int current_G_cost = GetHeuristicCost(tile);
                    int total = current_F_cost + current_G_cost;

                    if (opened.ContainsKey(tileHash))
                    {

                    }

                    near.Add(tile);
                    OpenTile open = new OpenTile() {
                        tile = tile,
                        Cost_F = current_F_cost,
                        Cost_G = current_G_cost,
                        parent = current
                    };
                    if (!closed.ContainsKey(tileHash)) {
                        closed.Add(tileHash, tile);
                    }
                    opened.Add(tileHash, open);
                }
            }
            //Debug.Log(opened.Count);

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
            if (!near.Contains(select.tile))
            {
                //remove path recursivly
                if (currentOpen != null) {
                    //RemovePathRecursivly(currentOpen, select);
                }
            }
            CalculateTile(select.Cost_F, select.tile);
            */

        }

        void RemovePathRecursivly(Node start, Node dest) {
            selectedAsPath.Remove(start.tile);
            TileUnit parent = start.parent;
            while (parent != dest.tile) {
                try
                {
                    int ind = GetTileIndex(parent);
                    if (opened.ContainsKey(ind))
                    {
                        Node del = opened[ind];
                        parent = del.parent;
                        selectedAsPath.Remove(del.tile);
                        opened.Remove(ind);
                    }
                    else {
                        break;
                    }
                }
                catch (System.Exception e) {
                    Debug.LogError(e);
                    break;
                }
            }
        }

        /// <summary>
        /// 특정 위치에서 해당 타일까지 인덱스 비교값, 거리를 의미하게 됨
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int GetDistValue(int x, int y) {
            if (x == 0 && x == y)
            {
                return 0;
            }
            int value = Mathf.Abs(x) + Mathf.Abs(y);
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
