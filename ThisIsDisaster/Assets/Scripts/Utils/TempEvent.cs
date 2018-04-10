using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Unit {
//Change As UnitModel
}

public class EventBase {
    /*
     실행순서

     OnGenerated : 생성만 된 상태, 실제로 시작하지는 않음
     ↓
     OnStart : 이벤트 시작 시 호출
     ↓
     OnExecution : 이벤트 진행 중 호출
     ↓
     OnEnd : 이벤트 종료 시 호출
     ↓
     OnDestroy : 이벤트 제거 시 호출
    */

    public virtual void OnGenerated() { }

    public virtual void OnStart() { }

    public virtual void OnExecution() { }

    public virtual void OnEnd() { }

    public virtual void OnDestroy() { }

    public virtual void OnGiveDamage(Unit target) { }
}

public class CycloneEvent : EventBase {
    public override void OnGenerated()
    {
        //MakeWorldRain - level 0
        //MakeWorldWind - level 0
        
    }

    public override void OnStart()
    {
        //MakeWorkDark
        //RainFallHeavier - level 1
        //WindBlowFaster - level 1
        //Start Cyclone Lifetime - 30 seconds
        
    }

    public override void OnExecution()
    {
        //Update Rain Heavyily - 1 ~ MAX
        //Update Wind Fast - 1 ~ MAX
        //Execution Lifetime

        //Decrease Player's Health
        //if Player has Some Water And Food
        //  Decrease Player's Stamina few
        //else
        //  Decrease Player's Stamina normaly

        //if Player is In Dark Area
        //  if Player's Near is Dark
        //     Decrease Player's Health

        //Make Cyclone's Eye?
    }

    public override void OnEnd()
    {
        //Clear DarkEffect
        //Clear Wind
        //Clear Rain
    }

    public override void OnGiveDamage(Unit target)
    {
        //if (unit is in Shelter)
        //  give some lower damage
        //else
        //  give normal damage
    }
}

public class FireEvent : EventBase {
    //Fire Generation -> List or Array

    public override void OnGenerated()
    {
        //Read Map And Decision Fire Generation Point
    }

    public override void OnStart()
    {
        //Check All Positions Which Fire Generation For Start Fire
        
    }

    public override void OnExecution()
    {
        //If Player Tries Extinguish
        //  if Player Has Stronger Extinguish Object || Fire Is Small
        //      Effective Extinguish
        //  else (Fire Is Strong || Player Doesn't Have Good Extinguish Object
        //      Not Effective Extinguish
    }

    public override void OnGiveDamage(Unit target)
    {
        //Make Target Some Burn Debuf (화상)
        //Decrease Target's Health Heavily
        //Decrease Target's Stamina Heavily
    }
}
