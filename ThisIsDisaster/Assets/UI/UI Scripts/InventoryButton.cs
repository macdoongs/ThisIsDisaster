using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventoryButton : MonoBehaviour {

    public GameObject InventoryInside;
        
    public void InventoryInsideActive()
    {
        InventoryInside.SetActive(true);
    }


}
