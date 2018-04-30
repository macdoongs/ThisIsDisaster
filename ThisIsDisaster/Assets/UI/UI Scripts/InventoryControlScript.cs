using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControlScript : MonoBehaviour {


    public Color defaultColor;
    public Color focusedColor;

    public Image[] itemTypes = new Image[4];

	// Use this for initialization
	void Start () {
        DefaultItemTypes();
        itemTypes[0].color = focusedColor;	
	}
	
    public void ItemTypeFocused(int index)
    {
        DefaultItemTypes();
        itemTypes[index].color = focusedColor;
    }

    public void DefaultItemTypes()
    {
        for (int i = 0; i < itemTypes.Length; i++)
        {
            itemTypes[i].color = defaultColor;
        }
    }

}
