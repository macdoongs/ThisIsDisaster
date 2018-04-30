using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItmeFocused : MonoBehaviour {

    public Image border;
    public Color focusedColor;

    public void itemFocused()
    {
        border.color = focusedColor;

    }
}
