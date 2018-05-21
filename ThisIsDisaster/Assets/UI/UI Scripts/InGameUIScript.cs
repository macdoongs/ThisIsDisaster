using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    // 플레이어 캐릭터 게임 오브젝트
    public GameObject PlayerCharacter;    
    // 상태창 컨트롤러가 저장되는 게임 오브젝트
    public GameObject StatusManager;
    // 인벤토리 컨트롤러가 저장되는 게임 오브젝트
    public GameObject InventoryManager;

    public GameObject StatusBarManager;
    public GameObject SettingManager;
    public GameObject StageClearManager;
    public void Start()
    {
        if (PlayerCharacter == null) {
            PlayerCharacter = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
        }

        StatusManager.GetComponent<StatusUIController>().SetPlayerInfo(PlayerCharacter);
        StatusBarManager.GetComponent<StatusBarUIScript>().
            SetPlayerInfo(PlayerCharacter);
        InventoryManager.GetComponent<InventoryUIController>().
            InitialCategory();
    }

    public void Update()
    {
        StatusManager.GetComponent<StatusUIController>().
            GetStatus(PlayerCharacter);

        InventoryManager.GetComponent<InventoryUIController>().
            SlotSprite();
        InventoryManager.GetComponent<InventoryUIController>().
            PreviewSprite();
        InventoryManager.GetComponent<InventoryUIController>().
            InventoryUpdate();

        StatusBarManager.GetComponent<StatusBarUIScript>().
            UpdateStatusBar(PlayerCharacter);
    }  


    public void MenuClicked(GameObject menu)
    {

        if (menu.activeInHierarchy)
        {
            CloseAllUI();
        }
        else
        {
            CloseAllUI();
            menu.SetActive(true);
        }

    }

    public void CloseAllUI()
    {
        StatusManager.GetComponent<StatusUIController>().Close();
        InventoryManager.GetComponent<InventoryUIController>().Close();
        SettingManager.GetComponent<SettingUIController>().Close();
    }

    public void StageClear()
    {
        CloseAllUI();
        StageClearManager.GetComponent<StageClearUIContorller>().StageClearPanel.SetActive(true);
    }

}


