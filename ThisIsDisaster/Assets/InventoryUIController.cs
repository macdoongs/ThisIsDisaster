using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour {


    //인벤토리 아이템 슬롯 이미지
    public Image[] ItemSlots = new Image[30];
    //인벤토리 아이템 개수 텍스트
    public Text[] ItemCounts = new Text[30];

    //인벤토리 착용장비 슬롯 이미지
    public Image[] PreviewSlots = new Image[5];

    //인벤토리 착용장비 기본 이미지셋 
    public Sprite[] defaultSlotSprite = new Sprite[5];

    //스프라이트 소스
    string[] spriteSrcs;

    //아이템 슬롯 스프라이트 추가.
    public void SlotSprite(GameObject PlayerCharacter)
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

    //인벤토리 슬롯 스프라이트 초기화
    public void initialSprite()
    {
        for (int i = 0; i < 30; i++)
        {
            ItemSlots[i].sprite = null;
            ItemSlots[i].color = Color.clear;
            ItemCounts[i].text = "";
        }
    }

    //인벤토리 좌측 프리뷰 슬롯 스프라이트 할당
    public void SetPreviewSprite(GameObject PlayerCharacter)
    {
        //착용중인 장비 슬롯
        ItemModel head = PlayerCharacter.GetComponent<CharacterModel>().headSlot;
        ItemModel weapon = PlayerCharacter.GetComponent<CharacterModel>().weaponSlot;
        ItemModel util1 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot1;
        ItemModel util2 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot2;
        ItemModel util3 = PlayerCharacter.GetComponent<CharacterModel>().utilSlot3;
        ItemModel[] slots = new ItemModel[] { head, weapon, util1, util2, util3 };

        for (int i = 0; i < 5; i++)
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
