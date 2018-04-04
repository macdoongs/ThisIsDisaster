using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItmeFocused : MonoBehaviour {

    public Image border;

    public void itemFocused()
    {
        Color newColor = Color.blue;
        Debug.Log("Button Pressed");
        border.color = newColor;

    }
}
