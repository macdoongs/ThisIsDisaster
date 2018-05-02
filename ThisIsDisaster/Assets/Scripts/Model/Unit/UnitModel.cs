﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnitModel {

    protected Dictionary<long, ItemModel> ItemSpace = new Dictionary<long, ItemModel>();

    public virtual bool AddItem(ItemModel item) {
        ItemModel oldData = null;
        if (ItemSpace.TryGetValue(item.instanceId, out oldData)) {
            //already contains
            return false;
        }

        ItemSpace.Add(item.instanceId, item);

        return true;
    }

    public virtual bool RemoveItem(ItemModel item) {
        ItemModel oldData = null;
        if (!ItemSpace.TryGetValue(item.instanceId, out oldData)) {
            //this unit doesn't has this item
            return false;
        }

        ItemSpace.Remove(item.instanceId);

        return true;
    }
    
}