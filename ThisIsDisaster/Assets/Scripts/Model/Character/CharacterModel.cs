using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CharacterModel : UnitModel {

    public float defaultHealth = 100.0f;
    public float defaultStamina = 100.0f;
    public float defaultDefense = 10.0f;
    public float defaultDamage = 10.0f;

    public float maxHealth = 0.0f;
    public float maxStamina = 0.0f;
    public float maxDefense = 0.0f;
    public float maxDamage = 0.0f;

    public float health = 0.0f;
    public float stamina = 0.0f;
    public float defense = 0.0f;
    public float damage = 0.0f;
    
    public float itemHealth = 0.0f;
    public float itemStamina = 0.0f;
    public float itemDefense = 0.0f;
    public float itemDamage =0.0f;

    public ItemModel headSlot = null;
    public ItemModel weaponSlot = null;
    public ItemModel utilSlot1 = null;
    public ItemModel utilSlot2 = null;
    public ItemModel utilSlot3 = null;

    public virtual void initialState()
    {
        UpdateStat();

        health = maxHealth;
        stamina = maxStamina;
        defense = maxDefense;
        damage = maxDamage;
    }

    public virtual void PrintStats()
    {
        UnityEngine.Debug.Log(MaxStatsToString());
        UnityEngine.Debug.Log(CurrentStatsToString());

    }

    public virtual string MaxStatsToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Max Stats : ");
        builder.Append(maxHealth);
        builder.Append(" , ");
        builder.Append(maxStamina);
        builder.Append(" , ");
        builder.Append(maxDefense);
        builder.Append(" , ");
        builder.Append(maxDamage);
        builder.AppendLine();
        string output = builder.ToString();

        return output;
        
    }


    public virtual string CurrentStatsToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Current Stats : ");
        builder.Append(health);
        builder.Append(" , ");
        builder.Append(stamina);
        builder.Append(" , ");
        builder.Append(defense);
        builder.Append(" , ");
        builder.Append(damage);
        builder.AppendLine();
        string output = builder.ToString();

        return output;

    }

    public virtual void WearEquipment(ItemModel equipment)
    {
        ItemType equipType = equipment.metaInfo.itemType;

        if (equipType.Equals(ItemType.Weapon))
        {
            if (weaponSlot == null)
            {
                weaponSlot = equipment;
                AddStats(equipment);
            }
            else
            {
                UnityEngine.Debug.Log("Weapon is already worn");
            }//무기 슬롯 착용중
        }
        else if (equipType.Equals(ItemType.Head))
        {
            if (headSlot == null)
            {
                headSlot = equipment;
                AddStats(equipment);
            }
            else
            {
                UnityEngine.Debug.Log("Head is already worn");
            }//머리 슬롯 착용중
        }
        else if (equipType.Equals(ItemType.Util))
        {
            if (utilSlot1 == null)
            {
                utilSlot1 = equipment;
                AddStats(equipment);
            }
            else if (utilSlot2 == null)
            {
                utilSlot2 = equipment;
                AddStats(equipment);
            }
            else if (utilSlot3 == null)
            {
                utilSlot3 = equipment;
                AddStats(equipment);
            }
            else
            {

            }//유틸 슬롯 풀
        }        
    }

    public virtual void RemoveEquipment(ItemModel Slot)
    {

        ItemType slotType = Slot.metaInfo.itemType;
        

        if (slotType.Equals(ItemType.Weapon))
        {
            weaponSlot = null;
            SubtractStats(Slot);
        }
        else if (slotType.Equals(ItemType.Head))
        {
            headSlot = null;
            SubtractStats(Slot);
        }
        else if (slotType.Equals(ItemType.Util))
        {
            //
        }
    }


    public void AddStats(ItemModel equip)
    {
        itemHealth += equip.GetHealth();
        //itemStamina += equip.GetStamina();
        itemDefense += equip.GetDefense();
        itemDamage += equip.GetDamage();

        UpdateStat();
    }

    private void SubtractStats(ItemModel equip)
    {
        itemHealth -= equip.GetHealth();
        //itemStamina -= equip.GetStamina();
        itemDefense -= equip.GetDefense();
        itemDamage -= equip.GetDamage();

        UpdateStat();
    }


    private void UpdateStat()
    {
        maxHealth = defaultHealth + itemHealth;
        maxStamina = defaultStamina + itemStamina;
        maxDefense = defaultDefense + itemDefense;
        maxDamage = defaultDamage + itemDamage;
    }

    private void UseExpendables(ItemModel etc)
    {

    }


    public void WoundHealth(float weight)
    {
        health -= weight;

        if(health <= 0f)
        {
            health = 0f;
            Debug.Log("Player Died");
        }
    }

    public void HealHealth(float weight)
    {
        if(health < maxHealth)
        {
            health += weight;

            if(health >= maxHealth)
            {
                health = maxHealth;
            }
        }
        else
        {
            Debug.Log("HP is Full");
        }

    }


}
