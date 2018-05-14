using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ItemManager {
    public class ItemManagement {
        public long MetaId = 0;
        public Dictionary<long, ItemModel> items = new Dictionary<long, ItemModel>();

        public void AddItem(ItemModel item) {
            items.Add(item.instanceId, item);
        }

        public bool GetItem(long instId, out ItemModel output) {
            return items.TryGetValue(instId, out output);
        }
        
    }

    private static ItemManager _manager = null;
    public static ItemManager Manager
    {
        get
        {
            if (_manager == null) _manager = new ItemManager();
            return _manager;
        }
    }

    long instanceId = 0;

    /// <summary>
    /// 아이템 관리용 딕셔너리 (instanceId, itemModel)
    /// </summary>
    private Dictionary<long, ItemModel> _managementDic = new Dictionary<long, ItemModel>();

    /// <summary>
    /// 메타 정보 저장용 딕셔너리 (metaId, itemTypeInfo);
    /// </summary>
    private Dictionary<long, ItemTypeInfo> _typeInfoDic = new Dictionary<long, ItemTypeInfo>();

    ItemManager() {
        //Load Static Info
        
    }

    public void OnGameInitialized() {
        instanceId = 0;
    }

    public void InitTypeInfoList(List<ItemTypeInfo> infos) {
        _typeInfoDic.Clear();
        foreach (var info in infos) {
            _typeInfoDic.Add(info.metaId, info);
        }

        //LogExistItems();
    }

    public void LogExistItems() {
        foreach (var info in _typeInfoDic) {
            UnityEngine.Debug.Log(info.ToString());
            
        }
    }

    public static void Log(string desc, bool isError = false) {
#if UNITY_EDITOR
        if (isError)
        {
            UnityEngine.Debug.LogError("[Item Management Error] " + desc);
        }
        else {
            UnityEngine.Debug.Log("[Item Management] " + desc);
        }
#endif
    }

    public ItemModel MakeItem(long metaId) {
        ItemTypeInfo meta = null;
        
        if (!_typeInfoDic.TryGetValue(metaId, out meta)) {
            //log error : Couldn't found item info
            Log(metaId.ToString(), true);
            return null;
        }
        ItemModel output = new ItemModel() {
            metaInfo = meta,
            instanceId = instanceId++
        };
        return output;
    }

    public void AddItem(UnitModel target, long itemMetaId, int amount) {
        var item = MakeItem(itemMetaId);
        if (item != null) {
            AddItem(target, item, amount);
        }
    }

    /// <summary>
    /// 아이템을 보유할 대상에게 제공합니다.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="item"></param>
    public void AddItem(UnitModel owner, ItemModel item, int amount)
    {
        //check item movable state

        if (owner.AddItem(item, amount)) {
            item.OnItemAqquired(owner);
        }
    }

    /// <summary>
    /// 아이템을 소지자에게서 제거합니다.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="item"></param>
    public void RemoveItem(UnitModel owner, ItemModel item) {
        if (owner.RemoveItem(item)) {
            item.OnItemRemoved(owner);
        }
    }
}
