using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkComponents;
using System.IO;
using System.Net;
using LitJson;
using System.Text;

public class MatchingPanel : MonoBehaviour, IObserver
{
    public static MatchingPanel Instance { private set; get; }
    delegate void DelayEvent();

    public Animator PivotCTRL;
    public MatchingSlot[] matchingSlots;
    private Dictionary<int, int> _slotAccount = new Dictionary<int, int>();

    private int _udpServerPort = 0;

    private DelayEvent _delayEvent = null;
    private Timer _delayEventTrigger = new Timer();

    private bool _isHost = false;

    public CanvasGroup StartingPanel;

    string hostAddress = "";



    private string url = "http://api.thisisdisaster.com/";
    private string result = null;

    int resultCode = 0;
    string resultMsg;

    public bool IsEnabled {
        private set; get;
    }

    private void Awake()
    {
        Instance = this;
        if (GlobalGameManager.Instance != null) { }
    }

    private void Start()
    {
        hostAddress = GetLocalHost();
        //zero is account
        _slotAccount.Add(0, GlobalParameters.Param.accountId);


        matchingSlots[0].SetPlayer(GlobalParameters.Param.accountId, GlobalParameters.Param.accountLevel, GlobalParameters.Param.accountName, false);
        matchingSlots[0].SetPlayerReady(false);
    }

    private void Update()
    {
        if(!IsEnabled)
            CancelInvoke("GetMultiplayLobby");

        if (_delayEventTrigger.started)
        {
            if (_delayEventTrigger.RunTimer())
            {
                if (_delayEvent != null)
                {
                    _delayEvent();
                }

                
            }
        }
    }

    private void OnEnable()
    {
        ObserveNotices();
    }

    private void OnDisable()
    {
        RemoveNotices();
    }

    /*
     매칭 패널이 열리는 순간 호출됩니다.
     이 때, 서버와의 통신을 통해 매칭 중인 방이 없다면 OnHost를 호출해야 합니다.
     만약 매칭 중인 방이 있다면 해당 방의 IP를 호스트 IP로 지정한 뒤, OnGuest를 호출해야 합니다.
     
        매칭 호스트 지정: MatchingPanel.Instance.SetHostAddress(ipAddr : string);
         */
    public void OnOpenPanel() {
        IsEnabled = true;

        Show();
        SetGameStarting(false);

        GoMatching();
        InvokeRepeating("GetMultiplayLobby", 3.0f, 3.0f);

    }

    public void OnClosePanel() {
        //stop
        IsEnabled = false;
        Hide();
        LobbyUIScript.Instance.DefaultMenu();


        string email = GlobalParameters.Param.accountEmail;

        WebManager.SendRequest(Json.RequestMethod.POST, "game/multiplay/leave?email=" + email, "");
    }

    void Show() {
        AnimatorUtil.SetTrigger(PivotCTRL, "Show");
    }

    void Hide() {
        AnimatorUtil.SetTrigger(PivotCTRL, "Hide");
    }
    
    #region Network
    string GetLocalHost()
    {
        return Network.player.ipAddress.ToString();
    }

    /// <summary>
    /// Host의 아이디를 이어줘야합니다.
    /// </summary>
    /// <returns></returns>
    string GetHostAddress()
    {
        IPAddress temp;
        if (IPAddress.TryParse(hostAddress, out temp)) {
            return hostAddress;
        }
        return GetLocalHost();//dummy
    }

    public void SetHostAddress(string address) {
        hostAddress = address;
    }
    
    void StartHostServer() {
        bool state = NetworkModule.Instance.StartGameServer(NetConfig.PLAYER_MAX);
        if (!state) {
            NetDebug.LogError("Failed To Start Host Server");
        }
    }

    void StartUdpServer(int port)
    {
        try
        {
            //NetworkModule.Instance.ClearReceiveNotification();
            //GameServer.Instance.InitializeNetworkModule();
            _udpServerPort = port;
            GameServer.Instance.SetUDPServerPort(port);
            NetworkModule.Instance.StartServer(port, NetConfig.PLAYER_MAX, NetworkModule.ConnectionType.UDP);
        }
        catch {
            NetDebug.LogError("Failed To Start UDP Server");
        }
    }

    void ConnectToHost() {
        string hostAddr = _isHost ? GetLocalHost() : GetHostAddress();
        int serverNode = NetworkModule.Instance.Connect(hostAddr, NetConfig.SERVER_PORT, NetworkModule.ConnectionType.TCP);
        if (serverNode >= 0)
        {
            NetworkModule.Instance.SetServerNode(serverNode);
            NetDebug.LogError("TCP Server Node : " + serverNode);
        }
        else {
            NetDebug.LogError("Failed To Connect TCP Server");
        }
    }

    public void OnHost() {
        SetIsHost(true);
#if true
        GameServer.Instance.SetLocalAddress(GetLocalHost());
        GameServer.Instance.InitializeNetworkModule();

        //StartUdpServer();
        StartUdpServer(NetConfig.GAME_PORT);
        StartHostServer();
#endif
        GameServer.Instance.MakeMatchingView();
    }

    public void OnGuest() {
        SetIsHost(false);
#if true
        GameServer.Instance.SetLocalAddress(GetLocalHost());
        GameServer.Instance.InitializeNetworkModule();
        ConnectToHost();
        GameServer.Instance.MakeMatchingView();

        SendMatchingRequest();
        //_delayEvent = new DelayEvent(SendMatchingRequest);
        //_delayEventTrigger.StartTimer(1f);
        //StartUdpServer();
        //GameServer.Instance.SendMatchingRequest();
        
#endif
    }

    void SendMatchingRequest() {
        //accquire udp port
        GameServer.Instance.SendMatchingRequest();

        //_delayEvent = new DelayEvent(RequestMatchingData);
        //_delayEventTrigger.StartTimer(1f);
    }

    void RequestMatchingData() {
        GameServer.Instance.SendGameServerRequest(GameServerRequestType.MatchingData);
    }


    Json.WebCommunicationManager WebManager
    {
        get
        {
            return Json.WebCommunicationManager.Manager;
        }
    }


    public void OnClickGameStart() {
        if (!_isHost) return;
        GlobalGameManager.Instance.SetGameNetworkType(GameNetworkType.Multi);
        GlobalGameManager.Instance.OnGameStart();
        GameServer.Instance.DestroyMatchingView();

        string email = GlobalParameters.Param.accountEmail;

        WebManager.SendRequest(Json.RequestMethod.GET, "game/start?mode=multiplay&email=" + email, "");
    }

    public void SetIsHost(bool isHost) {
        _isHost = isHost;
        GameServer.Instance.SetHost(isHost);
        GlobalGameManager.Instance.SetHost(isHost);
    }

    public void ResetNetwork() {
        NetworkModule.Instance.Disconnect();
        NetworkModule.Instance.StopGameServer();
        NetworkModule.Instance.StopServer();
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.OnReceiveMatchingResponse) {
            int node = (int)param[0];
            StartUdpServer(node + NetConfig.GAME_PORT);
            RequestMatchingData();
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.OnReceiveMatchingResponse, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.OnReceiveMatchingResponse, this);
    }

    public void SetMatchingData(NetworkComponents.Matching.MatchingData data) {
        int index = 1;
        foreach (var d in data.nodes) {
            if (d.accountId == GlobalParameters.Param.accountId) {
                continue;
            }
            if (index == matchingSlots.Length) {
                Debug.LogError("Index out of range " + index);
                return;
            }
            var slot = matchingSlots[index];
            slot.SetPlayer(d.accountId, 1, d.playerName, false);
            slot.SetPlayerReady(d.isReady);
            index++;

        }
        for (; index < matchingSlots.Length; index++) {
            var slot = matchingSlots[index];
            slot.ClearPlayer();
        }
    }

    public void ClearMatchingData() {
        int index = 1;
        for (; index < matchingSlots.Length; index++) {
            matchingSlots[index].ClearPlayer();
        }
    }

    public void SetMatchingReadyState(int accountId, bool state) {
        foreach (var s in matchingSlots) {
            if (s.AccountId == accountId) {
                s.SetPlayerReady(state);
                break;
            }
        }
        //check ready state
        bool isStartable = true;
        foreach (var s in matchingSlots) {
            if (s.AccountId != -1) {
                isStartable &= s.IsReady;
            }
        }

        if (isStartable)
        {
            SetGameStarting(true);
            //send message to start game
            OnClickGameStart();
        }
        else {
            isStartable = false;
        }
    }

    public void SetGameStarting(bool state) {
        StartingPanel.alpha = state ? 1f : 0f;
    }

#endregion


    public void GetHttpRequest(string url)
    {
        WebClient webClient = new WebClient();
        Stream stream = webClient.OpenRead(url);
        result = new StreamReader(stream).ReadToEnd();
    }

    public void GetDataFromJson()
    {
        JsonData jsonResult = JsonMapper.ToObject(result);
        resultCode = int.Parse(jsonResult["result_code"].ToString());

        JsonData result_data = jsonResult["result_data"];
        resultMsg = jsonResult["result_msg"].ToString();

        string player = result_data["email"].ToString();

        string userData = JsonHelper.fixJson(result_data["user_list"].ToJson());
        Json.User[] users = JsonHelper.FromJson<Json.User>(userData);

        string hostIP = "";

        Debug.Log(userData);

        int[] order = new int[4];
        for (int i = 0; i < users.Length; i++)
        {
            if (users[i].role == "host")
            {
                hostIP = users[i].ip;
                OnHost();
            }
            else
            {
                MatchingPanel.Instance.SetHostAddress(hostIP);
                OnGuest();
            }


            if (users[i].email == player)
            {
                if (i > 0)
                {
                    int temp = order[0];
                    order[0] = i;
                    order[i] = temp;
                }
            }


        }


        for (int i = 0; i < 4; i++)
        {
            if(i > users.Length - 1)
            {
                matchingSlots[i] = new MatchingSlot();

            }
            else
            {
                matchingSlots[i].SetPlayer(users[i].id, users[i].level, users[i].nickname, false);
                matchingSlots[i].SetPlayerReady(false);

            }
        }
    }

    public void PostHttpRequest()
    {
        Debug.Log("PostHttpRequest");

        string email = GlobalParameters.Param.accountEmail;


        Json.User userData = new Json.User();

        userData.email = email;
        userData.ip = GetLocalHost();

        var jsonMsg = JsonUtility.ToJson(userData);

        WebManager.SendRequest(Json.RequestMethod.POST, "game/multiplay/join", jsonMsg);

        /*
        WebClient webClient = new WebClient();

        Json.User user = new Json.User();
        user.email = GlobalParameters.Param.accountEmail;
        user.ip = GetLocalHost();


        using (WebClient client = new WebClient())
        {
            Debug.Log("using WebClient");
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("email", user.email);
            reqparm.Add("ip", user.ip);

            Debug.Log(reqparm);

            byte[] responsebytes = client.UploadValues(url + "game/multiplay/join", "POST", reqparm);
            string responsebody = Encoding.UTF8.GetString(responsebytes);
        }
        */
    }

    void GoMatching() {
        PostHttpRequest();
    }

    void GetMultiplayLobby()
    {
        Debug.Log("GetMultiplayLobby");
        string email = GlobalParameters.Param.accountEmail;

        Debug.Log(email);

        GetHttpRequest(url + "game/multiplay/lobby?email=" + email);
        GetDataFromJson();
    }
}
