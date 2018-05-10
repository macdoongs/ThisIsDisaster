using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour {

    public int bagSize = 30;

    public int typePosition = 0;
    public int itemPosition = 0;
    public int prevPosition = 0;

    //인벤토리 타입 상단 아이템 종류 텍스트
    public Text[] ItemTypes = new Text[5];

    //인벤토리 타입 하단 언더바 이미지
    public GameObject[] ItemTypeUnderbars = new GameObject[5];

    public Color defaultItemTypeColor;
    public Color focusedItemTypeColor;
    public Color typeColor;


    public GameObject Content;


    //인벤토리 아이템 슬롯 이미지
    public Image[] ItemSlots ;
    //인벤토리 아이템 개수 텍스트
    public Text[] ItemCounts ;
    //인벤토리 슬롯 테두리
    public Image[] ItemBorders ;
    //인벤토리 아이템 슬롯 버트
    public Button[] ItemButtons ;

    //인벤토리 착용장비 슬롯 이미지
    public Image[] PreviewSlots = new Image[5];

    //인벤토리 착용장비 기본 이미지셋 
    public Sprite[] defaultSlotSprite = new Sprite[5];

    //스프라이트 소스
    string[] spriteSrcs;

    public GameObject DescriptionPanel;

    public Image DescriptionItemImage;
    public Text DescriptionItemName;
    public Text DescriptionItemDescription;
    public Text[] DescriptionItemStats = new Text[3];
    public Text[] DescriptionItemStatAmount = new Text[3];
    public Text DescriptionItemUseButton ;
    public Text DescriptionItemDeleteButton ;
    public GameObject DescriptionItemRegisterButton;

    public GameObject Player;

    
    public void Start()
    {
        Image[] temp = Content.transform.GetComponentsInChildren<Image>();
        Image[] items = new Image[30];
        for(int i =0; i < temp.Length; i++)
        {
            if(i%4 == 0)
            {
                items[i / 4] = temp[i];
            }
        }

        ItemSlots = new Image[bagSize];
        ItemCounts = new Text[bagSize];
        ItemBorders = new Image[bagSize];
        ItemButtons = new Button[bagSize];

        for (int i =0; i < bagSize; i++)
        {
            ItemSlots[i] = items[i].transform.GetComponentsInChildren<Image>()[1];
            ItemCounts[i] = items[i].transform.GetComponentInChildren<Text>();
            ItemBorders[i] = items[i].transform.GetComponentsInChildren<Image>()[2];
            ItemButtons[i] = items[i].transform.GetComponentInChildren<Button>();
            int index = i;
            ItemButtons[i].onClick.AddListener(() => GetItemPosition(index));
            ItemButtons[i].onClick.AddListener(() => SlotItemClicked(Player));
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
                ItemSlots[index].preserveAspect = true;
                ItemCounts[index].text = counts[index].ToString();
            }
            
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
                    PreviewSlots[i].preserveAspect = true;
                }
            }
            else
            {
                PreviewSlots[i].sprite = defaultSlotSprite[i];
                PreviewSlots[i].preserveAspect = true;
            }

        }
    }

    //인벤토리 상단 아이템 카테고리 선택시
    public void ItemTypeFocused(int index)
    {
        DefaultItemTypes();
        ItemTypes[index].color = focusedItemTypeColor;
        ItemTypeUnderbars[index].SetActive(true);
    }

    public void DefaultItemTypes()
    {
        for (int i = 0; i < ItemTypes.Length; i++)
        {
            ItemTypes[i].color = defaultItemTypeColor;
            ItemTypeUnderbars[i].SetActive(false);
        }
    }

    //인벤토리 상단 아이템 카테고리 초기 상태. InGameUIScript에서 사용
    public void InitialItemTypes()
    {
        DefaultItemTypes();
        ItemTypeFocused(0);
    }


    public void DefaultBorder(GameObject PlayerCharacter)
    {
        int i = 0;
        foreach(ItemModel item in PlayerCharacter.GetComponent<CharacterModel>().ItemLists)
        {
            ItemBorders[i].color = defaultItemTypeColor;
            i++;
        }
    }


    public void GetPosition(int pos)
    {
        typePosition = pos;
    }


    //인벤토리 업데이트. InGameUIScript에서 사용.
    public void InventoryUpdate(GameObject PlayerCharacter)
    {    
        ItemTypeFocused(typePosition);

        DefaultBorder(PlayerCharacter);

        switch (typePosition)
        {           
            case 0:
                break;
            case 1:
                HighlightsType(ItemType.Head, PlayerCharacter);
                break;
            case 2:
                HighlightsType(ItemType.Weapon, PlayerCharacter);
                break;
            case 3:
                HighlightsType(ItemType.Util, PlayerCharacter);
                break;
            case 4:
                HighlightsType(ItemType.Etc, PlayerCharacter);
                break;
        }

    }

    //인벤토리 상단 아이템 카테고리 선택시 해당 아이템 타입 보더 변경
    public void HighlightsType(ItemType type, GameObject PlayerCharacter)
    {
        List<ItemModel> items = PlayerCharacter.GetComponent<CharacterModel>().ItemLists;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].metaInfo.itemType.Equals(type))
            {
                ItemBorders[i].color = typeColor;
            }
        }
    }

    public void PreviewItemClicked(GameObject PlayerCharacter)
    {
        ClearDescription();
        ItemModel item = null;

        ClearDescription();

        switch (prevPosition)
        {
            case 0:
                {
                    try
                    {
                        if(PlayerCharacter.GetComponent<CharacterModel>().headSlot!=null)
                            item = PlayerCharacter.GetComponent<CharacterModel>().headSlot;
                    }
                    catch
                    {
                        return ;
                    }
                    break;
                }
            case 1:
                {
                    try
                    {
                        if (PlayerCharacter.GetComponent<CharacterModel>().weaponSlot != null)
                            item = PlayerCharacter.GetComponent<CharacterModel>().weaponSlot;
                    }
                    catch
                    {
                        return;
                    }
                    break;
                }
            case 2:
                {
                    try
                    {
                        if (PlayerCharacter.GetComponent<CharacterModel>().utilSlot1 != null)
                            item = PlayerCharacter.GetComponent<CharacterModel>().utilSlot1;
                    }
                    catch
                    {
                        return;
                    }
                    break;
                }
            case 3:
                {
                    try
                    {
                        if (PlayerCharacter.GetComponent<CharacterModel>().utilSlot2 != null)
                            item = PlayerCharacter.GetComponent<CharacterModel>().utilSlot2;
                    }
                    catch
                    {
                        return;
                    }
                    break;
                }
            case 4:
                try
                {
                    if (PlayerCharacter.GetComponent<CharacterModel>().utilSlot3 != null)
                        item = PlayerCharacter.GetComponent<CharacterModel>().utilSlot3;
                }
                catch
                {
                    return;
                }
                break;

        }


     if (item != null)
        {
            SetDescriptionPannel(item);

            DescriptionPanel.SetActive(true);
        }
        else
            return ;
    }

    public void SlotItemClicked(GameObject PlayerCharacter)
    {
        DescriptionPanel.SetActive(false);
        ClearDescription();
        ItemDescription(PlayerCharacter);
    }

    public void ItemDescription(GameObject PlayerCharacter)
    {

        ItemModel item = null;


        try
        {
            item = PlayerCharacter.GetComponent<CharacterModel>().
            ItemLists[itemPosition];
        }
        catch
        {
            Debug.Log("Item Slot is Empty");
            DescriptionPanel.SetActive(false);
            return;
        }

        SetDescriptionPannel(item);
    }


    public void SetDescriptionPannel(ItemModel item)
    {

        if (item == null)
        {
            Debug.Log("Item Slot is Empty");
            return;
        }

        if (item.metaInfo.itemType.Equals(ItemType.Etc))
        {
            DescriptionItemUseButton.text = "사용";
            DescriptionItemRegisterButton.SetActive(true);
        }
        else
        {
            DescriptionItemUseButton.text = "착용";
            DescriptionItemRegisterButton.SetActive(false);
        }

        string src = item.metaInfo.spriteSrc;

        Sprite s = Resources.Load<Sprite>(src);

        DescriptionItemImage.sprite = s;
        DescriptionItemImage.color = Color.white;
        DescriptionItemImage.preserveAspect = true;

        DescriptionItemName.text = item.metaInfo.Name;

        Dictionary<string, float> stats = GetItemStats(item);

        int i = 0;
        foreach (var key in stats)
        {
            DescriptionItemStats[i].text = key.Key;
            DescriptionItemStatAmount[i].text = key.Value.ToString();
            i++;
        }

        DescriptionPanel.SetActive(true);
    }

    public void ClearDescription()
    {
        DescriptionPanel.SetActive(false);
        DescriptionItemImage.sprite = null;
        DescriptionItemImage.color = Color.clear;

        DescriptionItemName.text = "";

        for (int i = 0; i<3; i++)
        {
            DescriptionItemStats[i].text = "";
            DescriptionItemStatAmount[i].text = "";
        }

    }

    public Dictionary<string , float> GetItemStats(ItemModel item)
    {
        // 순서 : HP , Stamina , Damage , Defense 

        Dictionary<string, float> result = new Dictionary<string, float>();

        if(item.GetHealth() != 0f)
        {
            result.Add("health", item.GetHealth());
        }

        if (item.GetStamina() != 0f)
        {
            result.Add("stamina", item.GetStamina());
        }
        if (item.GetDamage() != 0f)
        {
            result.Add("damage", item.GetDamage());
        }
        if (item.GetDefense() != 0f)
        {
            result.Add("defense", item.GetDefense());
        }
        return result;
    }

    public void RemoveItemSlot(string slotName)
    {
        Player.GetComponent<CharacterModel>().RemoveEquipment(slotName);
    }

    public void RemoveHead()
    {
        RemoveItemSlot("head");
    }
    public void RemoveWeapon()
    {
        RemoveItemSlot("weapon");
    }
    public void RemoveUtil1()
    {
        RemoveItemSlot("util1");
    }
    public void RemoveUtil2()
    {
        RemoveItemSlot("util2");
    }
    public void RemoveUtil3()
    {
        RemoveItemSlot("util3");
    }




    //인벤토리 아이템 클릭 시 해당 아이템의 포지션 반환
    public void GetItemPosition(int position)
    {
        itemPosition = position;
    }

    //프리뷰 아이템 슬롯 클릭 시 해당 슬롯의 포지션 반환
    public void GetPrevPosition(int position)
    {
        prevPosition = position;
    }

    public void OpenObject(GameObject target)
    {
        target.SetActive(true);
    }

    public void CloseObject(GameObject target)
    {
        target.SetActive(false);
    }

    public void UseItemInInventory(GameObject PlayerCharacter)
    {
        ItemModel item = PlayerCharacter.GetComponent<CharacterModel>().ItemLists[itemPosition];
        if (item.metaInfo.itemType.Equals(ItemType.Etc))
        {
            UseEtc(PlayerCharacter);
        }
        else
        {
            WearEquip(PlayerCharacter);
        }
    }

    public void RemoveItmeInInventory(GameObject PlayerCharacter)
    {
        PlayerCharacter.GetComponent<CharacterModel>().RemoveItemAtIndex(itemPosition);
    }

    public void UseEtc(GameObject PlayerCharacter)
    {
        ItemModel etc = PlayerCharacter.GetComponent<CharacterModel>().ItemLists[itemPosition];
        if (PlayerCharacter.GetComponent<CharacterModel>().UseExpendables(etc))
        {

            PlayerCharacter.GetComponent<CharacterModel>().ItemCounts[itemPosition]--;
            if(PlayerCharacter.GetComponent<CharacterModel>().ItemCounts[itemPosition] == 0)
            {
                RemoveItmeInInventory(PlayerCharacter);
                ClearDescription();
            }
        }
    }

    public void WearEquip(GameObject PlayerCharacter)
    {
        ItemModel equip = PlayerCharacter.GetComponent<CharacterModel>().ItemLists[itemPosition];
        if (PlayerCharacter.GetComponent<CharacterModel>().WearEquipment(equip))
        {
            RemoveItmeInInventory(PlayerCharacter);
            ClearDescription();
        }
    }
}
/*
    public void ItemClicked(GameObject PlayerCharacter)
    {
        DestroyAllButton(PlayerCharacter);
        Vector3 pos = ItemSlots[itemPosition].transform.position;
        GameObject button;
        ItemModel selectedItem = null;

        Debug.Log("Item Clicked");

        try
        {
            selectedItem = PlayerCharacter.GetComponent<CharacterModel>().ItemLists[itemPosition];
        }
        catch
        {
            return ;
        }
        

        if (selectedItem != null)
        {
            if (selectedItem.metaInfo.itemType.Equals(ItemType.Etc))
            {
                button = Instantiate(EtcButton) as GameObject;
            }
            else
            {
                button = Instantiate(EquipButton) as GameObject;
            }

            button.transform.SetParent(ItemSlots[itemPosition].transform.parent.transform);
            button.transform.localScale = new Vector3(1, 1, 1);
            button.transform.localPosition = new Vector3(1, 1, 1);

        }
        else
        {
            Debug.Log("Selected Item Slot is Empty");
        }
            return ;
    }


    public void DestroyAllButton(GameObject PlayerCharacter)
    {

        List<ItemType> itemTypes = new List<ItemType>();

        foreach(ItemModel item in PlayerCharacter.GetComponent<CharacterModel>().ItemLists)
        {
            ItemType _itemType = item.metaInfo.itemType;
            itemTypes.Add(_itemType);
        }


        for(int index = 0; index < 30; index++)
        {
            GameObject parent = ItemSlots[index].transform.parent.gameObject;
            GameObject button;

            if (itemTypes[index].Equals(ItemType.Etc))
            {
                try
                {
                    button = parent.transform.FindChild("ItemClickBtn_Etc(Clone)").gameObject;
                }
                catch
                {
                    return ;
                }
            }
            else
            {
                try
                {
                    button = parent.transform.FindChild("ItemClickBtn_Equip(Clone)").gameObject;
                }
                catch
                {
                    return ;
                }                
            }
            Destroy(button);
        }

    }
*/