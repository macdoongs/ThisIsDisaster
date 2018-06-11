using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using Shelter;

/// <summary>
/// 임시
/// </summary>
public class PlayerModel : UnitModel
{
    public CharacterModel _character;

    private AutoTileMovementSetter _tileSetter;

    public override float GetAttackDamage()
    {
        return _character.CurrentStats.Damage;
    }

    public override string GetUnitName()
    {
        return _character.PlayerName;
    }

    public override void OnTakeDamage(UnitModel attacker, float damage)
    {
        Debug.Log(GetUnitName() + " Attacked By " + attacker.GetUnitName());

        //피격시 5% 확률로 부상 
        DisorderController.Instance.MakeDisorderByProbability(Disorder.DisorderType.injury, 5);

        bool check = _character.IsDead() == false;
        _character.SubtractHealth(damage);

        if (check) {
            if (_character.IsDead()) {
                Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.PlayerDead, GlobalParameters.Param.accountName, PlayerDeadType.Monster);
                if (!_character.HasItem(33004)) {
                    GameManager.CurrentGameManager.EndStage(false);
                }
                //
            }
        }
        

        SoundLayer.CurrentLayer.PlaySound("se_takedamage");
    }

    public void SetTileSetter(AutoTileMovementSetter tileSetter) {
        _tileSetter = tileSetter;
        _tileSetter.SetOwner(this);
    }

    public override TileUnit GetCurrentTile()
    {
        return _tileSetter.GetCurrentTile();
    }
    
    public override Vector3 GetCurrentPos()
    {
        return _character.transform.position;
    }
    
    public override void SetCurrentTileForcely(TileUnit tile)
    {
        _character.transform.position = tile.transform.position;
        _tileSetter.SetCurrentTileForcely(tile);
    }

    public int GetAttackAnimType() {
        if (_character.weaponSlot != null) {
            return _character.weaponSlot.GetAttackAnimType();
        }
        return 0;
    }

    public override AutoTileMovementSetter GetTileSetter()
    {
        return _tileSetter;
    }

    public override void SetShelter(ShelterModel shelter)
    {
        base.SetShelter(shelter);
        if (GameManager.CurrentGameManager.GetLocalPlayer() == _character.gameObject) {
            if (shelter != null)
            {
                Notice.Instance.Send(NoticeName.OnPlayerEnterShelter, shelter);
                Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.PlayerEnterShelter, GlobalParameters.Param.accountName);
            }
            else {
                Notice.Instance.Send(NoticeName.OnPlayerExitShelter);
                Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.PlayerExitShelter, GlobalParameters.Param.accountName);
            }
        }
    }
}

public class CharacterModel : MonoBehaviour
{
    public static CharacterModel Instance { get; private set; }

    public enum PlayerSpriteParts
    {
        Body = 0,
        LowBody,
        FrontLeg,
        BackLeg,
        BackOrnament,
        FrontArm = 5,
        Shoulder,
        FrontWeapon,
        Head,
        HeadOrnament,
        Back = 10,
        Mask,
        FrontHair,
        Face,
        BackHair,
        Tail = 15,
        BackArm,
        BackWeapon
    }
    public struct DefaultSpriteInfo {
        public PlayerSpriteParts part;
        public Sprite sprite;
        public Color color;
        public bool enabled;
    }

    public bool isPlayerDead = false;

    public bool MoveRestrict = false;

    public List<TileUnit> EffectTiles = new List<TileUnit>();
    public long EffectItemID;
    public bool EffectToken = false;
    //테스트용 값
    public string PlayerName = "TestID";
    public string PlayerLevel = "123";


    public Stats DefaultStats = new Stats();
    public int default_attack_range_x = 5;
    public int default_attack_range_y = 5;
    //가방(인벤토리) 사이즈. 
    private int defaultBagSize = 5;
    //최대 물 보관 개수.
    private int defaultWaterMax = 5;
    private int defaultVisionLevel = 0;

    public Stats CurrentStats = new Stats();
    public float tempMoveSpeed;
    public float attack_range_x = 0;
    public float attack_range_y = 0;
    public int bagSize = 0;
    public int waterMax = 0;
    public int visionLevel = 0;

    public Stats ItemStats = new Stats();
    
    public Stats DisorderStats = new Stats();

    public Stats EventStats = new Stats();

    public Stats RegionItemStats = new Stats();

    public Disorder[] disorders = new Disorder[5];

    //아이템 착용 슬롯
    public ItemModel[] EquipSlots;
    public ItemModel headSlot = null;
    public ItemModel clothesSlot = null;
    public ItemModel weaponSlot = null;
    public ItemModel backpackSlot = null;
    public ItemModel bottleSlot = null;
    public ItemModel toolSlot = null;


    public SpriteRenderer Body;

    public SpriteRenderer[] SpriteParts = new SpriteRenderer[18];

    public Sprite BackHair;
    public Sprite Tail;

    Dictionary<PlayerSpriteParts, DefaultSpriteInfo> _defaultSpriteInfo = new Dictionary<PlayerSpriteParts, DefaultSpriteInfo>();


    public Dictionary<long, ItemModel> ItemSpace = new Dictionary<long, ItemModel>();

    public List<ItemModel> ItemLists = new List<ItemModel> { };
    public List<string> ItemNames = new List<string> { };
    public List<int> ItemCounts = new List<int> { };

    public int cursor = 0;

    private PlayerModel _player;
    public AttackSender attackSender;
    public AttackReceiver attackReceiver;
    public AutoTileMovementSetter tileSetter;


    private float TimeLeft = 1.0f;
    private float nextTime = 1f;

    public float SpeedFactor {
        private set;
        get;
    }

    private void Awake()
    {
        _player = new PlayerModel
        {
            _character = this
        };
        if (attackSender)
            attackSender.SetOwner(_player);
        if (attackReceiver)
            attackReceiver.SetOwner(_player);
        if (tileSetter) {
            tileSetter.SetChangeAction(_player.SetCurrentTile);
            _player.SetTileSetter(tileSetter);
        }
        initialCharacterSetting();
        InitDefaultSprite();

        //Instance = this;
        SpeedFactor = 1f;
    }

    public void SetInstance() {
        Instance = this;
    }

    private void Start()
    {
        _restorationTimer.StartTimer(nextTime);
    }


    Timer _restorationTimer = new Timer();//회복 타이머
    private void Update()
    {
        if (MoveRestrict)
        {
            CurrentStats.MoveSpeed = 0;
        }

        //일정 시간마다 HP, Stamina 회복
        if (_restorationTimer.RunTimer()) {
            if (!isPlayerDead)
            {
                _restorationTimer.StartTimer(nextTime);
                StatRegeneration();
            }
        }

        EffectTileEffect(GetPlayerModel().GetCurrentTile());

    }

    public PlayerModel GetPlayerModel() {
        return _player;
    }

    public void InitDefaultStats()
    {
        DefaultStats.Health = 100.0f;
        DefaultStats.Stamina = 100.0f;
        DefaultStats.Defense = 10.0f;
        DefaultStats.Damage = 10.0f;
        DefaultStats.HealthRegen = 1.0f;
        DefaultStats.StaminaRegen = 1.0f;
        DefaultStats.MoveSpeed = 10;


        default_attack_range_x = 5;
        default_attack_range_y = 5;

        //가방(인벤토리) 사이즈. 
        defaultBagSize = 5;
        //최대 물 보관 개수.
        defaultWaterMax = 5;
    }

    private void initialCharacterSetting()
    {
        InitDefaultStats();

        attack_range_x = default_attack_range_x;
        attack_range_y = default_attack_range_y;
        bagSize = defaultBagSize;
        waterMax = defaultWaterMax;
        visionLevel = defaultVisionLevel;

        CurrentStats.Health = DefaultStats.Health;
        CurrentStats.Stamina = DefaultStats.Stamina;
        CurrentStats.MaxHealth = DefaultStats.MaxHealth;
        CurrentStats.MaxStamina = DefaultStats.MaxStamina;
        CurrentStats.HealthRegen = DefaultStats.HealthRegen;
        CurrentStats.StaminaRegen = DefaultStats.StaminaRegen;
        CurrentStats.Defense = DefaultStats.Defense;
        CurrentStats.Damage = DefaultStats.Damage;
        CurrentStats.MoveSpeed = DefaultStats.MoveSpeed;

        for (int i = 0; i < 5; i++) {
            disorders[i] = null;
        }
        DisorderStatSetting();
        UpdateStat();
        UpdateAttackRange();
    }

    public void DisorderStatSetting()
    {
        DisorderStats.ClearStats();
        foreach(var disorder in disorders)
        {
            if(disorder != null)
            {
                DisorderStats.Health += disorder.Health;
                DisorderStats.Stamina += disorder.Stamina;
                DisorderStats.MaxHealth += disorder.MaxHealth;
                DisorderStats.MaxStamina += disorder.MaxStamina;
                DisorderStats.HealthRegen += disorder.HealthRegen;
                DisorderStats.StaminaRegen += disorder.StaminaRegen;
                DisorderStats.Damage += disorder.Damage;
                DisorderStats.Defense += disorder.Defense;
                DisorderStats.MoveSpeed += disorder.MoveSpeed;
            }
        }
    }


    void InitDefaultSprite() {
        _defaultSpriteInfo.Clear();
        int index = 0;
        foreach (var renderer in SpriteParts) {
            PlayerSpriteParts parts = (PlayerSpriteParts)index;
            Sprite s = renderer.sprite;
            Color c = renderer.color;
            bool e = renderer.enabled;
            DefaultSpriteInfo defInfo = new DefaultSpriteInfo()
            {
                part = parts,
                sprite = s,
                color = c,
                enabled = e
            };

            _defaultSpriteInfo.Add(parts, defInfo);

            index++;
        }
    }

    public virtual bool AddItem(ItemModel item, int amount)
    {
        if (item.metaInfo.itemType.Equals(ItemType.Etc) || 
            item.metaInfo.itemType.Equals(ItemType.Normal))
            //소모품과 일반 아이템은 같은 아이템이 인벤토리에 있으면 개수만 늘어남
        {
            if (ItemNames.Contains<string>(item.metaInfo.Name))//이미 인벤토리에 들어가 있으면
            {
                int index = ItemNames.IndexOf(item.metaInfo.Name);
                ItemCounts[index] += amount;
                if (item.metaInfo.Name.Equals("물"))
                {
                    if(ItemCounts[index] > waterMax)
                    {
                        ItemCounts[index] = waterMax;
                    }    
                }                
            }
            else
            {
                if (ItemLists.Count == bagSize)
                {
                    Debug.Log("Item Slot is Full");
                    return false;
                }

                ItemLists.Add(item);
                ItemNames.Add(item.metaInfo.Name);
                if(amount > waterMax)
                {
                    ItemCounts.Add(waterMax);
                }
                else
                {
                    ItemCounts.Add(amount);
                }             
            }
        }

        else
        {
            if (ItemLists.Count == bagSize)
            {
                Debug.Log("Item Slot is Full");
                return false;
            }

            ItemLists.Add(item);
            ItemNames.Add(item.metaInfo.Name);
            ItemCounts.Add(amount);
        }
        return true;
    }

    public void RemoveItemAtIndex(int index)
    {
        if (ItemLists.Count != 0)
        {
            if (ItemLists[index] != null)
            {
                ItemLists.RemoveAt(index);
                ItemNames.RemoveAt(index);
                ItemCounts.RemoveAt(index);
            }
        }
        else
            Debug.Log("Item is Empty");
    }

    public virtual bool RemoveItem(ItemModel item)
    {

        string itemName = item.metaInfo.Name;

        if (ItemNames.Contains(itemName))
        {
            int index = ItemNames.IndexOf(itemName);
            RemoveItemAtIndex(index);
        }
        else
        {
            Debug.Log("Item is Empty");
            return false;
        }

        ItemSpace.Remove(item.instanceId);

        return true;
    }

    public void PrintAllItems()
    {
        Debug.Log("Unit Items");
        foreach (var kv in ItemSpace)
        {
            Debug.Log(kv.Value.instanceId + " : " + kv.Value.metaInfo.ToString());
        }
    }

    public bool HasItem(int itemMetaId) {
        
        foreach(var item in ItemLists)
        {
            if (item.metaInfo.metaId.Equals(itemMetaId))
            {
                return true;
            }
        }
        return false;
    }

    public void PrintItemsInItems()
    {
        Debug.Log("Unit Items");
        foreach (var kv in ItemLists)
        {
            int index = ItemNames.IndexOf(kv.metaInfo.Name);
            Debug.Log("Item Name : " + kv.metaInfo.Name);
            Debug.Log("Amount : " + ItemCounts[index]);
        }
    }

    public List<ItemModel> GetAllItems()
    {
        List<ItemModel> output = new List<ItemModel>(ItemLists);
        return output;
    }

    public List<int> GetAllCounts()
    {
        List<int> output = new List<int>(ItemCounts);
        return output;
    }

    public bool IsDead() {
        return CurrentStats.Health <= 0f;
    }

    public virtual void WearEquipmentNetwork(int id, bool isAcquire) {
        var info = ItemManager.Manager.GetItemTypeInfo(id);
        
        switch (info.itemType) {
            case ItemType.Weapon:
                if (!isAcquire)
                {
                    weaponSlot = null;
                }
                else {
                    var item = ItemManager.Manager.MakeItem(id);
                    weaponSlot = item;
                }
                break;
            case ItemType.Head:
                if (!isAcquire)
                {
                    headSlot = null;
                }
                else {
                    var item = ItemManager.Manager.MakeItem(id);
                    headSlot = item;
                }
                break;
        }

        SpriteUpdate();
    }

    //장비 착용.
    public virtual bool WearEquipment(ItemModel equipment)
    {

        bool result = false;

        ItemType equipType = equipment.metaInfo.itemType;
        if (equipType.Equals(ItemType.Weapon))
        {
            if(weaponSlot == null){
                weaponSlot = equipment;
                AddStats(weaponSlot);
                result = true;

                if (result)
                {
                    if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
                    {
                        NetworkComponents.GameServer.Instance.SendPlayerItemAcquire((int)equipment.metaInfo.metaId, true);
                    }
                }
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("Weapon Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Clothes))
        {
            if (clothesSlot == null)
            {
                clothesSlot = equipment;
                AddStats(clothesSlot);
                result = true;
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("Clothes Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Head))
        {
            if (headSlot == null)
            {
                headSlot = equipment;
                AddStats(headSlot);
                result = true;

                if (result)
                {
                    if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
                    {
                        NetworkComponents.GameServer.Instance.SendPlayerItemAcquire((int)equipment.metaInfo.metaId, true);
                    }
                }
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("Head Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Backpack))
        {
            if (backpackSlot == null)
            {
                backpackSlot = equipment;
                int backpackSize = equipment.GetSize();
                bagSize = backpackSize;
                result = true;
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("BackPack Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Bottle))
        {
            if (bottleSlot == null)
            {
                bottleSlot = equipment;
                waterMax = equipment.GetSize();
                result = true;
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("Bottle Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Tool_Equip))
        {
            if (toolSlot == null)
            {
                toolSlot = equipment;      
                if(toolSlot.GetStaminaRegen() != 0)
                {
                    AddStats(toolSlot);
                }
                if(toolSlot.GetVision() != 0)
                {
                    visionLevel = toolSlot.GetVision();
                }

                if (equipment.metaInfo.metaId.Equals(31005))
                {//마스크 착용시
                    MaskSprite();
                }

                if (equipment.metaInfo.metaId.Equals(31006)|| equipment.metaInfo.metaId.Equals(31007))
                {
                    tempMoveSpeed = CurrentStats.MoveSpeed;
                    MoveRestrict = true;
                }

                result = true;
                SoundLayer.CurrentLayer.PlaySound("se_equip");
            }
            else
            {
                Debug.Log("Tool Slot is full");
            }
        }

        SpriteUpdate();
        return result;
    }

    //장비 제거 
    public virtual void RemoveEquipment(string SlotName)
    {

        if (SlotName.Equals("weapon"))
        {
            if(weaponSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(weaponSlot);
            attack_range_x = default_attack_range_x;
            attack_range_y = default_attack_range_y;

            if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
            {
                NetworkComponents.GameServer.Instance.SendPlayerItemAcquire((int)weaponSlot.metaInfo.metaId, false);
            }
            weaponSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");

        }
        else if (SlotName.Equals("clothes"))
        {
            if (clothesSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(clothesSlot);
            clothesSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");
        }
        else if (SlotName.Equals("head"))
        {
            if (headSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(headSlot);
            if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
            {
                NetworkComponents.GameServer.Instance.SendPlayerItemAcquire((int)headSlot.metaInfo.metaId, false);
            }
            headSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");
        }
        else if (SlotName.Equals("backpack"))
        {
            if (backpackSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            bagSize = defaultBagSize;
            backpackSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");
        }
        else if (SlotName.Equals("bottle"))
        {
            if (bottleSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }
            bottleSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");
        }
        else if (SlotName.Equals("tool"))
        {
            if (toolSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            if (toolSlot.GetVision() != 0)
            {
                visionLevel = 0;
            }
            if (toolSlot.GetStaminaRegen() != 0)
            {
                SubtractStats(toolSlot);
            }

            if (toolSlot.metaInfo.metaId.Equals(31005))
            {
                RemoveMaskSprite();
            }
            if (toolSlot.metaInfo.metaId.Equals(31006) || toolSlot.metaInfo.metaId.Equals(31007))
            {//침낭
                CurrentStats.MoveSpeed = tempMoveSpeed;
                MoveRestrict = false;
            }

            toolSlot = null;
            SoundLayer.CurrentLayer.PlaySound("se_unequip");

        }

        UpdateStat();
        UpdateAttackRange();
        if (CurrentStats.Health > CurrentStats.MaxHealth)
            CurrentStats.Health = CurrentStats.MaxHealth;
        if (CurrentStats.Stamina > CurrentStats.MaxStamina)
            CurrentStats.Stamina = CurrentStats.MaxStamina;

        SpriteUpdate();
    }

    public void DefaultBagSize()
    {
        bagSize = defaultBagSize;
    }

    //장비 착용시 스텟 업데이트
    public void AddStats(ItemModel equip)
    {
        ItemStats.Health += equip.GetHealth();
        CurrentStats.Health += equip.GetHealth();
        ItemStats.Stamina += equip.GetStamina();
        CurrentStats.Stamina += equip.GetStamina();
        ItemStats.Defense  += equip.GetDefense();
        ItemStats.Damage += equip.GetDamage();

        attack_range_x = equip.GetAttacRangeX();
        attack_range_y = equip.GetAttacRangeY();
        ItemStats.HealthRegen += equip.GetHealthRegen();
        ItemStats.StaminaRegen += equip.GetStaminaRegen();

        UpdateStat();
        UpdateAttackRange();
    }

    //장비 제거시 스텟 업데이트
    private void SubtractStats(ItemModel equip)
    {

        ItemStats.Health -= equip.GetHealth();
        ItemStats.Stamina -= equip.GetStamina();
        ItemStats.Defense -= equip.GetDefense();
        ItemStats.Damage -= equip.GetDamage();
        
        if(equip.GetAttacRangeX() != 0)
        {
            attack_range_x = default_attack_range_x;
        }

        if(equip.GetAttacRangeY() != 0)
        {
            attack_range_y = default_attack_range_y;
        }

        ItemStats.HealthRegen -= equip.GetHealthRegen();
        ItemStats.StaminaRegen -= equip.GetStaminaRegen();
        UpdateStat();
        UpdateAttackRange();
    }

    //상태이상 효과 추가
    public void GetDisorder(Disorder.DisorderType type)
    {
        if (ContainDisorder(type))
        {
            Disorder disorder = new Disorder(type);
            
            for(int i = 0; i < disorders.Length; i++)
            {
                if(disorders[i] == null)
                {
                    disorders[i] = disorder;
                    break;
                }
            }
            UpdateStat();

            Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.PlayerGetDisorder, GlobalParameters.Param.accountName, type);
        }
    }

    public void RecoverDisorder(Disorder.DisorderType type)
    {
        if (!ContainDisorder(type))
        {
            for(int i = 0; i < disorders.Length; i++)
            {
                if(disorders[i] != null)
                {
                    if (disorders[i].disorderType.Equals(type))
                    {
                        disorders[i] = null;
                        for (int j = i; j < disorders.Length - 1; j++)
                        {
                            disorders[j] = disorders[j + 1];
                        }

                        disorders[disorders.Length - 1] = null;
                        Notice.Instance.Send(NoticeName.SaveGameLog, GameLogType.PlayerRemoveDisorder, GlobalParameters.Param.accountName, type);
                        break;
                    }
                }
            }
        }

        UpdateStat();
    }

    //true 면 상태이상을 갖지 않은 것
    public bool ContainDisorder(Disorder.DisorderType type)
    {
        bool result = true ;

        for(int i = 0; i < disorders.Length; i++)
        {
            if (disorders[i] != null && disorders[i].disorderType.Equals(type))
            {
                result = false;
                break;
            }
        }
        return result;
    }


    //장비 착용시 Max Stat을 업데이트
    private void UpdateStat()
    {
        DisorderStatSetting();
        CurrentStats.MaxHealth = DefaultStats.Health + ItemStats.Health + DisorderStats.MaxHealth + EventStats.MaxHealth;
        CurrentStats.MaxStamina = DefaultStats.Stamina + ItemStats.Stamina + DisorderStats.MaxStamina + EventStats.MaxStamina;
        CurrentStats.Defense = DefaultStats.Defense + ItemStats.Defense + DisorderStats.Defense + EventStats.Defense;
        if(CurrentStats.Defense < 0)
            CurrentStats.Defense = 0;
        CurrentStats.Damage = DefaultStats.Damage + ItemStats.Damage + DisorderStats.Damage + EventStats.Damage;
        if (CurrentStats.Damage < 5f) {
            CurrentStats.Damage = 5f;
        }
        CurrentStats.HealthRegen = DefaultStats.HealthRegen + ItemStats.HealthRegen + DisorderStats.HealthRegen + EventStats.HealthRegen + RegionItemStats.HealthRegen;
        CurrentStats.StaminaRegen = DefaultStats.StaminaRegen + ItemStats.StaminaRegen + DisorderStats.StaminaRegen + EventStats.StaminaRegen + RegionItemStats.StaminaRegen;
        CurrentStats.MoveSpeed = DefaultStats.MoveSpeed + DisorderStats.MoveSpeed + EventStats.MoveSpeed;
    }

    private void StatRegeneration()
    {
        if(CurrentStats.Stamina == CurrentStats.MaxStamina)
            CurrentStats.Health += CurrentStats.HealthRegen;
        CurrentStats.Stamina += CurrentStats.StaminaRegen;

        if(CurrentStats.Health > CurrentStats.MaxHealth)
        {
            CurrentStats.Health = CurrentStats.MaxHealth;
        }
        if(CurrentStats.Stamina > CurrentStats.MaxStamina)
        {
            CurrentStats.Stamina = CurrentStats.MaxStamina;
        }
    }


    //소모품 사용. HP회복, Stamina회복
    public bool UseExpendables(ItemModel etc)
    {
        bool result = false;

        float etcHealth = etc.GetHealth();
        if (etcHealth != 0f)
        {
            if (PlusHealth(etcHealth))
                result = true;
        }


        float etcStamina = etc.GetStamina();
        if (etcStamina != 0f)
        {
            if (PlusStamina(etcStamina))
                result = true;
        }

        result = SpecialEffect(etc, result);

        UpdateStat();

        return result;
    }

    private void EffectTileEffect(TileUnit currentTile)
    {
        if (EffectTiles.Contains(currentTile))
        {
            
            if (!EffectToken)
            {
                if (EffectItemID.Equals(33001))
                {
                    RegionItemStats.StaminaRegen += 5;                    
                }
                else if(EffectItemID.Equals(33002))
                {
                    RegionItemStats.StaminaRegen += 3;
                }
                else
                {
                    //피뢰침 효과
                }
                UpdateStat();
            }
            EffectToken = true;
        }
        else
        {
            RegionItemStats.StaminaRegen = 0;
            UpdateStat();

            EffectToken = false;
        }
    }

    //특수효과 처리 
    private bool SpecialEffect(ItemModel etc, bool result)
    {
        if (etc.metaInfo.metaId.Equals(33001))
        {//텐트
            EffectItemID = etc.metaInfo.metaId;
            TileUnit currentTile = GetPlayerModel().GetCurrentTile();
        
            var item = ItemManager.Manager.MakeDropItem(33001, currentTile);
            if (EffectTiles.Contains(currentTile))
            {
                InGameUIScript.Instance.Notice("아이템 사용", "근처에 이미 다른 아이템이 설치되어 있습니다.");
                result = false;
                return result;
            }

            item.ItemRenderer.transform.localScale *= 7;

            item.isRegionEffect = true;
            result = true;

            for (int i = -5; i <= 5; i++)
            {
                int x = currentTile.x + i;

                for (int j = -5; j <= 5; j++)
                {
                    int y = currentTile.y + j;
                    TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);
                    if (tile == null) continue;
                    else
                        EffectTiles.Add(tile);
                }
            }

            SoundLayer.CurrentLayer.PlaySound("se_tent");
        }
        else if (etc.metaInfo.metaId.Equals(33002))
        {//모닥불
            EffectItemID = etc.metaInfo.metaId;            
            TileUnit currentTile = GetPlayerModel().GetCurrentTile();
            if (EffectTiles.Contains(currentTile))
            {
                InGameUIScript.Instance.Notice("아이템 사용", "근처에 이미 다른 아이템이 설치되어 있습니다.");
                result = false;
                return result;
            }
            var item = ItemManager.Manager.MakeDropItem(33002, currentTile);
            item.ItemRenderer.transform.localScale *= 1.5f;
            item.isRegionEffect = true;
            result = true;
       
            for (int i = -5; i <= 5; i++)
            {
                int x = currentTile.x + i;

                for (int j = -5; j <= 5; j++)
                {
                    int y = currentTile.y + j;
                    TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);
                    if (tile == null) continue;
                    else
                        EffectTiles.Add(tile);
                }
            }

            SoundLayer.CurrentLayer.PlaySound("se_bonfire");
        }
        else if (etc.metaInfo.metaId.Equals(33003))
        {//피뢰침
            EffectItemID = etc.metaInfo.metaId;
            TileUnit currentTile = GetPlayerModel().GetCurrentTile();
            if (EffectTiles.Contains(currentTile))
            {
                InGameUIScript.Instance.Notice("아이템 사용", "근처에 이미 다른 아이템이 설치되어 있습니다.");
                result = false;
                return result;
            }
            var item = ItemManager.Manager.MakeDropItem(33003, currentTile);
            item.ItemRenderer.transform.localScale *= 1.5f;
            item.isRegionEffect = true;
            result = true;

            for (int i = -5; i <= 5; i++)
            {
                int x = currentTile.x + i;

                for (int j = -5; j <= 5; j++)
                {
                    int y = currentTile.y + j;
                    TileUnit tile = RandomMapGenerator.Instance.GetTile(x, y);
                    if (tile == null) continue;
                    else
                        EffectTiles.Add(tile);
                }
            }

            //SoundLayer.CurrentLayer.PlaySound("se_bonfire");
        }

        if (etc.metaInfo.metaId.Equals(41001))
        {//약 특수효과
            RevoerDisorderByType(Disorder.DisorderType.poisoning);
            result = true;

            SoundLayer.CurrentLayer.PlaySound("se_eatitem");
        }
        else if (etc.metaInfo.metaId.Equals(41002))
        {//구급상자 특수효과
            RevoerDisorderByType(Disorder.DisorderType.injury);
            result = true;
            SoundLayer.CurrentLayer.PlaySound("se_eatitem");
        }
        else if (etc.metaInfo.metaId.Equals(40001) || etc.metaInfo.metaId.Equals(40002))
        {//물 특수효과
            RevoerDisorderByType(Disorder.DisorderType.thirst);
            result = true;
            SoundLayer.CurrentLayer.PlaySound("se_eatitem");
        }
        else if (etc.metaInfo.metaId.Equals(40003))
        {//생고기 특수효과
            System.Random random = new System.Random();
            int randomInt = random.Next(0, 100);
            if (randomInt < 25)
                GetDisorder(Disorder.DisorderType.poisoning);
            RevoerDisorderByType(Disorder.DisorderType.hunger);
            result = true;
        }
        else if (etc.metaInfo.metaId.Equals(41003))
        {//음식 특수효과
            RevoerDisorderByType(Disorder.DisorderType.hunger);
            result = true;
        }
        else
            ;

        return result;
    }

    private void RevoerDisorderByType(Disorder.DisorderType Disordertype)
    {
        foreach (var disorder in disorders)
        {
            if (disorder != null)
            {
                if (disorder.disorderType.Equals(Disordertype))
                {
                    RecoverDisorder(disorder.disorderType);
                    break;
                }
            }
        }
    }

    public void OnDie() {
        PlayerMoveController ctrl = gameObject.GetComponent<PlayerMoveController>();
        Animator anim = ctrl.PlayerMovementCTRL;
        AnimatorUtil.SetTrigger(anim, "Dead");
        if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi) {
            NetworkComponents.GameServer.Instance.SendPlayerAnimTrigger("Dead");
        }

        PlayerAttackController attackController = gameObject.GetComponent<PlayerAttackController>();
        if (attackController != null) {
            attackController.enabled = false;
        }

        attackReceiver.gameObject.SetActive(false);
        ctrl.enabled = false;

        var script = Camera.main.gameObject.GetComponent<ScreenCapture>();
        if (script != null) {
            script.Capature(1f);
        }
        isPlayerDead = true;
        
        InGameUIScript.Instance.PlayerDeadPanelOn(HasItem(33004));
    }

    public void AEDContain()
    {

    }

    //HP 감소
    public void SubtractHealth(float weight)
    {
        if (IsDead()) {
            return;
        }
        CurrentStats.Health -= weight;

        if (CurrentStats.Health <= 0f)
        {
            CurrentStats.Health = 0f;
            OnDie();
        }
        SendHealthState();
    }

    void SendHealthState() {
        if (GlobalGameManager.Instance.GameNetworkType != GameNetworkType.Multi) return;
        NetworkComponents.GameServer.Instance.SendPlayerStateInfo((int)CurrentStats.Health, CurrentStats.Health <= 0f);

    }

    //소모품 사용시 체력 회복. MaxHealth 이상은 회복되지 않음
    public bool PlusHealth(float weight)
    {
        bool result = false;
        if(CurrentStats.Health < CurrentStats.MaxHealth)
        {
            CurrentStats.Health += weight;
            if(CurrentStats.Health >= CurrentStats.MaxHealth)
            {
                CurrentStats.Health = CurrentStats.MaxHealth;
            }
            result = true;
        }
        SendHealthState();
        return result;
    }

    //Stamina 감소
    public void SubtractStamina(float weight)
    {
        CurrentStats.Stamina -= weight;

        if (CurrentStats.Stamina <= 0f)
        {
            CurrentStats.Stamina = 0f;
        }
    }

    //아이템 사용시 Stamina회복. MaxStamina 이상은 회복 안됨
    public bool PlusStamina(float weight)
    {
        bool result = false;
        if (CurrentStats.Stamina < CurrentStats.MaxStamina)
        {
            CurrentStats.Stamina += weight;
            if (CurrentStats.Stamina >= CurrentStats.MaxStamina)
            {
                CurrentStats.Stamina = CurrentStats.MaxStamina;
            }
            result = true;
        }
        return result;
    }

    public void SpriteUpdate()
    {
        HeadSprite();
        WeaponSprite();
    }

    public void UpdateAttackRange() {
        var attackCTRL = gameObject.GetComponent<PlayerAttackController>();
        if (attackCTRL) {
            attackCTRL.SetAttackRange(attack_range_x, attack_range_y);
        }
    }

    public void WeaponSprite()
    {

        if (weaponSlot != null)
        {
            SetSprite(PlayerSpriteParts.FrontWeapon, weaponSlot.metaInfo);
        }
        else {
            ClearSprite(PlayerSpriteParts.FrontWeapon);
        }
    }


    public void HeadSprite()
    {
        if (headSlot != null)
        {

            string src = headSlot.metaInfo.spriteSrc;
            Sprite s = Resources.Load<Sprite>(src);


            SpriteParts[9].sprite = s;
            if (SpriteParts[9].enabled == false) {
                SpriteParts[9].enabled = true;
            }
            SpriteParts[14].sprite = null;
            SpriteParts[15].sprite = null;
        }
        /*else
        {
            SpriteParts[9].sprite = null;
            SpriteParts[14].sprite = BackHair;
            SpriteParts[15].sprite = Tail;

            //string src = headSlot.metaInfo.spriteSrc;
            //Sprite s = Resources.Load<Sprite>(src);
            
            SetSprite(PlayerSpriteParts.HeadOrnament, headSlot.metaInfo);
            DisableSpritePart(PlayerSpriteParts.BackOrnament);
            DisableSpritePart(PlayerSpriteParts.Tail);
            //SpriteParts[9].sprite = s;
            //SpriteParts[14].color = Color.clear;
            //SpriteParts[15].color = Color.clear;
        }
        */
       else
        {
            ClearSprite(PlayerSpriteParts.HeadOrnament);
            ClearSprite(PlayerSpriteParts.BackHair);
            ClearSprite(PlayerSpriteParts.Tail);
            //SpriteParts[9].sprite = null;
            //SpriteParts[14].color = Color.white;
            //SpriteParts[15].color = Color.white;
        }
    }

    /// <summary>
    /// 색상 정보도 필요할 수 있음
    /// </summary>
    /// <param name="part"></param>
    /// <param name="info"></param>
    public void SetSprite(PlayerSpriteParts part, ItemTypeInfo info) {
        Sprite sprite = Resources.Load<Sprite>(info.spriteSrc);
        if (sprite == null)
        {
            ClearSprite(part);
            return;
        }
        else {
            SetSpritePart(part, sprite);
        }
    }

    void ClearSprite(PlayerSpriteParts part) {
        try {
            var rend = SpriteParts[(int)part];
            DefaultSpriteInfo def;
            if (_defaultSpriteInfo.TryGetValue(part, out def))
            {
                rend.enabled = def.enabled;
                rend.sprite = def.sprite;
                rend.color = def.color;
            }
            else {
                rend.enabled = false;
            }
        }
        catch {

        }
    }

    void DisableSpritePart(PlayerSpriteParts part) {
        try
        {
            var rend = SpriteParts[(int)part];
            rend.enabled = false;
        }
        catch { }
    }

    void SetSpritePart(PlayerSpriteParts part, Sprite s) {
        try
        {
            var rend = SpriteParts[(int)part];
            rend.enabled = true;
            rend.sprite = s;
            //color info?
        }
        catch (IndexOutOfRangeException ioe)
        {
            Debug.LogError("Could not find Player's sprite region " + part);
            return;
        }
        catch (NullReferenceException ne) {
            
        }
    }

    void SetSpritePartColor(PlayerSpriteParts part, Color c) {
        try
        {
            var rend = SpriteParts[(int)part];
            rend.color = c;
        }
        catch {

        }
    }       

    public void MaskSprite()
    {
        string src = "item/sample/mask0";
        Sprite s = Resources.Load<Sprite>(src);

        SpriteParts[11].sprite = s;
        SpriteParts[11].color = Color.white;
        SpriteParts[11].transform.gameObject.SetActive(true);
    }

    public void RemoveMaskSprite()
    {
        SpriteParts[11].sprite = null;
        SpriteParts[11].color = Color.clear;
        SpriteParts[11].transform.gameObject.SetActive(false);
    }

    public int GetReservedItemCount(ItemModel item)
    {
        for(int i = 0; i < ItemLists.Count; i++)
        {
            if(ItemLists[i] != null)
            {
                ItemModel _item = ItemLists[i];
                if (_item.metaInfo.Name == item.metaInfo.Name)
                    return ItemCounts[i];
            }
        }

        return 0;
    }

    public void SetSpeedFactor(float value = 1f) {
        SpeedFactor = value;
    }

    public void RetryGame()
    {
        isPlayerDead = false;
        CurrentStats.Health = CurrentStats.MaxHealth;
        CurrentStats.Stamina = CurrentStats.MaxStamina;
    }


    public class Stats
    {
        public float MaxHealth = 0.0f;
        public float MaxStamina = 0.0f;
        public float Health = 0.0f;
        public float Stamina = 0.0f;
        public float Defense = 0.0f;
        public float Damage = 0.0f;
        public float HealthRegen = 0;
        public float StaminaRegen = 0;
        public float MoveSpeed = 0;

        public Stats()
        {
            MaxHealth = 0.0f;
            MaxStamina = 0.0f;
            Health = 0.0f;
            Stamina = 0.0f;
            Defense = 0.0f;
            Damage = 0.0f;
            HealthRegen = 0.0f;
            StaminaRegen = 0.0f;
            MoveSpeed = 0.0f;
        }

        public void ClearStats()
        {
            MaxHealth = 0.0f;
            MaxStamina = 0.0f;
            Health = 0.0f;
            Stamina = 0.0f;
            Defense = 0.0f;
            Damage = 0.0f;
            HealthRegen = 0.0f;
            StaminaRegen = 0.0f;
            MoveSpeed = 0.0f;
        }
    }

    public class RegionEffectItem : DropItem
    {
        new
        public void OnTriggerEnter2D(Collider2D collision)
        {
            ;
        }
    }



}
