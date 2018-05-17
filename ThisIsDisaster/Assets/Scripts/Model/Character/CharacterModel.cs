using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 임시
/// </summary>
public class PlayerModel : UnitModel
{
    public CharacterModel _character;

    public override float GetAttackDamage()
    {
        return _character.damage;
    }

    public override string GetUnitName()
    {
        return _character.PlayerName;
    }

    public override void OnTakeDamage(UnitModel attacker, float damage)
    {
        Debug.Log(GetUnitName() + " Attacked By " + attacker.GetUnitName());
    }
}

public class CharacterModel : MonoBehaviour
{
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
    
    //테스트용 값
    public string PlayerName = "TestID";
    public string PlayerLevel = "123";

    //가방(인벤토리) 사이즈. 
    //util 아이템에 따라 사이즈 증가 가능하게 구현할 예정
    public int defaultBagSize = 30;
    
    //캐릭터 기본 스텟
    public float defaultHealth = 100.0f;
    public float defaultStamina = 100.0f;
    public float defaultDefense = 10.0f;
    public float defaultDamage = 10.0f;

    //캐릭터 맥스 스텟. 
    //맥스스텟 = 기본 스텟 + 아이템으로 증가하는 스텟
    public float maxHealth = 0.0f;
    public float maxStamina = 0.0f;

    //캐릭터 현재 스텟
    public float health = 0.0f;
    public float stamina = 0.0f;
    public float defense = 0.0f;
    public float damage = 0.0f;
    
    //아이템으로 증가하는 스텟
    public float itemHealth = 0.0f;
    public float itemStamina = 0.0f;
    public float itemDefense = 0.0f;
    public float itemDamage =0.0f;

    //아이템 착용 슬롯
    public ItemModel headSlot = null;
    public ItemModel weaponSlot = null;
    public ItemModel utilSlot1 = null;
    public ItemModel utilSlot2 = null;
    public ItemModel utilSlot3 = null;

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

        InitDefaultSprite();
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
        //   ItemModel oldData = null;

        if (item.metaInfo.itemType.Equals(ItemType.Etc))//소모품은 이미 인벤토리에 있으면 개수만 늘어나야함
        {
            if (ItemNames.Contains<string>(item.metaInfo.Name))//이미 인벤토리에 들어가 있으면
            {
                int index = ItemNames.IndexOf(item.metaInfo.Name);
                ItemCounts[index] += amount;
            }
            else
            {
                if (ItemLists.Count == 30)
                {
                    Debug.Log("Item Slot is Full");
                    return false;
                }

                ItemLists.Add(item);
                ItemNames.Add(item.metaInfo.Name);
                ItemCounts.Add(amount);
            }
        }

        else
        {
            if (ItemLists.Count == 30)
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

    //초기화 
    public virtual void initialState()
    {

        UpdateStat();

        health = maxHealth;
        stamina = maxStamina;
        BackHair = SpriteParts[14].sprite;
        Tail = SpriteParts[15].sprite;

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
            }
            else
            {
                Debug.Log("Weapon Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Head))
        {
            if (headSlot == null)
            {
                headSlot = equipment;
                AddStats(headSlot);
                result = true;
            }
            else
            {
                Debug.Log("Head Slot is full");
            }
        }
        else if (equipType.Equals(ItemType.Util))
        {
            if (utilSlot1 == null)
            {
                utilSlot1 = equipment;
                AddStats(utilSlot1);
                result = true;
            }
            else if (utilSlot2 == null)
            {
                utilSlot2 = equipment;
                AddStats(utilSlot2);
                result = true;
            }
            else if (utilSlot3 == null)
            {
                utilSlot3 = equipment;
                AddStats(utilSlot3);
                result = true;
            }
            else
            {
                UnityEngine.Debug.Log("All UtilSlot is full");
            }//유틸 슬롯 풀
        }
        SpriteUpdate();
        return result;
    }
    
    //장비 착용. 착용할 슬롯과 아이템 모델을 변수로 
    public virtual void WearSlot(ItemModel Slot, ItemModel equipment)
    {
        if(Slot == null)
        {
            Slot = equipment;
            AddStats(Slot);
        }
        else
        {
            UnityEngine.Debug.Log("Slot is already full");
        }
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
            weaponSlot = null;
        }
        else if (SlotName.Equals("head"))
        {
            if (headSlot == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(headSlot);
            headSlot = null;
        }
        else if (SlotName.Equals("util1"))
        {
            if (utilSlot1 == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }
            SubtractStats(utilSlot1);
            utilSlot1 = null;
        }
        else if (SlotName.Equals("util2"))
        {
            if (utilSlot2 == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(utilSlot2);
            utilSlot2 = null;
        }
        else if (SlotName.Equals("util3"))
        {
            if (utilSlot3 == null)
            {
                Debug.Log("Slot is Empty");
                return;
            }

            SubtractStats(utilSlot3);
            utilSlot3 = null;
        }

        UpdateStat();

        if (health > maxHealth)
            health = maxHealth;
        if (stamina > maxStamina)
            stamina = maxStamina;

        SpriteUpdate();
    }

    //장비 착용시 스텟 업데이트
    public void AddStats(ItemModel equip)
    {
        itemHealth += equip.GetHealth();
        health += equip.GetHealth();
        itemStamina += equip.GetStamina();
        stamina += equip.GetStamina();
        itemDefense += equip.GetDefense();
        itemDamage += equip.GetDamage();

        UpdateStat();
    }

    //장비 제거시 스텟 업데이트
    private void SubtractStats(ItemModel equip)
    {
        itemHealth -= equip.GetHealth();
        itemStamina -= equip.GetStamina();
        itemDefense -= equip.GetDefense();
        itemDamage -= equip.GetDamage();

        UpdateStat();
    }
    
    //장비 착용시 Max Stat을 업데이트
    private void UpdateStat()
    {
        maxHealth = defaultHealth + itemHealth;
        maxStamina = defaultStamina + itemStamina;
        defense = defaultDefense + itemDefense;
        damage = defaultDamage + itemDamage;
    }

    //소모품 사용. HP회복, Stamina회복
    public bool UseExpendables(ItemModel etc)
    {
        bool result = false;

        float etcHealth = etc.GetHealth();
        if(etcHealth != 0f)
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

        UpdateStat();


        return result;
    }

    //HP 감소
    public void SubtractHealth(float weight)
    {
        health -= weight;

        if(health <= 0f)
        {
            health = 0f;
            Debug.Log("Player Died");
        }
    }

    //소모품 사용시 체력 회복. MaxHealth 이상은 회복되지 않음
    public bool PlusHealth(float weight)
    {
        bool result = false;
        if(health < maxHealth)
        {
            health += weight;
            if(health >= maxHealth)
            {
                health = maxHealth;
            }
            result = true;
        }
        else
        {
            Debug.Log("HP is Full");
        }

        return result;
    }

    //Stamina 감소
    public void SubtractStamina(float weight)
    {
        stamina -= weight;

        if (stamina <= 0f)
        {
            stamina = 0f;
            Debug.Log("Stamina is 0");
        }
    }

    //아이템 사용시 Stamina회복. MaxStamina 이상은 회복 안됨
    public bool PlusStamina(float weight)
    {
        bool result = false;
        if (stamina < maxStamina)
        {
            stamina += weight;
            if (stamina >= maxStamina)
            {
                stamina = maxStamina;
            }
            result = true;
        }
        else
        {
            Debug.Log("Stamina is Full");
        }

        return result;
    }

    public void SpriteUpdate()
    {
        HeadSprite();
        WeaponSprite();
        UtilSprite();
    }


    /// <summary>
    /// This is Odd
    /// </summary>
    public void WeaponSprite()
    {
        //if (weaponSlot != null)
        //{
        //    string src = weaponSlot.metaInfo.spriteSrc;
        //    Sprite s = Resources.Load<Sprite>(src);


        //    SpriteParts[7].sprite = s;
        //}
        //else
        //{
        //    //SpriteParts[7].sprite = null;
        //    ClearSpritePart(PlayerSpriteParts.Head);
        //}
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

    public void UtilSprite()
    {
        ItemModel[] Utils = new ItemModel[] { utilSlot1, utilSlot2, utilSlot3 };

        foreach(var util in Utils)
        {
            if (util != null && util.metaInfo.tags.Contains("backpack"))
            {
                //string src = util.metaInfo.spriteSrc;
                //Sprite s = Resources.Load<Sprite>(src);


                //SpriteParts[4].sprite = s;
                SetSprite(PlayerSpriteParts.BackOrnament, util.metaInfo);

            }
            else
            {
                //SpriteParts[4].sprite = null;
                //여기 백팩 체크과정 필요
                ClearSprite(PlayerSpriteParts.BackOrnament);

            }
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
}
