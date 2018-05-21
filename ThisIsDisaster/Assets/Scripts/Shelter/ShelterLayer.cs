using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shelter
{
    public class ShelterLayer : MonoBehaviour
    {
        public static ShelterLayer CurrentLayer
        {
            private set;
            get;
        }

        public List<ShelterUnit> _shelters = new List<ShelterUnit>();
        public GameObject shelterObject;
        public GameObject ShelterTile;
        public Sprite shelterTileSprite;

        private void Awake()
        {
            CurrentLayer = this;
        }

        public ShelterUnit MakeShelter(ShelterModel model)
        {
            GameObject loaded = Instantiate(shelterObject);
            loaded.transform.SetParent(transform);
            loaded.transform.localPosition = new Vector3(model.PositionDelta, 0, 0);
            loaded.transform.localRotation = Quaternion.Euler(Vector3.zero);
            loaded.transform.localScale = Vector3.one;

            for (int x = 0; x < model.Width; x++) {
                for (int y = 0; y < model.Height; y++) {
                    GameObject copy = Instantiate(ShelterTile);
                    copy.transform.SetParent(loaded.transform);

                    TempTileModel tileModel = new TempTileModel() {
                        xPos = x + model.PositionDelta,
                        yPos = y,
                        spriteName = shelterTileSprite.name,
                    };
                    string name = string.Format("{2}[{0}|{1}]", x, y, "Shelter");
                    copy.name = name;
                    TileUnit tileUnit = copy.GetComponent<TileUnit>();
                    tileUnit.SetModel(tileModel);
                    tileUnit.spriteRenderer.sprite = shelterTileSprite;
                    model.AddTile(tileUnit, x, y);
                }
            }

            //calculate tile coord

            ShelterUnit unit = loaded.GetComponent<ShelterUnit>();

            model.SetUnit(unit);
            unit.SetModel(model);
            
            return unit;
        }
    }
}