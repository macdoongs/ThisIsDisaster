using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTestCharacter : MonoBehaviour {
    //static long[] _defaultEquipments = { 10000, 20000, 30000, 30001, 30002, 40000, 40001, 40002 };
    static long[] _defaultEquipments = { 40000 };
    static long[] _equip = { 10000};
    public GameObject PlayerCharacter;

    public CharacterModel CharacterUnit;

    public GameObject UIController;

    

    public void Awake()
    {
        CharacterUnit = PlayerCharacter.GetComponent<CharacterModel>();
        CharacterUnit.initialState();
    }

    public void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)){
            WearWeapon();
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            RemoveWeapon();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            WearHead();
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            RemoveHead();
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            CharacterUnit.SubtractHealth(20);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            EatFood();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            CharacterUnit.SubtractStamina(20);
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            EatWater();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            EquipDefaultItems();
        }
        
        if (Input.GetKeyDown(KeyCode.F10))
        {
            GetEquip();
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            RemoveFirstItem();
        }
    }
    void WearWeapon()
    {
        ItemModel weapon = ItemManager.Manager.MakeItem(20000);
        CharacterUnit.WearEquipment(weapon);
    }

    void RemoveWeapon()
    {
        CharacterUnit.RemoveEquipment(CharacterUnit.weaponSlot);
    }

    void WearHead()
    {
        ItemModel head = ItemManager.Manager.MakeItem(10000);
        CharacterUnit.WearEquipment(head);
    }

    void RemoveHead()
    {
        CharacterUnit.RemoveEquipment(CharacterUnit.headSlot);
    }

    void EatFood()
    {
        ItemModel food = ItemManager.Manager.MakeItem(40000);
        CharacterUnit.UseExpendables(food);
    }
  
    void EatWater()
    {
        ItemModel water = ItemManager.Manager.MakeItem(40001);
        CharacterUnit.UseExpendables(water);
    }

    void EquipDefaultItems()
    {
        foreach (long id in _defaultEquipments)
        {
            ItemManager.Manager.AddItem(CharacterUnit, id, 10);
        }
 //       CharacterUnit.PrintItemsInItems();
    }

    void GetEquip()
    {
        foreach (long id in _equip)
        {
            ItemManager.Manager.AddItem(CharacterUnit, id, 1);
        }
    //    CharacterUnit.PrintItemsInItems();
        
    }

    void RemoveFirstItem()
    {
        CharacterUnit.RemoveItemAtIndex(0);
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
