using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUIScript : MonoBehaviour
{
    public Image VisionImage;

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

    public Color DefaultNightColor;

    public GameObject EventDescTitle;
    public Image EventIconImage;
    public Text EventNameText;
    public Image EventDescPanel;
    public Text EventDescText;

    public GameObject AEDText;
    public GameObject AEDButton;
    public GameObject PlayerDeadPanel;

    public GameObject GameLogUI;
    public GameObject GameLogPivot;
    public Text GameLogText;

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
        if (GlobalGameManager.Instance.GameNetworkType == GameNetworkType.Multi)
        {
            return;
        }
        else {
            //GameManager.CurrentGameManager.Init();
            //Init();
        }
    }

    public void Init()
    {
        if (PlayerCharacter == null)
        {
            PlayerCharacter = GameManager.CurrentGameManager.GetLocalPlayer().gameObject;
        }

        StatusUIController.Instance.SetPlayerInfo(PlayerCharacter);
        StatusBarUIScript.Instance.SetPlayerInfo(PlayerCharacter);
        InventoryUIController.Instance.InitialCategory();
        DefaultEventDesc();
        PlayerDeadPanel.SetActive(false);
        GameLogUI.SetActive(false);
        GameLogPivot.SetActive(false);


    }

    public void Update()
    {
        if (PlayerCharacter == null) return;
        VisionSet();
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

    public void VisionSet()
    {
        int vision = PlayerCharacter.GetComponent<CharacterModel>().visionLevel;
        
        VisionImage.GetComponentInChildren<RectTransform>().sizeDelta = new Vector2(Screen.height, VisionImage.rectTransform.rect.width);

        if (vision == 0)
        {
            VisionImage.sprite = null;                      
        }
        else
        {
            string spriteSrc = "Vision/vision" + vision.ToString();
            Sprite s = Resources.Load<Sprite>(spriteSrc);
            VisionImage.sprite = s;
        }
        VisionImage.color = DefaultNightColor;
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
        SetGameLog();
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

    public void EventDescSetting(WeatherType weather)
    {
        string src;
        if (weather.Equals(WeatherType.Flood))
        {//홍수
            src = "EventIcon/floodEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "홍수";
            EventDescText.text = "홍수가 발생하면 낮은 지역에 물이 차오릅니다.\n물이 차오른 지역에 있는 캐릭터는 피해를 받습니다.\n홍수가 끝날때까지 높은곳으로 대피하세요.";
        }
        else if (weather.Equals(WeatherType.Yellowdust))
        {//황사
            src = "EventIcon/yellowdustEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "황사";
            EventDescText.text = "황사가 발생하면 지속적으로 피해를 받습니다.\n피난처로 대피하거나 마스크를 착용하세요.";
        }
        else if (weather.Equals(WeatherType.Drought))
        {//가뭄
            src = "EventIcon/droughtEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "가뭄";
            EventDescText.text = "가뭄이 발생하면 스테미나가 빠르게 감소합니다.\n지속적으로 물을 섭취해주세요.";
        }
        else if (weather.Equals(WeatherType.Fire))
        {//화재
            src = "EventIcon/fireEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "화재";
            EventDescText.text = "화재가 발생하면 불이 빠른속도로 번지게 됩니다.\n소화기 아이템을 이용해 불을 꺼주세요.";

        }
        else if (weather.Equals(WeatherType.Earthquake))
        {//지진
            src = "EventIcon/earthquakeEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "지진";
            EventDescText.text = "지진이 발생하면 진앙을 기준으로 일정범위에 큰 피해를 줍니다.\n강진이 오기 전에 진앙으로부터 최대한 멀어지세요.";
        }
        else if (weather.Equals(WeatherType.Thunderstorm))
        {//낙뢰
            src = "EventIcon/thunderEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "낙뢰";
            EventDescText.text = "낙뢰에 맞으면 큰 피해를 받습니다.\n피뢰침을 설치하여 낙뢰를 대신 맞게 하세요.";
        }
        else if (weather.Equals(WeatherType.Landslide))
        {//산사태
            src = "EventIcon/landslidEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "산사태";
            EventDescText.text = "산사태 설명";
        }
        else if (weather.Equals(WeatherType.Cyclone))
        {//태풍
            src = "EventIcon/cycloneEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "태풍";
            EventDescText.text = "태풍 발생 시 비를 맞으면 지속적으로 스테미너가 감소합니다.\n비를 막을 수 있는 아이템을 착용하거나 피난처로 이동해 비를 피하세요.";
        }
        else//heavysnow 
        {
            src = "EventIcon/heavysnowEvent";
            Sprite s = Resources.Load<Sprite>(src);
            EventIconImage.sprite = s;
            EventNameText.text = "폭설";
            EventDescText.text = "폭설이 발생하면 스테미너가 빠르게 감소합니다.\n피난처로 대피하고 모닥불을 피워 피해를 최소화하세요.";
        }
        float y = EventDescText.rectTransform.sizeDelta.y;
    //    EventDescPanel.rectTransform.sizeDelta = new Vector2(EventDescPanel.rectTransform.sizeDelta.x, y);
        //Debug.LogError(EventDescPanel.rectTransform.sizeDelta.x);
        //Debug.LogError(y);


//        EventDescPanel.GetComponent<BoxCollider2D>().size = new Vector2(EventDescPanel.rectTransform.sizeDelta.x, EventDescText.rectTransform.rect.height + 20);
        EventDescTitle.SetActive(true);
    }

    public void DefaultEventDesc()
    {
        EventIconImage.sprite = null;
        EventNameText.text = "";
        EventDescText.text = "";
        EventDescOff();
        EventDescTitle.SetActive(false);
    }
    
    public void EventDescOn()
    {
        EventDescPanel.gameObject.SetActive(true);
    }

    public void EventDescOff()
    {
        EventDescPanel.gameObject.SetActive(false);
    }

    public void PlayerDeadPanelOn(bool hasAED)
    {
        if (hasAED)
        {
            AEDText.SetActive(true);
            AEDButton.SetActive(true);            
        }
        else
        {
            AEDText.SetActive(false);
            AEDButton.SetActive(false);

            //GameManager.CurrentGameManager.EndStage(false);
            SetGameLog();
        }
        PlayerDeadPanel.SetActive(true);
    }

    public void Retry()
    {
        PlayerDeadPanel.SetActive(false);
        PlayerCharacter.GetComponent<CharacterModel>().RetryGame();
    }

    public void SetGameLog() {
        string text = GameLogManager.Instance.OnGameEnd();
        GameLogText.text = text;
        GameLogUI.SetActive(true);
    }

    public void OnClickLog() {
        if (GameLogPivot.activeInHierarchy)
        {
            GameLogPivot.SetActive(false);
        }
        else {
            GameLogPivot.SetActive(true);
        }
    }
}


