using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetworkComponents;

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

    private void Awake()
    {
        Instance = this;
        if (GlobalGameManager.Instance != null) { }
    }

    private void Start()
    {
        //zero is account
        _slotAccount.Add(0, GlobalParameters.Param.accountId);
    }

    private void Update()
    {
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

    public void OnOpenPanel() {
        Show();

        //zero is localplayer
        matchingSlots[0].SetPlayer(GlobalParameters.Param.accountId, 1, GlobalParameters.Param.accountName, true);

    }

    public void OnClosePanel() {
        //stop
        Hide();
        LobbyUIScript.Instance.DefaultMenu();
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

    string GetHostAddress()
    {
        return GetLocalHost();//dummy
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
        GameServer.Instance.SetLocalAddress(GetLocalHost());
        //StartUdpServer();
        StartUdpServer(NetConfig.GAME_PORT);
        StartHostServer();
        GameServer.Instance.MakeMatchingView();
    }

    public void OnGuest() {
        SetIsHost(false);
        GameServer.Instance.SetLocalAddress(GetLocalHost());

        ConnectToHost();
        GameServer.Instance.MakeMatchingView();

        SendMatchingRequest();
        //_delayEvent = new DelayEvent(SendMatchingRequest);
        //_delayEventTrigger.StartTimer(1f);
        //StartUdpServer();
        //GameServer.Instance.SendMatchingRequest();
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

    public void OnClickGameStart() {
        if (!_isHost) return;
        GlobalGameManager.Instance.OnGameStart();
        GameServer.Instance.DestroyMatchingView();
    }

    public void SetIsHost(bool isHost) {
        _isHost = isHost;
        GameServer.Instance.SetHost(isHost);
    }

    void ResetNetwork() {
        NetworkModule.Instance.Disconnect();
        NetworkModule.Instance.StopGameServer();
        NetworkModule.Instance.StopServer();
    }

    public void OnNotice(string notice, params object[] param)
    {
        if (notice == NoticeName.OnReceiveMatchingResponse) {
            int node = (int)param[0];
            StartUdpServer(node);
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
            slot.SetPlayer(d.accountId, 1, NetworkComponents.GameServer.Instance.GetPlayerName(d.nodeIndex), false);
            index++;

        }
    }

    #endregion
}
