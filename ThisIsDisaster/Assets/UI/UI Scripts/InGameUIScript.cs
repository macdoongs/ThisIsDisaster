using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIScript : MonoBehaviour
{
    // 플레이어 캐릭터 게임 오브젝트
    public GameObject PlayerCharacter;    

    public GameObject NoticePanel;
    public Text NoticeTitle;
    public Text NoticeDescription;
    
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
        GameManager.CurrentGameManager.Init();
        if (PlayerCharacter == null) {
            PlayerCharacter = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
        }

        StatusUIController.Instance.SetPlayerInfo(PlayerCharacter);
        StatusBarUIScript.Instance.SetPlayerInfo(PlayerCharacter);
        InventoryUIController.Instance.InitialCategory();

       
     //   Destroy(LobbyUIScript.Instance.transform.gameObject);
       
    }

    public void Update()
    {
        StatusUIController.Instance.GetStatus(PlayerCharacter);
        
        InventoryUIController.Instance.SlotSprite();
        InventoryUIController.Instance.PreviewSprite();
        InventoryUIController.Instance.InventoryUpdate();

        StatusBarUIScript.Instance.UpdateStatusBar(PlayerCharacter);


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
        StatusUIController.Instance.Close();
        InventoryUIController.Instance.Close();
        SettingUIController.Instance.Close();
        NoticePanel.SetActive(false);
    }

    public void StageClear()
    {
        CloseAllUI();
        StageClearUIContorller.Instance.StageClearPanel.SetActive(true);
    }

    public void AttackClicked()
    {
        GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<PlayerAttackController>().OnAttackClicked();        
    }

    public void JumpClicked()
    {
        GameManager.CurrentGameManager.GetLocalPlayer().GetComponent<PlayerMoveController>().Jump();
    }


    public void Notice(string title,string desc)
    {
        NoticeTitle.text = title;
        NoticeDescription.text = desc;
        NoticePanel.SetActive(true);
    }

    public void EventNotice(string eventName, int type)
    {
        DefaultEventNoticePanel();
        string Name = "";

        if (eventName == "Cyclone")
            Name = "태풍";
        else if (eventName == "Flood")
            Name = "홍수";
        else if (eventName == "Yellowdust")
            Name = "황사";
        else if (eventName == "Drought")
            Name = "가뭄";
        else if (eventName == "Earthquake")
            Name = "지진";
        else if (eventName == "Thunderstorm")
            Name = "천둥";
        else
            Name = "폭설";

        if (type == 0)
        {
            EventNoticeText.text = Name + " 이벤트가 발생하였습니다.";
        }
        else if(type == 1)
        {
            EventNoticeText.text = Name + " 이벤트가 시작되었습니다.";
        }
        else
        {
            EventNoticeText.text = Name + " 이벤트가 종료되었습니다.";
        }

        EventNoticePanel.SetActive(true);

        NoticeToken = true;
        nextTime = Time.time + TimeLeft;
    }

    public void DisorderNotice(Disorder.DisorderType disorder)
    {
        DefaultEventNoticePanel();
        if (disorder.Equals(Disorder.DisorderType.mirage))
        {
            EventNoticeText.text = "캐릭터가 신기루를 보고 있습니다.";
        }
        else if (disorder.Equals(Disorder.DisorderType.injury))
        {
            EventNoticeText.text = "캐릭터가 부상을 당했습니다.";
        }
        else if (disorder.Equals(Disorder.DisorderType.hunger))
        {
            EventNoticeText.text = "캐릭터가 굶주림을 느낍니다.";
        }
        else if (disorder.Equals(Disorder.DisorderType.thirst))
        {
            EventNoticeText.text = "캐릭터가 갈증을 심하게 느낍니다.";
        }
        else if (disorder.Equals(Disorder.DisorderType.poisoning))
        {
            EventNoticeText.text = "캐릭터가 식중독에 걸렸습니다.";
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

    public void BackToLobbyScene()
    {
        SceneManager.LoadScene("Lobby Scene");
    }
}


