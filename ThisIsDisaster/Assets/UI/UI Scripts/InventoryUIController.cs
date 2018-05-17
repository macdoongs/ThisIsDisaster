using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour {
    
    public GameObject Player;
    public CharacterModel PlayerCharacter;

    public int bagSize = 30;

    public int typePosition = 0;
    public int itemPosition = 0;
    public int prevPosition = 0;

    //인벤토리 타입 상단 아이템 종류 텍스트
    public Text[] Category = new Text[5];

    //인벤토리 타입 하단 언더바 이미지
    public GameObject[] CategoryUnderBar = new GameObject[5];

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
    public string[] PreviewSlotName = new string[5] { "head", "weapon", "util1", "util2", "util3" };
    public ItemModel[] PreviewItems = new ItemModel[5];


    //인벤토리 착용장비 기본 이미지셋 
    public Sprite[] defaultSlotSprite = new Sprite[5];

    //스프라이트 소스
    string[] spriteSrcs;

    public GameObject DescriptionPanel;

    public Image SlotDescriptionItemImage;
    public Text SlotDescriptionItemName;
    public Text SlotDescriptionItemDescription;
    public Text[] SlotDescriptionItemStats = new Text[3];
    public Text[] SlotDescriptionItemStatAmount = new Text[3];
    public Text SlotDescriptionRightButton;
    public Text SlotDescriptionLeftButton;
    public GameObject SlotDescriptionRegisterButton;
    
    
    public GameObject PrevDescriptionPanel;

    public Image PrevDescriptionItemImage;
    public Text PrevDescriptionItemName;
    public Text PrevDescriptionItemDescription;
    public Text[] PrevDescriptionItemStats = new Text[3];
    public Text[] PrevDescriptionItemStatAmount = new Text[3];
    public Text PrevDescriptionRightButton;
    public Text PrevDescriptionLeftButton;

    public SpriteRenderer[] PreviewSpriteParts = new SpriteRenderer[18];

    DescriptionUI SlotDescription;
    DescriptionUI PrevDescription;
    DescriptionUI FirstItemDescription;
    DescriptionUI SecondItemDescription;

    
    
    public GameObject ChangePanel;

    public Image FirstItemImage;
    public Text FirstItemName;
    public Text[] FirstItemStats = new Text[3];
    public Text[] FirstItemStatAmount = new Text[3];

    public Image SecondItemImage;
    public Text SecondItemName;
    public Text[] SecondItemStats = new Text[3];
    public Text[] SecondItemStatAmount = new Text[3];

    public GameObject PresetPanel;
    public Image[] PresetSprites;
    public GameObject[] PresetRemoves;
    public int presetPosition;
    public Button[] PresetButtons = new Button[6];
    public Button PresetRegisterButton;
    public PresetItem[] PresetItems = new PresetItem[6];

    public Image[] PresetBarSprite = new Image[6];
    public Text[] PresetBarCounts = new Text[6];


    public ItemModel[] ItemSlot ;

    public void Start()
    {
        if (Player == null) {
            Player = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
        }
        PlayerCharacter = Player.GetComponent<CharacterModel>();

        SlotDescription = new DescriptionUI(DescriptionPanel , SlotDescriptionItemImage, SlotDescriptionItemName, SlotDescriptionItemDescription, 
        SlotDescriptionItemStats, SlotDescriptionItemStatAmount,  SlotDescriptionLeftButton, SlotDescriptionRightButton, SlotDescriptionRegisterButton);

        PrevDescription = new DescriptionUI(PrevDescriptionPanel, PrevDescriptionItemImage, PrevDescriptionItemName, PrevDescriptionItemDescription,
        PrevDescriptionItemStats, PrevDescriptionItemStatAmount, PrevDescriptionLeftButton ,PrevDescriptionRightButton );

        FirstItemDescription = new DescriptionUI(FirstItemImage, FirstItemName, FirstItemStats, FirstItemStatAmount);
        SecondItemDescription = new DescriptionUI(SecondItemImage, SecondItemName, SecondItemStats, SecondItemStatAmount);


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
            ItemButtons[i].onClick.AddListener(() => SlotItemClicked());
        }

        PreviewItems = new ItemModel[5] { PlayerCharacter.headSlot , PlayerCharacter.weaponSlot , PlayerCharacter.utilSlot1
                , PlayerCharacter.utilSlot2, PlayerCharacter.utilSlot3};

        PresetRegisterButton.onClick.AddListener(() => PresetEditOpen());
    }

    //인벤토리 업데이트. InGameUIScript에서 사용.
    public void InventoryUpdate()
    {
        SlotCategory(typePosition);

        DefaultBorder();

        switch (typePosition)
        {
            case 0:
                break;
            case 1:
                HighlightsCategory(ItemType.Head);
                break;
            case 2:
                HighlightsCategory(ItemType.Weapon);
                break;
            case 3:
                HighlightsCategory(ItemType.Util);
                break;
            case 4:
                HighlightsCategory(ItemType.Etc);
                break;
        }
        PresetUpdate();
        PlayerCharacter.SpriteUpdate();
        PreviewCharacterSprite();
    }

    //****************************************************************************************//
    // 아이템 인벤토리 관련 기능들

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
    public void SlotSprite()
    {
        initialSprite();
        var items = PlayerCharacter.GetAllItems();
        var counts = PlayerCharacter.GetAllCounts();

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
 
    //인벤토리 상단 아이템 카테고리 선택시
    public void SlotCategory(int index)
    {
        DefaultCategory();
        Category[index].color = focusedItemTypeColor;
        CategoryUnderBar[index].SetActive(true);
    }

    public void DefaultCategory()
    {
        for (int i = 0; i < Category.Length; i++)
        {
            Category[i].color = defaultItemTypeColor;
            CategoryUnderBar[i].SetActive(false);
        }
    }

    //인벤토리 상단 아이템 카테고리 초기 상태. InGameUIScript에서 사용
    public void InitialCategory()
    {
        DefaultCategory();
        SlotCategory(0);
    }


    public void DefaultBorder()
    {
        int i = 0;

        for(i =0; i < bagSize; i++)
        {
            ItemBorders[i].color = defaultItemTypeColor;
        }        
    }


    public void GetPosition(int pos)
    {
        typePosition = pos;
    }

    //인벤토리 상단 아이템 카테고리 선택시 해당 아이템 타입 보더 변경. Update 에서 사용됨
    public void HighlightsCategory(ItemType type )
    {
        List<ItemModel> items = PlayerCharacter.ItemLists;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].metaInfo.itemType.Equals(type))
            {
                ItemBorders[i].color = typeColor;
            }
        }
    }

    //인벤토리 아이템 슬롯 클릭시
    public void SlotItemClicked()
    {
        defaultDescriptions();

        ItemModel item = null;

        try
        {
            item = PlayerCharacter.ItemLists[itemPosition];
        }
        catch
        {
            Debug.Log("Item Slot is Empty");
            DescriptionPanel.SetActive(false);
            return;
        }

        if (item != null)
        {
            SlotDescription.SetDescription(item);
        }

        SlotDescription.DescriptionPanel.SetActive(true);
    }

    public void UseSlotItem()
    {

        ItemModel item = PlayerCharacter.ItemLists[itemPosition];
        if (item.metaInfo.itemType.Equals(ItemType.Etc)){
            UseEtc();
        }
        else{
            WearEquip();
        }
    }

    public void UseEtc()
    {
        ItemModel etc = PlayerCharacter.ItemLists[itemPosition];
        if (PlayerCharacter.UseExpendables(etc)){
            PlayerCharacter.ItemCounts[itemPosition]--;
            if (PlayerCharacter.ItemCounts[itemPosition] == 0){
                RemoveSlotItem();                
            }
        }
    }

    public void WearEquip()
    {
        ItemModel equip = PlayerCharacter.ItemLists[itemPosition];
        if (PlayerCharacter.WearEquipment(equip)){
            RemoveSlotItem();            
        }
        else{
            ChangeUIOpen();
        }
    }

    public void RemoveSlotItem()
    {
        ItemModel item = PlayerCharacter.ItemLists[itemPosition];

        int removedItemPosition = itemPosition;
        PresetItemRunOut(item , removedItemPosition);
        PlayerCharacter.RemoveItemAtIndex(itemPosition);
        SlotDescription.ClearDescription();
    }

    //인벤토리 아이템 클릭 시 해당 아이템의 포지션 반환
    public void GetItemPosition(int position)
    {
        itemPosition = position;
    }

    //****************************************************************************************//
    //프리뷰 슬롯 관련 기능들


    public void PreviewCharacterSprite()
    {
        DefaultPreviewCharacterSprite();

        int i = 0;
        foreach(var part in PlayerCharacter.SpriteParts)
        {
            

            if (part.sprite != null)
            {
                Sprite s = part.sprite;
                PreviewSpriteParts[i].sprite = s;
                PreviewSpriteParts[i].color = part.color;
            }
            else
            {
                PreviewSpriteParts[i].sprite = null;
                PreviewSpriteParts[i].color = Color.clear;
            }
            i++;
        }
    }

    public void DefaultPreviewCharacterSprite()
    {
        foreach(var sprite in PreviewSpriteParts)
        {
            sprite.sprite = null;
            sprite.color = Color.white;
        }
    }


    //인벤토리 좌측 프리뷰 슬롯 스프라이트 할당
    public void PreviewSprite()
    {
        //착용중인 장비 슬롯
        getPreviewItems();

        for (int i = 0; i < 5; i++)
        {

            if (PreviewItems[i] != null)
            {
                string src = PreviewItems[i].metaInfo.spriteSrc;
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

    public void getPreviewItems()
    {
        PreviewItems = new ItemModel[5] { PlayerCharacter.headSlot , PlayerCharacter.weaponSlot , PlayerCharacter.utilSlot1
                , PlayerCharacter.utilSlot2, PlayerCharacter.utilSlot3};
    }

    public void PreviewItemClicked()
    {
        defaultDescriptions();

        getPreviewItems();

        //선택된 preview item
        ItemModel item = PreviewItems[prevPosition];
        
        if (item != null)
            {
                PrevDescription.SetDescription(item);           
                PrevDescriptionPanel.SetActive(true);
            }
            else
                return ;
    }
    
    public void RemovePreviewItem()
    {
        getPreviewItems();
        ItemModel PrevItem = PreviewItems[prevPosition];
        long metaID = PrevItem.metaInfo.metaId;
        ItemManager.Manager.AddItem(PlayerCharacter, metaID, 1);

        string slotName = PreviewSlotName[prevPosition];
        PlayerCharacter.RemoveEquipment(slotName);

        PrevDescription.ClearDescription();
    }
    
    //프리뷰 아이템 슬롯 클릭 시 해당 슬롯의 포지션 반환
    public void GetPrevPosition(int position)
    {
        prevPosition = position;
    }

    //****************************************************************************************//
    //아이템 교체(장비 아이템) 관련 기능

    //아이템 교체 창 오픈
    public void ChangeUIOpen()
    {
        ItemModel first, second;
        findChageItems(out first, out second);

        SetChangeDescription(first, second);

        ChangePanel.SetActive(true);
    }


    // 교체할 장비(second) 와 교체될 장비(first) 반환
    private void findChageItems(out ItemModel first, out ItemModel second)
    {
        second = PlayerCharacter.ItemLists[itemPosition];
        ItemType type = second.metaInfo.itemType;

        if (type.Equals(ItemType.Head))
            first = PlayerCharacter.headSlot;
        else if (type.Equals(ItemType.Weapon))
            first = PlayerCharacter.weaponSlot;
        else
            first = PlayerCharacter.utilSlot3; ;
    }

    //교체 아이템 정보 입력
    public void SetChangeDescription(ItemModel First, ItemModel Second)
    {
        FirstItemDescription.ClearDescription();
        FirstItemDescription.SetDescription(First);
        SecondItemDescription.ClearDescription();
        SecondItemDescription.SetDescription(Second);
    }

    //아이템 교체.
    public void ChangeItem()
    {
        ItemType type = PlayerCharacter.ItemLists[itemPosition].metaInfo.itemType;
        getPrevPositionByType(type);
        RemovePreviewItem();
        WearEquip();
        defaultDescriptions();
    }


    public void defaultDescriptions()
    {
        SlotDescription.ClearDescription();
        PrevDescription.ClearDescription();
        ChangePanel.SetActive(false);
        PresetPanel.SetActive(false);
    }

    public void getPrevPositionByType(ItemType type)
    {
        if (type.Equals(ItemType.Head))
        {
            GetPrevPosition(0);
        }
        else if (type.Equals(ItemType.Weapon))
        {
            GetPrevPosition(1);
        }
        else
        {
            GetPrevPosition(4);
        }
    }


    //****************************************************************************************//
    //아이템(소모품) 등록 관련 기능

    public class PresetItem
    {
        public int itemPosition;
        public ItemModel Item;

        public PresetItem()
        {
            itemPosition = -1;
            Item = null;
        }

        public PresetItem(int position, ItemModel item)
        {
            itemPosition = position;
            Item = item;
        }
    }

    public void OpenPrestSetting()
    {
        PresetEditClose();
        RegisterButtonOn();
        PresetPanel.SetActive(true);
    }

    public void PresetRegister()
    {
        if(PresetItems[presetPosition] != null)
        {
            return ;
        }

        ItemModel item = PlayerCharacter.ItemLists[itemPosition];

        if (CheckRegistered(itemPosition))
        {

            PresetItems[presetPosition] = new PresetItem(itemPosition, item);

            PresetUpdate();
        }
        else
        {
            Debug.Log("Item is already registered");
        }
    }

    public void PresetSprite(ItemModel item)
    {

        string src = item.metaInfo.spriteSrc;
        Sprite s = Resources.Load<Sprite>(src);

        PresetSprites[presetPosition].sprite = s;
        PresetSprites[presetPosition].color = Color.white;

        PresetBarSprite[presetPosition].sprite = s;
        PresetBarSprite[presetPosition].color = Color.white;
        PresetBarCounts[presetPosition].text = ItemCounts[itemPosition].text;

    }

    public void PresetUpdate()
    {
        for(int i = 0; i < 6; i++)
        {
            PresetItem presetItem = PresetItems[i];

            if(presetItem != null)
            {
                if(presetItem.Item != null)
                {
                    string src = presetItem.Item.metaInfo.spriteSrc;
                    Sprite s = Resources.Load<Sprite>(src);


                    int position = presetItem.itemPosition;

                    PresetSprites[i].sprite = s;
                    PresetSprites[i].color = Color.white;

                    PresetBarSprite[i].sprite = s;
                    PresetBarSprite[i].color = Color.white;
                    PresetBarCounts[i].text = PlayerCharacter.ItemCounts[position].ToString();
                }
            }
            else if(presetItem == null)
            {
                PresetSprites[i].sprite = null;
                PresetSprites[i].color = Color.clear;

                PresetBarSprite[i].sprite = null;
                PresetBarSprite[i].color = Color.clear;
                PresetBarCounts[i].text = "";
            }
        }


    }

    public void PresetEditOpen()
    {

        foreach (var btn in PresetRemoves)
        {
            btn.SetActive(true);
        }

        PresetRegisterButton.GetComponentInChildren<Text>().text = "단축키 등록";
        PresetRegisterButton.onClick.AddListener(() => PresetEditClose());
        RegisterButtonOff();
    }

    public void PresetEditClose()
    {
        RegisterButtonOn();
        foreach (var btn in PresetRemoves)
        {
            btn.SetActive(false);
        }

        PresetRegisterButton.GetComponentInChildren<Text>().text = "단축키 편집";
        PresetRegisterButton.onClick.AddListener(() => PresetEditOpen());

    }

    public void PresetRemove(int position)
    {
        PresetItems[position] = null;

        PresetSprites[position].sprite = null;
        PresetSprites[position].color = Color.clear;

        PresetBarSprite[presetPosition].sprite = null;
        PresetBarSprite[presetPosition].color = Color.clear;
    }

    public void RegisterButtonOn()
    {
        for (int i = 0; i < 6; i++) {
            PresetButtons[i].onClick.AddListener(() => PresetRegister());
        }
    }

    public void RegisterButtonOff()
    {
        for(int i = 0; i < 6; i++)
        {
            PresetButtons[i].onClick.RemoveListener(PresetRegister);
        }
    }

    //true면 아직 등록되지 않음. false면 이미 등록됨
    public bool CheckRegistered(int position)
    {
        foreach(var preset in PresetItems)
        {

            if((preset != null) && (preset.itemPosition == position))
            {
                return false;
            }
        }

        return true;
    }

    public void getPresetPosition(int position)
    {
        presetPosition = position;
    }

    public void PresetBarClicked(int position)
    {

        if (PresetItems[position] != null)
        {
            if(PresetItems[position].Item != null)
            {
                int temp = itemPosition;

                itemPosition = PresetItems[position].itemPosition;
                UseSlotItem();
                itemPosition = temp;
            }
        }
    }

    public void PresetItemRunOut(ItemModel item, int removedItemPosition)
    {

        for(int i = 0; i < 6; i++)
        {
            PresetItem presetItem = PresetItems[i];

            if(presetItem != null)
            {
                if (presetItem.Item.Equals(item))
                {
                    PresetItems[i] = null;
                    continue;
                }

                if (presetItem.itemPosition > removedItemPosition)
                {
                    presetItem.itemPosition--;
                }
            }


        }

        PresetUpdate();
    }



}