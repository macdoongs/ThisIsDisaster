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

#if MIDDLE_PRES
        public int[] shelterItems = new int[] {
            41001,
            41002,
            10002,
            17,
            20003,
            31001,
            12,
            8,
            1,
            30002,
            30010,
            33002,
            40002
        };

        List<TileUnit> tiles = new List<TileUnit>();
#endif

        public List<ShelterUnit> _shelters = new List<ShelterUnit>();
        public GameObject shelterObject;
        public GameObject ShelterTile;
        public Sprite shelterTileSprite;

        Dictionary<ShelterUnit, GameObject> _accesses = new Dictionary<ShelterUnit, GameObject>();
        public GameObject shelterAccessObject;

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

                    if (x > model.Width / 2 - 5 && x < model.Width / 2 + 5) {
                        if (y > model.Height / 2 - 5 && y < model.Height / 2 + 5) {
                            tiles.Add(tileUnit);
                        }
                    }
                }
            }

            //calculate tile coord

            ShelterUnit unit = loaded.GetComponent<ShelterUnit>();

            var shelter = MakeShelterAccess(model.AccessTile);
            _accesses.Add(unit, shelter);

            model.SetUnit(unit);
            unit.SetModel(model);

            MakePresDropItems();

            return unit;
        }

#if MIDDLE_PRES
        public void MakePresDropItems() {
            
            for (int i = 0; i < shelterItems.Length; i++) {
                if (tiles.Count > shelterItems.Length - i) {
                    TileUnit tile = tiles[UnityEngine.Random.Range(0, tiles.Count)];
                    tiles.Remove(tile);

                    ItemManager.Manager.MakeDropItem(shelterItems[i], tile);
                }
            }
        }
#endif

        public GameObject MakeShelterAccess(TileUnit tile) {
            GameObject copy = Instantiate(shelterAccessObject);
            copy.transform.SetParent(transform);
            copy.transform.localScale = Vector3.one;
            copy.transform.position = tile.transform.position;

            var tileSetter = copy.GetComponent<AutoTileMovementSetter>();
            tileSetter.SetCurrentTileForcely(tile);
            return copy;
        }
    }
}