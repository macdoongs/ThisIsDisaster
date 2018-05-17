using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour {

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

    public void Start()
    {
        Panels = new GameObject[] { InformationPanel, GameSettingPanel, PushSettingPanel, AccountPanel };
        Category = new Button[] { InformationButton, GameSettingButton, PushSettingButton, AccountButton };
        DefaultMenu();
    }

    public void Update()
    {
        HighlightCategory();
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

}
