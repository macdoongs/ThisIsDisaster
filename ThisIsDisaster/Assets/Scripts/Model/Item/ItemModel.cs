using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemModel {
    //Who is Owner

    //field will be initialized
    //Think which will be needed

    public ItemTypeInfo metaInfo;
    public long instanceId;

    public UnitModel Owner = null;

    public virtual void OnItemAqquired(UnitModel target) {
        //do smth
        Owner = target;
    }

    public virtual void OnItemRemoved(UnitModel prevOwner) {
        //do smth
        Owner = null;
    }
}

public class DummyItem : ItemModel {
    
}