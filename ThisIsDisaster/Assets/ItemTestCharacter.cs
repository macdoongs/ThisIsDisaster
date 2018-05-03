using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTestCharacter : MonoBehaviour {
    static long[] _defaultEquipments = { 10000, 20000, 30000, 30001, 30002, 40000, 40001, 40002 };
    public UnitModel PlayerUnit;

    public void Start()
    {
        PlayerUnit = new UnitModel();


    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) {
            EquipDefaultItems();
        }
    }

    void EquipDefaultItems() {
        foreach (long id in _defaultEquipments) {
            ItemManager.Manager.AddItem(PlayerUnit, id);
        }

        PlayerUnit.PrintAllItems();
        
        MakeEquippedItems();
    }

    void MakeEquippedItems() {
        var items = PlayerUnit.GetAllItems();
        foreach (ItemModel item in items) {
            //Make some Gameobject
            string src = item.metaInfo.spriteSrc;
            if (string.IsNullOrEmpty(src)) continue;
            Sprite s = Resources.Load<Sprite>(src);

            if (s != null) {
                GameObject go = new GameObject(item.metaInfo.ToString());
                go.AddComponent<SpriteRenderer>().sprite = s;
            }
        }
    }
}
