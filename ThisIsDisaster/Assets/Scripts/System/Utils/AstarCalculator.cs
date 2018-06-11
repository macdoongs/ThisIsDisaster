using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarCalculator {
    public class PathInfo {
        public TileUnit Origin = null;
        public TileUnit Destination = null;
        public List<TileUnit> path = null;
        public int currentPathIndex = 0;
    }

    public class Calculation
    {
        public class Node {
            public TileUnit tile;
            public Node parent;

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
        public struct Offset {
            public int x;
            public int y;
            public Offset(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        const int INDEX_MAX = 3;
        const int DIAGONALCOST = 14;
        const int NORMALCOST = 10;

        static readonly int[] _indexer = { -1, 0, 1 };
        static readonly Offset[] offsets = new Offset[8] {
            new Offset( -1,  0),
            new Offset(  0,  1),
            new Offset(  0, -1),
            new Offset(  1,  0),
            new Offset( -1,  1),
            new Offset(  1,  1),
            new Offset( -1, -1),
            new Offset(  1, -1)
        };
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
        List<Node> _searchNodeList = new List<Node>();//tree
        List<TileUnit> selectedAsPath = new List<TileUnit>();
        UnitModel _caller = null;

        public Calculation(UnitModel caller)
        {
            _caller = caller;
        }

        public List<TileUnit> CalculateAstar(TileUnit origin, TileUnit dest) {
            
            Origin = origin;
            Destination = dest;

            Node originNode = new Node() {
                tile = origin,
                Cost_H = GetHeuristicCost(origin),
                Cost_O = 0,
                parent = null
            };
            try
            {
                CalculateNode(originNode);
            }
            catch {
                return new List<TileUnit>();
            }
            Node parent = _searchNodeList[_searchNodeList.Count - 1];
            
            while (parent != null) {
                selectedAsPath.Add(parent.tile);
                parent = parent.parent;
            }

            selectedAsPath.Reverse();
            //result is selectedAsPath
            return selectedAsPath;
        }

        Node GetOpenTile(int hash) {
            return opened[hash];
        }

        void CalculateNode(Node selectedNode) {
            _searchNodeList.Add(selectedNode);
            TileUnit current = selectedNode.tile;
            int currentX = current.x;
            int currentY = current.y;

            int curIndex = GetTileIndex(current);
            if (!closed.ContainsKey(curIndex)) {
                closed.Add(curIndex, current);
            }
            if (opened.ContainsKey(curIndex)) {
                opened.Remove(curIndex);
            }

            //selectedAsPath.Add(current);
            List<TileUnit> near = new List<TileUnit>();
            List<int> xIgnore = new List<int>();
            List<int> yIgnore = new List<int>();
            
            for (int i = 0; i < offsets.Length; i++) {
                Offset offset = offsets[i];

                if (xIgnore.Contains(offset.x)) continue;
                if (yIgnore.Contains(offset.y)) continue;

                int x = currentX + offset.x;
                int y = currentY + offset.y;

                int cost = GetDistValue(offset.x, offset.y);
                if (cost == 0) continue;
                TileUnit tile = Instance.GetTile(x, y);
                if (tile == null) continue;
                if (tile == Destination)
                {
                    Node destNode = new Node()
                    {
                        tile = Destination,
                        Cost_H = 0,
                        Cost_O = 0,
                        parent = selectedNode
                    };
                    _searchNodeList.Add(destNode);
                    return;
                }

                int tileHash = GetTileIndex(tile);
                if (closed.ContainsKey(tileHash)) continue;
                if (!tile.IsPassable(_caller))
                {
                    closed.Add(tileHash, tile);

                    if (i < 4) {
                        if (offset.x != 0) {
                            if (!xIgnore.Contains(offset.x)) {
                                xIgnore.Add(offset.x);
                            }
                        }
                        if (offset.y != 0) {
                            if (!yIgnore.Contains(offset.y)) {
                                yIgnore.Add(offset.y);
                            }
                        }
                    }

                    continue;
                }
                int current_O_cost = selectedNode.Cost_O + cost;
                int current_H_cost = GetHeuristicCost(tile);

                near.Add(tile);

                if (opened.ContainsKey(tileHash))
                {
                    var node = opened[tileHash];
                    if (node.Cost_O > current_O_cost)
                    {
                        node.Cost_O = current_O_cost;
                    }
                    node.parent = selectedNode;
                    continue;
                }

                Node open = new Node()
                {
                    tile = tile,
                    Cost_O = current_O_cost,
                    Cost_H = current_H_cost,
                    parent = selectedNode
                };

                if (!closed.ContainsKey(tileHash))
                {
                    closed.Add(tileHash, tile);
                }
                opened.Add(tileHash, open);
            }
            int selected = 0;
            int minCost = int.MaxValue;
            //현재 위치에서 대각선은 바로 갈 수 있는지 체크해야 할듯?
            foreach (var v in opened) {
                int cost = v.Value.Cost_F;
                if (cost < minCost) {
                    selected = v.Key;
                    minCost = cost;
                }
            }

            Node select = opened[selected];
            CalculateNode(select);
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

    public PathInfo GetDestinationPath(UnitModel model, TileUnit origin, TileUnit dest) {
        if (dest == null) {
            Debug.Log("Dest is null");
            model.MoveControl.StopMovement();
            return null;
        }
        if (dest.IsPassable(model) == false) {

            Debug.Log("Dest cannot passed by " + model.GetUnitName());
            model.MoveControl.StopMovement(); return null;
        }
        Calculation calculation = new Calculation(model);
        var list = calculation.CalculateAstar(origin, dest);
        if (list.Count == 0)
        {
            model.MoveControl.StopMovement();
            return null;
        }
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
