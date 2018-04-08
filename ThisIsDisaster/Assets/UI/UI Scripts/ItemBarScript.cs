using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBarScript : MonoBehaviour {

    public Image[] itemBorders = new Image[6];

    public Color defaultColor;
    /**
    public Image itemBorder1;
    public Image itemBorder2;
    public Image itemBorder3;
    public Image itemBorder4;
    public Image itemBorder5;
    public Image itemBorder6;
    */

    public void FocusOff()
    {
        for(int i = 0; i < itemBorders.Length; i++)
        {
            itemBorders[i].color = defaultColor;

        }
    }
    
}
