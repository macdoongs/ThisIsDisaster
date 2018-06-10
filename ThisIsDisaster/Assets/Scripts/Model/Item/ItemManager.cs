using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ItemRareness {
    Low,
    Middle,
    High
}

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

    /// <summary>
    /// Rareness prob
    /// </summary>
    private const float _lowProb = 0.5f;
    private const float _midProb = 0.3f;
    private const float _highProb = 0.1f;

    long instanceId = 0;

    /// <summary>
    /// 아이템 관리용 딕셔너리 (instanceId, itemModel)
    /// </summary>
    private Dictionary<long, ItemModel> _managementDic = new Dictionary<long, ItemModel>();

    /// <summary>
    /// 메타 정보 저장용 딕셔너리 (metaId, itemTypeInfo);
    /// </summary>
    private Dictionary<long, ItemTypeInfo> _typeInfoDic = new Dictionary<long, ItemTypeInfo>();

    private Dictionary<long, MixtureRecipe> _recipeDic = new Dictionary<long, MixtureRecipe>();

    private Dictionary<ItemRareness, List<ItemTypeInfo>> _rareDic = new Dictionary<ItemRareness, List<ItemTypeInfo>>();

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
            List<ItemTypeInfo> list = null;
            if (!_rareDic.TryGetValue(info.rareness, out list)) {
                list = new List<ItemTypeInfo>();
                _rareDic.Add(info.rareness, list);
            }
            list.Add(info);
        }

        //LogExistItems();
    }

    public void InitRecipeList(List<MixtureRecipe> infos)
    {
        _recipeDic.Clear();
        foreach (var info in infos)
        {
            _recipeDic.Add(info.resultID, info);
        }

        //LogExistItems();
    }

    public void LogExistItems() {
        foreach (var info in _typeInfoDic) {
            UnityEngine.Debug.Log(info.ToString());
            
        }
    }

    public ItemTypeInfo GetItemTypeInfo(int id) {
        ItemTypeInfo output = null;
        _typeInfoDic.TryGetValue(id, out output);
        return output;
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

    public void AddItem(CharacterModel target, long itemMetaId, int amount) {
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
    public void AddItem(CharacterModel owner, ItemModel item, int amount)
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
    public void RemoveItem(CharacterModel owner, ItemModel item) {
        if (owner.RemoveItem(item)) {
            item.OnItemRemoved(owner);
        }
    }

    public bool OnTryAcquireItem(DropItem item, PlayerModel player) {
        if (!item.isRegionEffect)
        {
            UnityEngine.Debug.Log("player try to acquire item " + item.ItemModel.metaInfo.Name);
            if (player._character.AddItem(item.ItemModel, 1))
            {
                return true;
            }
        }
        return false;
    }

    public DropItem MakeDropItem(int metaId, TileUnit curTile) {
        var item = MakeItem(metaId);
        if (item == null) return null;
        else {
            DropItem output = ItemLayer.CurrentLayer.MakeDropItem(item);
            output.SetTile(curTile);
            return output;
        }
    }

    public List<MixtureRecipe> ContainRecipe(ItemModel item)
    {
        long itemId = item.metaInfo.metaId;
        List<MixtureRecipe> recipeList = new List<MixtureRecipe>();
        foreach(var _recipe in _recipeDic)
        {
            MixtureRecipe recipe = _recipe.Value;
            foreach(var matID in recipe.MaterialID)
            {
                if (matID == itemId)
                {
                    recipeList.Add(recipe);
                    break;
                }
            }
        }

        return recipeList;
    }

    public ItemTypeInfo GetRandomItemByRare() {
        ItemRareness rareness = ItemRareness.Middle;
        float randVal = UnityEngine.Random.Range(0f, _lowProb + _midProb + _highProb);
        if (randVal <= _lowProb)
        {
            rareness = ItemRareness.Low;
        }
        else if (randVal <= _midProb)
        {
            rareness = ItemRareness.Middle;
        }
        else {
            rareness = ItemRareness.High;
        }
        List<ItemTypeInfo> list = _rareDic[rareness];
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
