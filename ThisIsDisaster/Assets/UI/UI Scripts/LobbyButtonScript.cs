using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButtonScript : MonoBehaviour {

    public Text focusedText;
    public GameObject focusedUnderLine;

    public Text anotherText1;
    public GameObject anotherUnderLIne1;

    public Text anotherText2;
    public GameObject anotherUnderLIne2;
    

    public void OnClick()
    {
        focusedText.color = Color.yellow;
        focusedUnderLine.SetActive(true);

        anotherText1.color = Color.white;
        anotherUnderLIne1.SetActive(false);
        anotherText2.color = Color.white;
        anotherUnderLIne2.SetActive(false);

    }
    
}
