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

        public void SetModel(ShelterModel model) {
            Model = model;
            UpdateTilePosition();
            Model.MakeExitTile();
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
