using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIScript : MonoBehaviour {

    public Color focusedTextColor;
    public Color defaultTextColor;

    public Text[] MenuTexts = new Text[3];
    public GameObject[] Underbars = new GameObject[3];

    public int MenuPosition = -1;

    public GameObject GameModePanel;
    public Text[] GameModeTexts = new Text[2];
    public GameObject[] GameModeUnderbars = new GameObject[2];

    public GameObject StartPanel;

    public void Start()
    {
        
    }

    public void Update()
    {
        MenuUpdate();
    }

    public void GetMenuPosition(int position)
    {
        MenuPosition = position;
    }
    
    public void MenuUpdate()
    {
        if(MenuPosition == -1)
        {
            DefaultMenu();
            return;
        }

        for(int i = 0; i < MenuTexts.Length; i++)
        {
            if (i == MenuPosition)
            {
                MenuTexts[i].color = focusedTextColor;
                Underbars[i].SetActive(true);
            }
            else
            {
                MenuTexts[i].color = defaultTextColor;
                Underbars[i].SetActive(false);
            }
        }

        if (MenuPosition == 0)
        {
            GameModePanel.SetActive(true);
        }
        else
        {
            GameModePanel.SetActive(false);
        }
    }

    public void DefaultMenu()
    {
        for(int i = 0; i < MenuTexts.Length; i++)
        {
            MenuTexts[i].color = defaultTextColor;
            Underbars[i].SetActive(false);
        }
        DefaultGameMode();
        GameModePanel.SetActive(false);
    }

    public void DefaultGameMode()
    {
        for(int i = 0; i < GameModeTexts.Length; i++)
        {
            GameModeTexts[i].color = defaultTextColor;
            GameModeUnderbars[i].SetActive(false);
        }

        StartPanel.SetActive(false);
    }

    public void GameModeClicked(int mode)
    {
        DefaultGameMode();
        GameModeTexts[mode].color = focusedTextColor;
        GameModeUnderbars[mode].SetActive(true);
        StartPanel.SetActive(true);
    }
}
