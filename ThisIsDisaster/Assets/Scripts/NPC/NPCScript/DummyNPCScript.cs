using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class DummyNPCScript : NPCScriptBase
    {

        public override void Initialize()
        {
            //Debug.Log("Make Dummy NPC");
        }

        public override void OnDefeated()
        {
            Debug.Log("Dummy dead");
            Unit.animTarget.SetActive(false);
        }
    }
}