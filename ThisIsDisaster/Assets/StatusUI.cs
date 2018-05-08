using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public Image[] ItemSlots = new Image[30];
    public Text[] ItemCounts = new Text[30];

    public GameObject PlayerCharacter;

    public Text[] StatusPannel;
    public Text Health;
    public Text Stamina;
    public Text Defense;
    public Text Damage;

    public Image[] PreviewSlots = new Image[5];
    public Sprite[] defaultSlotSprite = new Sprite[5];
    string[] spriteSrcs;

    public void Start()
    {
        StatusPannel = new Text[] { Health, Stamina, Defense, Damage };
    }


    public void Update()
    {
        GetStatus();
        SlotSprite();
        SetPreviewSprite();
        //      ItemCount();    
    }

    public void GetStatus()
    {
        string _maxHealth = PlayerCharacter.GetComponent<CharacterModel>().maxHealth.ToString();
        string _health = PlayerCharacter.GetComponent<CharacterModel>().health.ToString();
        string _healthInUI = _health + " / " + _maxHealth;

        string _maxStamina = PlayerCharacter.GetComponent<CharacterModel>().maxStamina.ToString();
        string _stamina = PlayerCharacter.GetComponent<CharacterModel>().stamina.ToString();
        string _staminaInUI = _stamina + " / " + _maxStamina;


        Health.GetComponent<Text>().text = _healthInUI;
        Stamina.GetComponent<Text>().text = _staminaInUI;
        Damage.GetComponent<Text>().text =
                PlayerCharacter.GetComponent<CharacterModel>().damage.ToString();
        Defense.GetComponent<Text>().text =
            PlayerCharacter.GetComponent<CharacterModel>().defense.ToString();
    }

    public void SlotSprite()
    {
        initialSprite();
        var items = PlayerCharacter.GetComponent<CharacterModel>().GetAllItems();
        var counts = PlayerCharacter.GetComponent<CharacterModel>().GetAllCounts();

        int index = 0;

        for (index = 0; index < items.Count; index++)
        {
            string src = items[index].metaInfo.spriteSrc;
            if (string.IsNullOrEmpty(src)) continue;
            Sprite s = Resources.Load<Sprite>(src);

            if (s != null)
            {
                ItemSlots[index].sprite = s;
                ItemSlots[index].color = Color.white;
                ItemCounts[index].text = counts[index].ToString();
            }
        }
    }

    public void initialSprite()
    {
        for (int i = 0; i < 30; i++)
        {
            ItemSlots[i].sprite = null;
            ItemSlots[i].color = Color.clear;
            ItemCounts[i].text = "";
        }
    }

    public void SetPreviewSprite()
    {
        
        ItemModel head = PlayerCharacter.GetComponent<CharacterModel>().headSlot;
        ItemModel weapon = PlayerCharacter.GetComponent<CharacterModel>().weaponSlot;
        ItemModel util1 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot1;
        ItemModel util2 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot2;
        ItemModel util3 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot3;
        ItemModel[] slots = new ItemModel[] {head, weapon, util1, util2, util3 };

        for(int i = 0; i < 5; i++)        
        {
            if (slots[i] != null)
            {
                string src = slots[i].metaInfo.spriteSrc;
                if (string.IsNullOrEmpty(src)) continue;
                Sprite s = Resources.Load<Sprite>(src);

                if (s != null)
                {
                    PreviewSlots[i].sprite = s;
                    PreviewSlots[i].color = Color.white;
                }
            }
            else
            {
                PreviewSlots[i].sprite = defaultSlotSprite[i];
            }

        }



    }
}


