using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class LobbyUIScript : MonoBehaviour {

    public Color focusedTextColor;
    public Color defaultTextColor;

    public Text[] MenuTexts = new Text[3];

    public int MenuPosition = -1;

    public GameObject GameModePanel;
    public Text[] GameModeTexts = new Text[2];

    public GameObject StartPanel;

    public GameObject LoadingPanel;
    public Image LoadingBar;
    public Text LoadingText;
    public float LoadingAmount = 0;

    public MatchingPanel matching;

    public static LobbyUIScript Instance
    {
        private set;
        get;
    }


    void Awake()
    {

        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;

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
            }
            else
            {
                MenuTexts[i].color = defaultTextColor;
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
        }
        DefaultGameMode();
        GameModePanel.SetActive(false);
    }

    public void DefaultGameMode()
    {
        for(int i = 0; i < GameModeTexts.Length; i++)
        {
            GameModeTexts[i].color = defaultTextColor;
        }

        StartPanel.SetActive(false);
    }

    public void GameModeClicked(int mode)
    {
        DefaultGameMode();
        GameModeTexts[mode].color = focusedTextColor;


        if (mode == 1) {
            matching.OnOpenPanel();
        }
        else {
            if (matching.IsEnabled) {
                matching.OnClosePanel();
            }
            StartPanel.SetActive(true);
        }
    }

    public void StartGame()
    {
        GlobalGameManager.Instance.SetGameNetworkType(GameNetworkType.Single);
        GlobalGameManager.Instance.OnGameStart();
    }

    public void LogOut()
    {
        AccountManager.Instance.ClearAccount();

        FB.Init();
        FB.ActivateApp();
        FB.LogOut();

        LoadingSceneManager.LoadScene("Login Scene");
    }
}
