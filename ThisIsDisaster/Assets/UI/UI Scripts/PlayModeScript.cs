using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayModeScript : MonoBehaviour {

    public GameObject playModePannel;
    public GameObject startPannel;

    public Text singleText;
    public GameObject singleUnderLine;

    public Text multiText;
    public GameObject multiUnderLine;

    public void ModePannelOn()
    {
        PlayModePannelDefault();
        playModePannel.SetActive(true);
        

    }

    public void StartPannelOn()
    {
        startPannel.SetActive(true);
    }


    public void ModePannelOff()
    {
        playModePannel.SetActive(false);
        startPannel.SetActive(false);
    }

    public void SingleFocused()
    {
        singleText.color = Color.yellow;
        singleUnderLine.SetActive(true);

        multiText.color = Color.black;
        multiUnderLine.SetActive(false);

        StartPannelOn();
    }

    public void MultiFocused()
    {
        singleText.color = Color.black;
        singleUnderLine.SetActive(false);

        multiText.color = Color.yellow;
        multiUnderLine.SetActive(true);
        StartPannelOn();

    }

    public void PlayModePannelDefault()
    {
        singleText.color = Color.black;
        singleUnderLine.SetActive(false);

        multiText.color = Color.black;
        multiUnderLine.SetActive(false);
    }
}
