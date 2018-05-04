using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTestCharacter : MonoBehaviour {
    static long[] _defaultEquipments = { 10000, 20000, 30000, 30001, 30002, 40000, 40001, 40002 };

    public CharacterModel CharacterUnit;
    
    public void Start()
    {
        CharacterUnit = new CharacterModel();
        CharacterUnit.initialState();
    }

    public void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.F2)) {
            EquipDefaultItems();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            
            UpdateCharacterStat();

        }

        if (Input.GetKeyDown(KeyCode.F3))
        {

            WearHead();
        }


        if (Input.GetKeyDown(KeyCode.F4))
        {
            CharacterUnit.PrintStats();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CharacterUnit.WoundHealth(20);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            CharacterUnit.HealHealth(20);
        }

    }

    void UpdateCharacterStat()
    {
        
        UnityEngine.Debug.Log("Character Stat");
        UnityEngine.Debug.Log( CharacterUnit.health );
        
    }

    void WearHead()
    {
        ItemModel head = ItemManager.Manager.MakeItem(10000);

        CharacterUnit.WearEquipment(head);
       // CharacterUnit.AddStats(head);

    }

    void EquipDefaultItems() {
        foreach (long id in _defaultEquipments) {
            ItemManager.Manager.AddItem(CharacterUnit, id);
        }

        CharacterUnit.PrintAllItems();
        
    }


    void MakeEquippedItems() {
        var items = CharacterUnit.GetAllItems();
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
