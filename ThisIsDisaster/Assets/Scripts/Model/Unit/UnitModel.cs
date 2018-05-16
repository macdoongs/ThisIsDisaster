using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UnitModel {
    public long instanceId = 0;

    public virtual float GetAttackDamage() {
        return 0f;
    }

    public virtual bool IsAttackTargetable() {
        return true;
    }
}
