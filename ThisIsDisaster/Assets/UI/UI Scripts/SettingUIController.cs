using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour {

    public GameObject SettingUI;

    public GameObject[] Panels;
    public GameObject InformationPanel;
    public GameObject GameSettingPanel;
    public GameObject PushSettingPanel;
    public GameObject AccountPanel;

    public Button[] Category;
    public Button InformationButton;
    public Button GameSettingButton;
    public Button PushSettingButton;
    public Button AccountButton;

    public Color highlightColor;
    public Color defaultColor;

    public Scrollbar[] GameSettingScrollBars;
    public Scrollbar BackgroundVolume;
    public Scrollbar EffectVolume;
    public Scrollbar HPWarning;
    public Scrollbar StaminaWarning;

    public Image[] ScrollBarFillAmount;
    public Image BackgroundVolumeFill;
    public Image EffectVolumeFill;
    public Image HPWarningFill;
    public Image StaminaWarningFill;

    public static SettingUIController Instance
    {
        private set;
        get;
    }

    private void Awake()
    {
        if (Instance != null && Instance.gameObject != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void Start()
    {
        Panels = new GameObject[] { InformationPanel, GameSettingPanel, PushSettingPanel, AccountPanel };
        Category = new Button[] { InformationButton, GameSettingButton, PushSettingButton, AccountButton };
        GameSettingScrollBars = new Scrollbar[] { BackgroundVolume, EffectVolume, HPWarning, StaminaWarning };
        ScrollBarFillAmount = new Image[] { BackgroundVolumeFill , EffectVolumeFill , HPWarningFill
            , StaminaWarningFill};
        DefaultMenu();
    }

    public void Update()
    {
        HighlightCategory();
        FillScrollBar();
    }

    public void DefaultMenu()
    {
        CloseAllMenu();
        InformationPanel.SetActive(true);
        Category[0].GetComponent<Image>().color = highlightColor;
    }

    public void CloseAllMenu()
    {
        foreach(var panel in Panels)
        {
            panel.SetActive(false);
        }

        foreach(var button in Category)
        {
            button.GetComponent<Image>().color = defaultColor;
        }
    }

    public void HighlightCategory()
    {
        for(int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].activeSelf)
                Category[i].GetComponent<Image>().color = highlightColor;
            else
                Category[i].GetComponent<Image>().color = defaultColor;
        }
        
    }

    public void MenuButtonClicked(GameObject menu)
    {
        CloseAllMenu();
        menu.SetActive(true);
        HighlightCategory();
    }

    public void FillScrollBar()
    {
        for(int i = 0; i < GameSettingScrollBars.Length; i++)
        {
            float fillAmount = GameSettingScrollBars[i].value;

            ScrollBarFillAmount[i].fillAmount = fillAmount;
        }
    }


    public void Close()
    {
        SettingUI.SetActive(false);
    }
}
