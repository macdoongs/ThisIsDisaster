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

    public GameObject WarningPanel;
    public Text WarningDescription;

    public GameObject StatusBarManager;
    public GameObject SettingManager;
    public GameObject StageClearManager;

    public GameObject EventNoticePanel;
    public Text EventNoticeText;

    public bool NoticeToken = false;
    private float TimeLeft = 5.0f;
    private float nextTime = 0.0f;

    public static InGameUIScript Instance
    {
        private set;
        get;
    }

    private void Awake()
    {
       if(Instance != null && Instance.gameObject != null) {
            GameObject.Destroy(gameObject);
            return;
                }
        Instance = this;
    }

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


        if (NoticeToken) {
            if (Time.time > nextTime)
            {
                DefaultEventNoticePanel();
                NoticeToken = false;
            }
        }
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
        DefaultEventNoticePanel();
        StatusManager.GetComponent<StatusUIController>().Close();
        InventoryManager.GetComponent<InventoryUIController>().Close();
        SettingManager.GetComponent<SettingUIController>().Close();
        WarningPanel.SetActive(false);
    }

    public void StageClear()
    {
        CloseAllUI();
        StageClearManager.GetComponent<StageClearUIContorller>().StageClearPanel.SetActive(true);
    }

    public void AttackClicked()
    {
        GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<PlayerAttackController>().OnAttackClicked();        
    }

    public void Warning(string desc)
    {
        WarningDescription.text = desc;
        WarningPanel.SetActive(true);
    }

    public void EventNotice(string eventName, int type)
    {
        DefaultEventNoticePanel();

        if(type == 0)
        {
            EventNoticeText.text = eventName + " 이벤트가 발생하였습니다.";
        }
        else if(type == 1)
        {
            EventNoticeText.text = eventName + " 이벤트가 시작되었습니다.";
        }
        else
        {
            EventNoticeText.text = eventName + " 이벤트가 종료되었습니다.";
        }

        EventNoticePanel.SetActive(true);

        NoticeToken = true;
        nextTime = Time.time + TimeLeft;
    }

    public void DefaultEventNoticePanel()
    {
        EventNoticeText.text = "";
        EventNoticePanel.SetActive(false);
    }
}


