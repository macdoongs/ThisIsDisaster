using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Shelter
{
    public class ShelterModel
    {
        public ShelterUnit Unit {
            get;
            private set;
        }

        public int InstanceId {
            private set;
            get;
        }
        public Dictionary<int, List<TileUnit>> tileMap = new Dictionary<int, List<TileUnit>>();
        public TileUnit AccessTile;
        public TileUnit ExitTile;

        public int PositionDelta = 0;
        public int Width = 10;
        public int Height = 10;

        public ShelterModel(int id, int position) {
            InstanceId = id;
            PositionDelta = position;
        }

        public void SetUnit(ShelterUnit unit) {
            Unit = unit;
        }

        public void AccessShelter(UnitModel target) {
            if (target is PlayerModel) { 
                target.SetCurrentTileForcely(ExitTile);
                target.SetShelter(this);
            }
        }

        public void ExitShelter(UnitModel target) {
            if (target is PlayerModel) { 
                target.SetCurrentTileForcely(AccessTile);
                target.SetShelter(null);
            }
        }

        public void AddTile(TileUnit tile, int x, int y) {
            List<TileUnit> list = GetVertical(x);
            list.Add(tile);
        }

        public List<TileUnit> GetVertical(int x) {
            List<TileUnit> output = null;
            if (!tileMap.TryGetValue(x, out output)) {
                output = new List<TileUnit>();
                tileMap.Add(x, output);
            }
            return output;
        }

        public void MakeExitTile() {
            var list = GetVertical(0);
            ExitTile = list[0];
            ExitTile.spriteRenderer.color = Color.red;
            ExitTile.SetEnterAction(ExitShelter);
        }
    }
    
}
