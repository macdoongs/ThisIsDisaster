using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour {

    public GameObject Inventory;

    public void OpenInventory()
    {
        Inventory.SetActive(true);
    }

    public void CloseInventory()
    {
        Inventory.SetActive(false);
    }
}
