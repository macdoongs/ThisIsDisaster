using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour, IObserver {

    public GameObject InventoryUI;

    public GameObject Player;
    public CharacterModel PlayerCharacter;

    
    public int bagSize = 0;

    public int typePosition = 0;
    public int itemPosition = 0;
    public int prevPosition = 0;

    //인벤토리 타입 상단 아이템 종류 텍스트
    public Text[] Category = new Text[4];

    //인벤토리 타입 하단 언더바 이미지
    public GameObject[] CategoryUnderBar = new GameObject[4];

    public Color defaultItemTypeColor;
    public Color focusedItemTypeColor;
    public Color typeColor;
    public Color defaultSlotColor;
    public GameObject Content;
    public Color RestrictedSlotClolr;
    public Image[] ItemSlotBackground;
    //인벤토리 아이템 슬롯 이미지
    public Image[] ItemSlots ;
    //인벤토리 아이템 개수 텍스트
    public Text[] ItemCounts ;
    //인벤토리 슬롯 테두리
    public Image[] ItemBorders ;
    //인벤토리 아이템 슬롯 버트
    public Button[] ItemButtons ;

    //인벤토리 착용장비 슬롯 이미지
    public Image[] PreviewSlots = new Image[6];
    public string[] PreviewSlotName;
    public ItemModel[] PreviewItems = new ItemModel[6];


    //인벤토리 착용장비 기본 이미지셋 
    public Sprite[] defaultSlotSprite = new Sprite[6];

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

    public GameObject RecipePanel;
    public GameObject RecipeListPanel;
    public GameObject RecipePrefeb;

    public GameObject RecipeDescPanel;
    public GameObject RecipeDescRequiredPanel;
    public GameObject RecipeDescReservedPanel;
    public Text RecipeDescNameText;

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


    public static InventoryUIController Instance
    {
        private set;
        get;
    }

    private void Awake()
    {
        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        RemoveNotices();
    }

    public void Start()
    {
        ObserveNotices();


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
            if(i%4 == 1)
            {
                items[i / 4] = temp[i];
            }
        }

        ItemSlotBackground = new Image[30];
        ItemSlots = new Image[30];
        ItemCounts = new Text[30];
        ItemBorders = new Image[30];
        ItemButtons = new Button[30];

        for (int i =0; i < 30; i++)
        {
            ItemSlotBackground[i] = items[i].transform.GetComponentsInChildren<Image>()[0];
            ItemSlots[i] = items[i].transform.GetComponentsInChildren<Image>()[1];
            ItemCounts[i] = items[i].transform.GetComponentInChildren<Text>();
            ItemBorders[i] = items[i].transform.GetComponentsInChildren<Image>()[2];
            ItemButtons[i] = items[i].transform.GetComponentInChildren<Button>();
            int index = i;
            ItemButtons[i].onClick.AddListener(() => GetItemPosition(index));
            ItemButtons[i].onClick.AddListener(() => SlotItemClicked());
        }

        PreviewSlotName =
            new string[6] { "head", "clothes", "weapon", "backpack", "bottle", "flash" };

        PresetRegisterButton.onClick.AddListener(() => PresetEditOpen());
    }

    //인벤토리 업데이트. InGameUIScript에서 사용.
    public void InventoryUpdate()
    {
        SlotCategory(typePosition);

        HighlightCategoryType();

        SlotSprite();
        PresetUpdate();
        PlayerCharacter.SpriteUpdate();
        PreviewCharacterSprite();
    }

    public void HighlightCategoryType()
    {
        DefaultBorder();

        switch (typePosition)
        {
            case 0:                
                break;
            case 1:
                HighlightsCategory(ItemType.Head);
                break;
            case 2:
                HighlightsCategory(ItemType.Etc);
                break;
            case 3:
                HighlightsCategory(ItemType.Normal);
                break;
        }
    }

    //****************************************************************************************//
    // 아이템 인벤토리 관련 기능들

    //인벤토리 슬롯 스프라이트 초기화
    public void initialSprite()
    {
        bagSize = PlayerCharacter.bagSize;

        for (int i = 0; i < bagSize; i++)
        {
            ItemSlotBackground[i].color = defaultSlotColor;
            ItemSlots[i].sprite = null;
            ItemSlots[i].color = Color.clear;
            ItemCounts[i].text = "";
        }
        for (int i = bagSize; i < 30; i++)
        {
            ItemSlotBackground[i].color = RestrictedSlotClolr;
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
                ItemSlotBackground[index].color = defaultSlotColor;
                ItemSlots[index].sprite = s;
                ItemSlots[index].color = Color.white;
                ItemSlots[index].preserveAspect = true;
                ItemCounts[index].text = counts[index].ToString();
            }           
        }

        for(index = PlayerCharacter.bagSize; index < 30; index++)
        {
            ItemSlotBackground[index].color = RestrictedSlotClolr;
            ItemSlots[index].sprite = null;
            ItemSlots[index].color = Color.clear;
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

        if (type.Equals(ItemType.Head))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].metaInfo.itemType.Equals(type) || items[i].metaInfo.itemType.Equals(ItemType.Clothes)
                    || items[i].metaInfo.itemType.Equals(ItemType.Weapon) || items[i].metaInfo.itemType.Equals(ItemType.Backpack)
                    || items[i].metaInfo.itemType.Equals(ItemType.Bottle) || items[i].metaInfo.itemType.Equals(ItemType.Tool_Equip))
                {
                    ItemBorders[i].color = typeColor;
                }
            }
        }
        else if (type.Equals(ItemType.Etc))
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].metaInfo.itemType.Equals(type) || items[i].metaInfo.itemType.Equals(ItemType.Tool_Use))
                {
                    ItemBorders[i].color = typeColor;
                }
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].metaInfo.itemType.Equals(type))
                {
                    ItemBorders[i].color = typeColor;
                }
            }
        }
        
    }

    //인벤토리 아이템 슬롯 클릭시
    public void SlotItemClicked()
    {
        DefaultRecipePanel();
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
        ItemType type = item.metaInfo.itemType;
        if (type.Equals(ItemType.Etc) || type.Equals(ItemType.Tool_Use))
        {
            UseEtc();
        }
        else if(type.Equals(ItemType.Normal))
        {
            Recipe();
            
        }
        else
        {
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

    public void Recipe()
    {
        DefaultRecipeDescription();
        DefaultRecipePanel();

        ItemModel norm = PlayerCharacter.ItemLists[itemPosition];
        List<MixtureRecipe> recipeList = ItemManager.Manager.ContainRecipe(norm);

        if(recipeList.Count != 0)
        {
            int index = 0;
            foreach(var recipe in recipeList)
            {
                

                ItemModel result = ItemManager.Manager.MakeItem(recipe.resultID);
                ItemModel mat1 = ItemManager.Manager.MakeItem(recipe.MaterialID[0]);
                long mat1_num = recipe.MaterialNum[0];


                GameObject recipePrefeb = Instantiate(RecipePrefeb);


                Image resultImage = recipePrefeb.GetComponentsInChildren<Image>()[1];
                Image Material1Image = recipePrefeb.GetComponentsInChildren<Image>()[2];
                Image Material2Image = recipePrefeb.GetComponentsInChildren<Image>()[3];
                Text Material1Num = recipePrefeb.GetComponentsInChildren<Text>()[2];
                Text Material2Num = recipePrefeb.GetComponentsInChildren<Text>()[4];

                if (recipe.MaterialID.Count == 2)
                {
                    ItemModel mat2 = ItemManager.Manager.MakeItem(recipe.MaterialID[1]);
                    long mat2_num = recipe.MaterialNum[1];
                    Material2Image.sprite = Resources.Load<Sprite>(mat2.metaInfo.spriteSrc);
                    Material2Num.text = mat2_num.ToString();
                }
                else{
                    Material2Image.sprite = null;
                    Material2Image.color = Color.clear;
                    recipePrefeb.GetComponentsInChildren<Text>()[3].text = "";
                    Material2Num.text = "";
                }

                resultImage.sprite = Resources.Load<Sprite>(result.metaInfo.spriteSrc);
                Material1Image.sprite = Resources.Load<Sprite>(mat1.metaInfo.spriteSrc);

                Material1Num.text = mat1_num.ToString();
                int i = index;
                recipePrefeb.GetComponentInChildren<Button>().onClick.AddListener(() => 
                        RecipeDescription(i));

                if(index % 2 == 0)
                    recipePrefeb.GetComponentInChildren<Image>().color = Color.white;
                else
                    recipePrefeb.GetComponentInChildren<Image>().color = defaultSlotColor;

                recipePrefeb.transform.parent = RecipeListPanel.transform;
                recipePrefeb.transform.localScale = new Vector3(1, 1, 1);
                recipePrefeb.SetActive(true);
                index++;
            }

            RecipePanel.SetActive(true);
        }

    }

    public void DefaultRecipePanel()
    {
        for(int i = 0; i < RecipeListPanel.transform.childCount; i++)
        {            
            DestroyObject(RecipeListPanel.transform.GetChild(i).gameObject);
        }

        RecipePanel.SetActive(false);
    }

    public void RecipeDescription(int index)
    {
        DefaultRecipeDescription();

        ItemModel item = PlayerCharacter.ItemLists[itemPosition];
        List<MixtureRecipe> recipeList = ItemManager.Manager.ContainRecipe(item);
        MixtureRecipe recipe = recipeList[index];

        ItemModel ResultItem = ItemManager.Manager.MakeItem(recipe.resultID);
        ItemModel Material1 = ItemManager.Manager.MakeItem(recipe.MaterialID[0]);
        int Material1Num = recipe.MaterialNum[0];
       

        RecipeDescNameText.text = ResultItem.metaInfo.Name.ToString();

        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[1].sprite = 
            Resources.Load<Sprite>(Material1.metaInfo.spriteSrc); ;
        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[1].color = Color.white;
        RecipeDescRequiredPanel.GetComponentsInChildren<Text>()[1].text = 
            Material1Num.ToString() + " 개";
        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[1].sprite =
           Resources.Load<Sprite>(Material1.metaInfo.spriteSrc); ;
        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[1].color = Color.white;
        int reservedMat1Num = PlayerCharacter.GetReservedItemCount(Material1);
        RecipeDescReservedPanel.GetComponentsInChildren<Text>()[1].text = reservedMat1Num.ToString() + " 개";

        if (recipe.MaterialID.Count == 2)
        {
            ItemModel Material2 = ItemManager.Manager.MakeItem(recipe.MaterialID[1]);
            int Material2Num = recipe.MaterialNum[1];
            RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[2].sprite =
                Resources.Load<Sprite>(Material2.metaInfo.spriteSrc); ;
            RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[2].color = Color.white;
            RecipeDescRequiredPanel.GetComponentsInChildren<Text>()[2].text = 
                Material2Num.ToString() + " 개";

            RecipeDescReservedPanel.GetComponentsInChildren<Image>()[2].sprite =
                Resources.Load<Sprite>(Material2.metaInfo.spriteSrc); ;
            RecipeDescReservedPanel.GetComponentsInChildren<Image>()[2].color = Color.white;

            int reservedMat2Num = PlayerCharacter.GetReservedItemCount(Material2);
            RecipeDescReservedPanel.GetComponentsInChildren<Text>()[2].text = reservedMat2Num.ToString() + " 개";
        }

        MixtureRecipe ParamRecipe = recipe;
        RecipeDescPanel.GetComponentsInChildren<Button>()[1].onClick.AddListener(() =>
                        MakeRecipe(recipe));

        RecipeDescPanel.SetActive(true);

    }

    public void DefaultRecipeDescription()
    {
        RecipeDescNameText.text = "";
        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[1].sprite = null;
        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[1].color = Color.clear;
        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[2].sprite = null;
        RecipeDescRequiredPanel.GetComponentsInChildren<Image>()[2].color = Color.clear;

        RecipeDescRequiredPanel.GetComponentsInChildren<Text>()[1].text = "";
        RecipeDescRequiredPanel.GetComponentsInChildren<Text>()[2].text = "";

        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[1].sprite = null;
        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[1].color = Color.clear;
        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[2].sprite = null;
        RecipeDescReservedPanel.GetComponentsInChildren<Image>()[2].color = Color.clear;

        RecipeDescReservedPanel.GetComponentsInChildren<Text>()[1].text = "";
        RecipeDescReservedPanel.GetComponentsInChildren<Text>()[2].text = "";
        RecipeDescPanel.GetComponentsInChildren<Button>()[1].onClick.RemoveAllListeners();
        RecipeDescPanel.SetActive(false);
    }

    public void MakeRecipe(MixtureRecipe recipe)
    {
        if(PlayerCharacter.ItemLists.Count == PlayerCharacter.bagSize)
        {
            InGameUIScript.Instance.Notice("Warning", "가방 공간이 부족합니다.\n 가방을 비워주세요.");
            return;
        }

        string ResultItemName = ItemManager.Manager.MakeItem(recipe.resultID).metaInfo.Name;
        ItemModel Material1 = ItemManager.Manager.MakeItem(recipe.MaterialID[0]);
        int Material1Num = recipe.MaterialNum[0];
        int reservedMat1Num = PlayerCharacter.GetReservedItemCount(Material1);

        if (recipe.MaterialID.Count == 2)
        {
            ItemModel Material2 = ItemManager.Manager.MakeItem(recipe.MaterialID[1]);
            int Material2Num = recipe.MaterialNum[1];
            int reservedMat2Num = PlayerCharacter.GetReservedItemCount(Material2);

            if (reservedMat1Num >= Material1Num && reservedMat2Num >= Material2Num)
            {
                PlayerCharacter.AddItem(ItemManager.Manager.MakeItem(recipe.resultID), 1);

                for (int i = 0; i < PlayerCharacter.ItemLists.Count; i++)
                {
                    if (PlayerCharacter.ItemLists[i] != null)
                    {
                        if (PlayerCharacter.ItemLists[i].metaInfo.Name == Material1.metaInfo.Name)
                        {
                            PlayerCharacter.ItemCounts[i] -= Material1Num;
                            if (PlayerCharacter.ItemCounts[i] <= 0)
                            {
                                PlayerCharacter.RemoveItemAtIndex(i);
                                continue;
                            }
                        }

                        if (PlayerCharacter.ItemLists[i].metaInfo.Name == Material2.metaInfo.Name)
                        {
                            PlayerCharacter.ItemCounts[i] -= Material2Num;
                            if (PlayerCharacter.ItemCounts[i] <= 0)
                            {
                                PlayerCharacter.RemoveItemAtIndex(i);
                            }
                        }
                    }
                }

                DefaultRecipePanel();
                InGameUIScript.Instance.Notice(ResultItemName, "조합 성공");
                return;
            }
        }
        else
        {
            if (reservedMat1Num >= Material1Num)
            {
                PlayerCharacter.AddItem(ItemManager.Manager.MakeItem(recipe.resultID), 1);

                for (int i = 0; i < PlayerCharacter.ItemLists.Count; i++)
                {
                    if (PlayerCharacter.ItemLists[i] != null)
                    {
                        if (PlayerCharacter.ItemLists[i].metaInfo.Name == Material1.metaInfo.Name)
                        {
                            PlayerCharacter.ItemCounts[i] -= Material1Num;
                            if (PlayerCharacter.ItemCounts[i] <= 0)
                            {
                                PlayerCharacter.RemoveItemAtIndex(i);
                                break;
                            }
                        }
                    }
                }

                DefaultRecipePanel();
                InGameUIScript.Instance.Notice(ResultItemName, "조합 성공");
                return;
            }
        }
        DefaultRecipePanel();
        InGameUIScript.Instance.Notice(ResultItemName, "재료가 부족합니다.");

        InventoryUpdate();
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

        for (int i = 0; i < PreviewSlots.Length; i++)
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
        PreviewItems = new ItemModel[6] { PlayerCharacter.headSlot , PlayerCharacter.clothesSlot ,
            PlayerCharacter.weaponSlot , PlayerCharacter.backpackSlot ,
            PlayerCharacter.bottleSlot, PlayerCharacter.toolSlot};
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
        if(prevPosition == 3)
        {
            RemoveBackpack();
            PrevDescription.ClearDescription();
            return;
        }
        
        ItemModel PrevItem = PreviewItems[prevPosition];
        long metaID = PrevItem.metaInfo.metaId;
        ItemManager.Manager.AddItem(PlayerCharacter, metaID, 1);

        string slotName = PreviewSlotName[prevPosition];
        PlayerCharacter.RemoveEquipment(slotName);

        PrevDescription.ClearDescription();
    }
    public void RemoveBackpack()
    {
        ItemModel SlotBackpack = PlayerCharacter.backpackSlot;
        if (PlayerCharacter.ItemLists.Count > 4)
        {
            InGameUIScript.Instance.Notice("Warning","아이템이 너무 많습니다.\n먼저 가방을 비워주세요.");
        }
        else
        {
            PlayerCharacter.backpackSlot = null;
            PlayerCharacter.DefaultBagSize();
            PlayerCharacter.AddItem(SlotBackpack, 1);
        }
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
        else if (type.Equals(ItemType.Clothes))
            first = PlayerCharacter.clothesSlot;
        else if (type.Equals(ItemType.Weapon))
            first = PlayerCharacter.weaponSlot;
        else if (type.Equals(ItemType.Backpack))
            first = PlayerCharacter.backpackSlot;
        else if (type.Equals(ItemType.Bottle))
            first = PlayerCharacter.bottleSlot;
        else
            first = PlayerCharacter.toolSlot;
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
        if (type.Equals(ItemType.Backpack))
        {
            ChangeBackpack();
            defaultDescriptions();
            return;
        }
        getPrevPositionByType(type);
        RemovePreviewItem();
        WearEquip();
        defaultDescriptions();
    }

    public void ChangeBackpack()
    {
        ItemModel PrevBackpack = PlayerCharacter.backpackSlot;
        ItemModel NextBackpack = PlayerCharacter.ItemLists[itemPosition];

        if(PrevBackpack.GetSize() < NextBackpack.GetSize())
        {
            PlayerCharacter.RemoveEquipment("backpack");
            PlayerCharacter.WearEquipment(NextBackpack);          
            PlayerCharacter.AddItem(PrevBackpack, 1);
            RemoveSlotItem();
            defaultDescriptions();
        }
        else
        {
            InGameUIScript.Instance.Notice("Warning","작은 사이즈의 가방으로 교체할 수 없습니다.");
            defaultDescriptions();
        }

        SlotSprite();
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
        else if (type.Equals(ItemType.Clothes))
        {
            GetPrevPosition(1);
        }
        else if (type.Equals(ItemType.Weapon))
        {
            GetPrevPosition(2);
        }
        else if (type.Equals(ItemType.Backpack))
        {
            GetPrevPosition(3);
        }
        else if (type.Equals(ItemType.Bottle))
        {
            GetPrevPosition(4);
        }
        else
        {
            GetPrevPosition(5);
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



    public void Close()
    {
        InventoryUI.SetActive(false);
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.LocalPlayerGenerated) {
            if (Player == null)
            {
                Player = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
            }
            PlayerCharacter = Player.GetComponent<CharacterModel>();
            bagSize = PlayerCharacter.bagSize;
            getPreviewItems();
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.LocalPlayerGenerated, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.LocalPlayerGenerated, this);
    }

}