using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shelter
{
    public class ShelterUnit : MonoBehaviour
    {
        public ShelterModel Model {
            get;
            private set;
        }

        public Transform pivot;

        public void SetModel(ShelterModel model) {
            Model = model;
            UpdateTilePosition();
            Model.MakeExitTile();
        }

        public TileUnit GetTile(Vector3 globalPosition) {
            TileUnit output = null;
            pivot.transform.position = new Vector3(globalPosition.x, globalPosition.y, pivot.transform.position.z);
            Collider2D[] cs = Physics2D.OverlapPointAll(pivot.transform.position);
            if (cs != null && cs.Length > 0) {
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
        
        public void UpdateTilePosition() {
            float xInitial = 0f;
            float yInitial = 0f;
            float zInitial = 0f;
            for (int x = 0; x < Model.Width; x++) {
                List<TileUnit> list = Model.GetVertical(x);

                for (int y = 0; y < Model.Height; y++) {
                    TileUnit curTile = list[y];
                    float xPos = 0f;
                    float yPos = 0f;
                    float zPos = 0f;

                    xPos = xInitial + y * RandomMapGenerator._xDelta;
                    yPos = yInitial - y * RandomMapGenerator._yDelta;
                    zPos = zInitial - y * RandomMapGenerator._zDelta;

                    curTile.SetPosition(new Vector3(xPos, yPos, zPos));
                    curTile.SetCoord(x + Model.PositionDelta, y);
                    curTile.SetHeight(1);


                }
                xInitial += RandomMapGenerator._xDelta;
                yInitial += RandomMapGenerator._yDelta;
                zInitial += RandomMapGenerator._zDelta;
            }
        }
    }
}
