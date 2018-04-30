using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButtonScript : MonoBehaviour {

    public Color focusedTextColor;
    public Color defaultTextColor;

    public Text focusedText;
    public GameObject focusedUnderLine;

    public Text[] anotherTexts = new Text[2];
    public GameObject[] anotherUnderLInes = new GameObject[2];

    public void OnClick()
    {
        focusedText.color = Color.yellow;
        focusedUnderLine.SetActive(true);

        for(int i = 0; i < anotherTexts.Length; i++)
        {
            anotherTexts[i].color = defaultTextColor;
            anotherUnderLInes[i].SetActive(false);

        }

    }
    
}
