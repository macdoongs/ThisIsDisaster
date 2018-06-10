using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 아이템의 종류 -> 장비 혹은 소모품
/// </summary>
public enum ItemType
{
    /// <summary>
    /// 무기
    /// </summary>
    Weapon = 0,
    /// <summary>
    /// 옷
    /// </summary>
    Clothes =1 ,
    /// <summary>
    /// 모자
    /// </summary>
    Head = 2,
    /// <summary>
    /// 가방
    /// </summary>
    Backpack=3,    
    /// <summary>
    /// 물병
    /// </summary>
    Bottle=4,
    /// <summary>
    /// 전등
    /// </summary>
    Tool_Equip=5,

    Tool_Use=6,


    /// <summary>
    /// 기타 소모품
    /// </summary>
    Etc =7,

    Normal=8

}

public class ItemTypeInfo
{
    public long metaId = 0;

    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string Name;

    /// <summary>
    /// 소지가능한 최대 개수 : 장비의 경우 1개, 소모품의 경우 _defaultMaxCount를 반환
    /// </summary>
    public int MaxCount
    {
        get { return itemType == ItemType.Etc ? _defaultMaxCount : 1; }
    }

    private int _defaultMaxCount = 0;

    /// <summary>
    /// 아이템의 타입 - 장비, 소모품
    /// </summary>
    public ItemType itemType = ItemType.Weapon;

    public string spriteSrc = "";

    public string description = "";

    public Dictionary<string, float> stats = new Dictionary<string, float>();

    public ItemRareness rareness = ItemRareness.Middle;

    public List<string> tags = new List<string>();

    public static ItemType ParseType(string typeText) {
        switch (typeText.ToLower()) {
            case "head": return ItemType.Head;
            case "clothes": return ItemType.Clothes;
            case "weapon": return ItemType.Weapon;
            case "backpack": return ItemType.Backpack;
            case "bottle": return ItemType.Bottle;
            case "tool_equip": return ItemType.Tool_Equip;
            case "tool_use": return ItemType.Tool_Use;
            case "etc": return ItemType.Etc;
            default: return ItemType.Normal;
        }
    }

    public ItemTypeInfo(long metaId, string name, int maxCount, ItemType type, string description, string[] tags) {
        this.metaId = metaId;
        this.Name = name;
        this._defaultMaxCount = maxCount;
        this.itemType = type;
        this.description = description;

        if (tags.Length > 0)
            this.tags.AddRange(tags);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} : {1} : {2}", metaId, itemType, Name);
        foreach (var kv in stats)
        {
            builder.AppendLine();
            builder.AppendFormat("   {0}:{1}", kv.Key, kv.Value);
        }


        return builder.ToString();
    }
}
