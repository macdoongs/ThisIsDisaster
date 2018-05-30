using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shelter
{
    public class ShelterManager
    {
        private const int PositionInterval = 100;
        private static ShelterManager _instance = null;
        public static ShelterManager Instance {
            get {
                if (_instance == null) {
                    _instance = new ShelterManager();
                }
                return _instance;
            }
        }

        private int _instId = 0;

        private List<ShelterModel> _shelters = new List<ShelterModel>();

        public void MakeRandomShelter(TileUnit accessPoint) {
            ShelterModel newShelter = new ShelterModel(_instId++, _instId * PositionInterval);
            newShelter.Width = UnityEngine.Random.Range(20, 40);
            newShelter.Height = UnityEngine.Random.Range(20, 40);
            newShelter.AccessTile = accessPoint;
            accessPoint.SetEnterAction(newShelter.AccessShelter);
            //access point make as ShelterAccess
            accessPoint.spriteRenderer.color = Color.gray;

            //Make AccessPoint Door

            ShelterLayer.CurrentLayer.MakeShelter(newShelter);
            _shelters.Add(newShelter);

#if MIDDLE_PRES
            Notice.Instance.Send(NoticeName.AddShelter, newShelter);
#endif
        }
    }
}