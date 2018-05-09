using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NetworkComponents;

public class PlayerBehaviour : UnitBehaviourBase
{
    
    private void Start()
    {

        _prevPos = Controller.GetPosition();
    }

    private void Update()
    {
        try
        {
            if (IsRemoteCharacter)
            {
                CalcRemotePosition();
            }
            else {
                CalcLocalPosition();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    public virtual bool IsLocal() {
        return false;
    }

    public override void Initialize()
    {
        
    }

}
