﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTestCharacter : MonoBehaviour {
    
    static long[] _etcs = { 70000 , 70001, 70002 };
    static long[] _equip = { 10000, 20000, 30000, 40000, 50000, 60000};
    static long[] _equip2 = { 10001, 20000, 30001,40000, 50000, 60000};
    public GameObject PlayerCharacter;

    public CharacterModel CharacterUnit;

    public GameObject UIController;

    public GameObject Canvas;

    public void Awake()
    {
    }

    public void Start()
    {
        if (PlayerCharacter == null) {
            PlayerCharacter = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
        }
        CharacterUnit = PlayerCharacter.GetComponent<CharacterModel>();
        CharacterUnit.initialState();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)){
            WearAllEquipType();
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            RemoveAllEquipType();
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
        if (Input.GetKeyDown(KeyCode.F12))
        {
            OpenCanvas();
        }
    }

    void OpenCanvas()
    {
        Canvas.SetActive(true);
    }

    void WearAllEquipType()
    {
        foreach(var item in _equip)
        {
            ItemModel weapon = ItemManager.Manager.MakeItem(item);
            CharacterUnit.WearEquipment(weapon);
        }
    }

    void RemoveAllEquipType()
    {
        CharacterUnit.RemoveEquipment("weapon");
        CharacterUnit.RemoveEquipment("head");
        CharacterUnit.RemoveEquipment("util1");
        CharacterUnit.RemoveEquipment("util2");
        CharacterUnit.RemoveEquipment("util3");
    }


    void RemoveWeapon()
    {
        CharacterUnit.RemoveEquipment("weapon");
    }

    void RemoveHead()
    {
        CharacterUnit.RemoveEquipment("head");
    }

    void EatFood()
    {
        ItemModel food = ItemManager.Manager.MakeItem(70000);
        CharacterUnit.UseExpendables(food);
    }
  
    void EatWater()
    {
        ItemModel water = ItemManager.Manager.MakeItem(70001);
        CharacterUnit.UseExpendables(water);
    }

    void EquipDefaultItems()
    {
        foreach (long id in _etcs)
        {
            ItemManager.Manager.AddItem(CharacterUnit, id, 10);
        }
 //       CharacterUnit.PrintItemsInItems();
    }

    void GetEquip()
    {
        foreach (long id in _equip2)
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
