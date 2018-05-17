using UnityEngine;
using System.Collections;

public class PlayerSensor : AttackReceiver
{

    // Use this for initialization
    void Start()
    {
        if (_reciveCollider == null) {
            EmergencyLoadCollider();
}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
