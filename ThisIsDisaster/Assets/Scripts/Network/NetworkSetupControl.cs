using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections;

using NetworkComponents;

public class NetworkSetupControl : MonoBehaviour, IObserver
{
    public enum Step {
        None = -1,

        Wait = 0,
        ServerStart,
        ServerConnect,
        ClientConnect,
        Prepare,
        Started,
        Error,
        Restart,

        Dummy,//for indexing
    }
    public CanvasGroup ControlGroup;
    public InputField AddressField;
    public InputField AccountField;
    public Text[] PlayerText;
    private bool _isHost = false;

    private Step _currentStep = Step.None;
    private Step _nextStep = Step.None;
    private float _waitTime = 1f;

    Timer _waitTimer = new Timer();
    bool _updateText = false;
    private int _udpServerPort = 0;

    delegate void DelayEvent();

    private DelayEvent _delayEvent = null;
    private Timer _delayEventTrigger = new Timer();
    
    // Use this for initialization
    void Start()
    {
        Initialize();
        UpdateAccountId();
        SetupPlayerTexts();
        Debug.Log(GetLocalHost());
        Debug.Log(GetHostAddress());
        ObserveNotices();
        //NetworkModule.Instance.RegisterReceiveNotification(PacketId.GameSyncInfo, OnReceiveGameSyncPacket);
    }

    public void Initialize()
    {
        AddressField.text = GetLocalHost();

        _currentStep = Step.None;
        _nextStep = Step.Wait;

        SetControl(true);
        gameObject.SetActive(true);
    }

    public void SetControl(bool isControllable) {
        ControlGroup.alpha = isControllable ? 1f : 0f;
        ControlGroup.blocksRaycasts = isControllable;
        ControlGroup.interactable = isControllable;
    }

    // Update is called once per frame
    void Update()
    {
        if (_delayEventTrigger.started) {
            if (_delayEventTrigger.RunTimer()) {
                if (_delayEvent != null) {
                    _delayEvent();
                    _delayEvent = null;
                }
            }
        }
        if (_updateText) {
            _updateText = false;
            SetupPlayerTexts();
        }
        if (Input.GetKeyDown(KeyCode.F5)) {
            NetworkModule.Instance.PrintNodeInfo();
        }
        if (_waitTimer.started)
        {
            _waitTimer.RunTimer();
        }

        while (_nextStep != Step.None) {
            _currentStep = _nextStep;
            _nextStep = Step.None;

            ExecuteStep();
            _waitTimer.StartTimer(_waitTime);
        }

        SetNextStep();
    }

    void ExecuteStep() {
        switch (_currentStep) {
            case Step.ServerStart:
                ServerStart();
                break;
            case Step.ServerConnect:
                ServerConnect();
                break;
            case Step.ClientConnect:
                ClientConnect();
                break;
            case Step.Restart:
                ResetNetwork();
                break;
        }

        SetupPlayerTexts();
    }

    void SetNextStep() {
        switch (_currentStep) {
            case Step.ServerStart:
                if (_waitTimer.started) return;
                _nextStep = Step.ServerConnect;
                break;
            case Step.ServerConnect:
                _nextStep = Step.ClientConnect;
                break;
            case Step.ClientConnect:
                _nextStep = Step.Prepare;
                break;
            case Step.Prepare:
                //check sync
                if (CheckGameSyncState()) {
                    _nextStep = Step.Started;
                    //SetControl(false);
                    //gen test character
                    SetupPlayerTexts();
                }
                break;
            case Step.Error:
                _nextStep = Step.Restart;

                SetControl(true);
                break;
            case Step.Wait:
            case Step.Restart:
                //do nothing
                break;
        }
    }

    void SetupPlayerTexts() {
        foreach (var t in PlayerText) {
            Color c = t.color;
            c = Color.black;
            t.color = c;
            t.text = "";
        }

        //if (_isHost)
        //{
        //    PlayerText[0].text = GlobalGameManager.Param.accountName;
        //    PlayerText[0].color = Color.red;
        //}
        //else {

        //}
        if (_isHost)
        {
            //PlayerText[0].text = GlobalGameManager.Param.accountName + System.Environment.NewLine + GetLocalHost();
            //PlayerText[0].color = Color.red;
            var list = NetworkModule.Instance.GetReliableEndPoints();
            for (int i = 0; i < list.Count; i++) {
                if (i > 3) {
                    Debug.LogError("Player count may higher than 4");
                    break;
                }
                PlayerText[i].text = list[i].ToString();
            }
        }
    }

    public void ServerStart()
    {
        NetworkModule.Instance.ClearReceiveNotification();
        GameServer.Instance.InitializeNetworkModule();
        int port = NetConfig.GAME_PORT + GlobalParameters.Param.accountId % 4;
        Debug.LogError("Starting Server : " + port);
        _udpServerPort = port;
        //UDP Server
        bool ret = NetworkModule.Instance.StartServer(port, NetConfig.PLAYER_MAX, NetworkModule.ConnectionType.UDP);
        if (_isHost) {
            ret &= NetworkModule.Instance.StartGameServer(NetConfig.PLAYER_MAX);//TCP Server
        }
        if (ret == false) {
            LogStepState(ret, Step.ServerStart);
            _currentStep = Step.Error;
        }
    }

    public void ServerConnect() {
        string hostAddr = "";
        if (_isHost) {
            hostAddr = GetLocalHost();
        }
        else {
            hostAddr = GetHostAddress();
        }
        int serverNode = NetworkModule.Instance.Connect(hostAddr, NetConfig.SERVER_PORT, NetworkModule.ConnectionType.TCP);
        if (serverNode >= 0)
        {
            NetworkModule.Instance.SetServerNode(serverNode);
            Debug.LogError("ServerNode : " + serverNode);
        }
        else
        {
            LogStepState(false, Step.ServerConnect);
            _currentStep = Step.Error;
        }

    }

    public void ClientConnect() {

        //should know members
        //int playerNum = NetConfig.PLAYER_MAX;
        //for (int i = 0; i < playerNum; i++) {

        //}

        /*
        int port = _isHost ? NetConfig.GAME_PORT + 1 : NetConfig.GAME_PORT;
        //port += GlobalParameters.Param.AdditionalPortNum;
        bool state = Connect(GetHostAddress(), port, NetworkModule.ConnectionType.UDP);
        LogStepState(state, Step.ClientConnect);
        if (!state) {
            _currentStep = Step.Error;
        }*/
    }

    public void OnClickHost() {
        _isHost = true;
        //_nextStep = Step.ServerStart;
        //Make Listening Server
        ServerStart();
        //ServerConnect();
    }

    public void OnClickConnect()
    {
        _isHost = false;
        //GlobalParameters.Param.AdditionalPortNum += 10;
        //_nextStep = Step.ServerStart;
        ServerConnect();
        ServerStart();

        _delayEvent = new DelayEvent(SendSessionSyncInfo);
        //SendSessionSyncInfo();
        _delayEventTrigger.StartTimer(1f);
    }

    public void SendSessionSyncInfo() {
        var net = NetworkModule.Instance;
        if (net != null) {
            SessionSyncInfo info = new SessionSyncInfo() {
                accountId = GlobalGameManager.Param.accountId,
                serverPort = _udpServerPort,
                ipLength = GetLocalHost().Length,
                ip = GetLocalHost()
            };

            SessionSyncInfoPacket packet = new SessionSyncInfoPacket(info);
            int sendSize = net.SendReliable(net.GetServerNode(), packet);
            if (sendSize > 0) {
                NetDebug.Log("Sending Session Sync Info");
            }
        }
    }

    public void OnClickSync() {
        SendSessionSyncInfo();
    }

    public void OnClickGameStart() {
        GlobalGameManager.Instance.OnGameStart();
        SetControl(false);
    }

    bool StartServer(int port, int connectionMax, NetworkModule.ConnectionType connectType) {
        return NetworkModule.Instance.StartServer(port, connectionMax, connectType);
    }

    bool Connect(string addr, int port, NetworkModule.ConnectionType type) {

        return NetworkModule.Instance.Connect(addr, port, type) > -1;
    }

    string GetHostAddress() {
        ///TODO : add some check ip structure
        return AddressField.text;
    }

    string GetLocalHost() {
        //string hostAddress = "";
        //string hostName = Dns.GetHostName();
        //IPAddress[] addrList = Dns.GetHostAddresses(hostName);
        //var firstIPv4Address = addrList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        //hostAddress = firstIPv4Address.ToString();
        //return hostAddress;

        return Network.player.ipAddress.ToString();
    }

    void ResetNetwork() {
        var net = NetworkModule.Instance;
        net.Disconnect();
        net.StopGameServer();
        net.StopServer();

        SetControl(true);
    }

    public bool CheckGameSyncState() {
        return true;
    }

    void LogStepState(bool state, Step step) {
        if (state)
        {
            NetDebug.Log("Success to " + step.ToString());
        }
        else {
            NetDebug.LogError("Failed to " + step.ToString());
        }
    }

    public void OnSetAccount(string account) {
        GlobalGameManager.Param.accountName = account;
        if (GameManager.CurrentGameManager != null) {
            if (GameManager.CurrentGameManager.GetLocalPlayer() != null)
                GameManager.CurrentGameManager.GetLocalPlayer().SetUnitName(GlobalParameters.Param.accountName);
        }
    }

    void UpdateAccountId()
    {
        //change this network user id
        //GlobalGameManager.Param.accountId = System.Diagnostics.Process.GetCurrentProcess().Id;
    }

    public void UpdateAccountId(string input) {
        GlobalGameManager.Param.accountId = int.Parse(input);
    }

    public void OnReceiveGameSyncPacket(PacketId id, int packetSender, byte[] data) {
        Debug.LogError("Received Sync Info" + packetSender);
        GameSyncPacket packet = new GameSyncPacket(data);
        GameSyncData sync = packet.GetPacket();


        if (packetSender == GlobalParameters.Param.accountId) return;

        //StageGenerator.Instance.SetSeed(sync.stageGenSeed);

        //var remoteCharacter = GameManager.MakePlayerCharacter(sync.accountName, packetSender, false);
        //remoteCharacter.SetUnitName(sync.accountName);

        Debug.Log("Received Packet Info : " + sync.accountName + " " + sync.accountId);
        //make remote char
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.OnPlayerConnected) {
            _updateText = true;
            //SetupPlayerTexts();
        }
        if (notice == NoticeName.OnPlayerDisconnected)
        {
            _updateText = true;
            //SetupPlayerTexts();
        }

        if (notice == NoticeName.OnStartSession) {
            SetControl(false);
        }
    }

    public void ObserveNotices()
    {
        Notice.Instance.Observe(NoticeName.OnPlayerConnected, this);
        Notice.Instance.Observe(NoticeName.OnPlayerDisconnected, this);
        Notice.Instance.Observe(NoticeName.OnStartSession, this);
    }

    public void RemoveNotices()
    {
        Notice.Instance.Remove(NoticeName.OnPlayerConnected, this);
        Notice.Instance.Remove(NoticeName.OnPlayerDisconnected, this);
        Notice.Instance.Remove(NoticeName.OnStartSession, this);
    }
}
