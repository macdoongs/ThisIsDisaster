using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnitModel : MonoBehaviour {

    public Dictionary<long, ItemModel> ItemSpace = new Dictionary<long, ItemModel>();


    public List<ItemModel> ItemLists = new List<ItemModel> { };
    public List<String> ItemNames = new List<string> { };
    public List<int> ItemCounts = new List<int> { };

    public int cursor = 0;

    public virtual bool AddItem(ItemModel item, int amount) {
     //   ItemModel oldData = null;

        if (item.metaInfo.itemType.Equals(ItemType.Etc))//소모품은 이미 인벤토리에 있으면 개수만 늘어나야함
        {
            if (ItemNames.Contains<String>(item.metaInfo.Name))//이미 인벤토리에 들어가 있으면
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
        if(ItemLists.Count != 0)
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
    
    public virtual bool RemoveItem(ItemModel item) {

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
    
    public void PrintAllItems() {
        Debug.Log("Unit Items");
        foreach (var kv in ItemSpace) {
            Debug.Log(kv.Value.instanceId +" : "+ kv.Value.metaInfo.ToString());
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

    public List<ItemModel> GetAllItems() {
        List<ItemModel> output = new List<ItemModel>(ItemLists);
        return output;
    }

    public List<int> GetAllCounts()
    {
        List<int> output = new List<int>(ItemCounts);
        return output;
    }
}
