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


    public void Start()
    {
        StatusBarManager.GetComponent<StatusBarUIScript>().
            SetPlayerInfo(PlayerCharacter);
        InventoryManager.GetComponent<InventoryUIController>().
            InitialItemTypes();
    }

    public void Update()
    {
        StatusManager.GetComponent<StatusUIController>().
            GetStatus(PlayerCharacter);

        InventoryManager.GetComponent<InventoryUIController>().
            SlotSprite(PlayerCharacter);
        InventoryManager.GetComponent<InventoryUIController>().
            SetPreviewSprite(PlayerCharacter);
        InventoryManager.GetComponent<InventoryUIController>().
            InventoryUpdate(PlayerCharacter);

        StatusBarManager.GetComponent<StatusBarUIScript>().
            UpdateStatusBar(PlayerCharacter);
    }  
}


