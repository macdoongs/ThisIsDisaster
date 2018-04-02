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
    /// 장비 - 단일 존재 개체
    /// </summary>
    EQUIPMENT,

    /// <summary>
    /// 소모품 - 다수 소지 가능
    /// </summary>
    EXPENDABLES
}

public class ItemTypeInfo
{
    /// <summary>
    /// 아이템 이름
    /// </summary>
    public string Name;

    /// <summary>
    /// 소지가능한 최대 개수 : 장비의 경우 1개, 소모품의 경우 _defaultMaxCount를 반환
    /// </summary>
    public int MaxCount
    {
        get { return itemType == ItemType.EQUIPMENT ? 1 : _defaultMaxCount; }
    }

    private int _defaultMaxCount = 0;

    /// <summary>
    /// 아이템의 타입 - 장비, 소모품
    /// </summary>
    public ItemType itemType = ItemType.EQUIPMENT;

    //smth else info
}
