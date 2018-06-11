using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using NetworkComponents.Packet;

namespace NetworkComponents {
    //itemstate
    enum PickupState
    {
        Generation = 0,
        None,
        PickingUp,
        Picked,
        Dropping,
        Dropped
    }

    public enum GameServerRequestType {
        MatchingData,
        UpdateGuestPort,
        ConnectionState,
        SceneLoadCompleted,
    }

    public class GameServer : MonoBehaviour, IObserver {
        public static GameServer Instance {
            private set;
            get;
        }

        public const int SERVER_VERSION = NetConfig.SERVER_VERSION;
        NetworkModule _network = null;
        NetworkModule Network {
            get {
                if (_network == null) {
                    _network = NetworkModule.Instance;
                }
                return _network;
            }
        }
        Dictionary<int, int> _nodes = new Dictionary<int, int>();

        Dictionary<int, SessionSyncInfoReflection> _reflectionDics = new Dictionary<int, SessionSyncInfoReflection>();

        private List<int> _loadigWaitingNodes = new List<int>();

        private const int KEY_MASK = NetConfig.PLAYER_MAX;

        private int _playerNum = 0;
        private int _currentPartMask = 0;
        private bool _syncFlag = false;
        const string ITEM_OWNER_NONE = "";

        private float[] _keepAlive = new float[NetConfig.PLAYER_MAX];
        private const float KEEPALIVE_TIMEOUT = 10.0f;
        private Matching.MatchingView _matchingCTRL = null;

        Dictionary<int, string> _playerNames = new Dictionary<int, string>();

        public bool IsHost {
            private set;
            get;
        }

        public int UdpServerPort {
            private set;
            get;
        }

        public string LocalHost {
            private set;
            get;
        }

        private SessionInfo _currentSessionInfo;

        private void Awake()
        {
            if (Instance != null && Instance.gameObject != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ObserveNotices();   
        }

        private void Start()
        {
            InitializeNetworkModule();
        }

        public void InitializeNetworkModule() {
            Network.RegisterReceiveNotification(PacketId.SessionInfoSync, OnReceiveSessionInfoSyncPacket);
            Network.RegisterReceiveNotification(
                PacketId.SessionInfoSyncReflection, OnReceiveSessionInfoSyncReflectionPacket);
            Network.RegisterReceiveNotification(PacketId.StartSessionNotify, OnReceiveSessionStartNotice);

            Network.RegisterReceiveNotification(PacketId.MatchingRequest, OnReceiveMatchingRequest);
            Network.RegisterReceiveNotification(PacketId.MatchingResponse, OnReceiveMatchingResponse);
            Network.RegisterReceiveNotification(PacketId.GameServerRequest, OnReceiveGameServerRequest);
            Network.RegisterReceiveNotification(PacketId.MatchingReady, OnReceiveMatchingReadyState);
            Network.RegisterReceiveNotification(PacketId.StageStartTime, OnReceiveStageStartTime);
            Network.RegisterReceiveNotification(PacketId.PlayerAnimTrigger, OnReceivePlayerAnimTrigger);
            Network.RegisterReceiveNotification(PacketId.PlayerItemInfo, OnReceivePlayerItemInfo);
            Network.RegisterReceiveNotification(PacketId.PlayerStateInfo, OnReceivePlayerStateInfo);
            //Network.RegisterReceiveNotification(PacketId.Coordinates, GameManager.CurrentGameManager.OnReceiveCharacterCoordinate);
        }

        private void Update()
        {
            EventHandling();
        }

        public void SetHost(bool state) {
            IsHost = state;
        }

        public bool StartServer(int playerNum) {
            _playerNum = playerNum;
            return Network.StartServer(NetConfig.SERVER_PORT, NetConfig.PLAYER_MAX, NetworkModule.ConnectionType.TCP);
        }

        public void SetUDPServerPort(int portNum) {
            this.UdpServerPort = portNum;
            NetDebug.LogError("Set udp server port : " + portNum);
        }

        public void SetLocalAddress(string addr) {
            LocalHost = addr;
        }

        public void StopServer() {
            Network.StopServer();
        }

        public void OnReceiveReflectionPacket(int node, PacketId id, byte[] data) {
            Network.SendReliableToAll(id, data);
        }

        private void DisconnectClient(int node) {
            
            Network.Disconnect(node);
            if (!_nodes.ContainsKey(node)){
                return;
            }

            int gid = _nodes[node];
            _currentPartMask &= ~(1 << gid);

            _nodes.Remove(node);

            //send disconnect message
        }

        public void EventHandling()
        {
            NetEventState state = Network.GetEventState();
            if (state != null) {
                switch (state.type) {
                    case NetEventType.Connect:
                        NetDebug.Log("[Server] NET CONNECT");
                        break;
                    case NetEventType.Disconnect:
                        NetDebug.Log("[Server] NET DISCONNECT " + state.node);
                        if (state.node == _network.GetServerNode()) {
                            //disconnect from host
                            NetDebug.LogError("Disconnect From Host");
                            if (_matchingCTRL) {
                                DestroyMatchingView();
                                if (MatchingPanel.Instance) {
                                    MatchingPanel.Instance.OnClosePanel();
                                    MatchingPanel.Instance.ClearMatchingData();
                                }
                            }

                            Network.Disconnect();
                            Network.StopGameServer();
                            
                            return;
                        }
                        if (_matchingCTRL) {
                            _matchingCTRL.RemoveNode(state.node);
                        }
                        SendDisconnectReflection(state.node);
                        DisconnectClient(state.node);

                        if (_matchingCTRL) {
                            _matchingCTRL.GetMatchingData();//for update ui
                        }
                        break;
                }
            }

        }

        #region Receive
        public void OnReceiveSessionInfoSyncPacket(int node, PacketId id, byte[] data) {
            NetDebug.LogError("Received");
            SessionSyncInfoPacket packet = new SessionSyncInfoPacket(data);
            var info = packet.GetPacket();
            string log = string.Format("Session Sync Info From [{0}]\r\n{1}:{2}", info.accountId, info.ip, info.serverPort);
            NetDebug.LogError(log);

            SendConnectionReflection(node, info, true);

            //send reliable all
        }
        
        public void OnReceiveSessionInfoSyncReflectionPacket(int node, PacketId id, byte[] data) {
            SessionSyncInfoReflectionPacket packet = new SessionSyncInfoReflectionPacket(data);
            var info = packet.GetPacket();
            if (GlobalParameters.Param.accountId == info.nodeAccountId) {
                NetDebug.LogError("Get Reflection by own");
                return;
            }
            string log = string.Format("Session Sync Info \"{4}\" From [{0}:{3}]\r\n{1}:{2}", info.nodeIndex, info.nodeIp, info.nodeServerPort, info.nodeAccountId, info.isConnection);
            NetDebug.LogError(log);

            if (info.isConnection)
            {
                if (_reflectionDics.ContainsKey(info.nodeIndex))
                {
                    Debug.Log("AddInfo " + info.nodeIndex);
                    _reflectionDics[info.nodeIndex] = info;
                }
                else
                {
                    Debug.Log("AddInfo2 " + info.nodeIndex);
                    _reflectionDics.Add(info.nodeIndex, info);
                }
            }
            else
            {
                Debug.Log("Remove " + info.nodeIndex);
                NetDebug.LogError("Disconnect NODE : " + info.nodeIndex);

                if (_matchingCTRL) {
                    SendGameServerRequest(GameServerRequestType.MatchingData);
                }
                _reflectionDics.Remove(info.nodeIndex);
            }
        }

        public void OnReceiveSessionStartNotice(int node, PacketId id, byte[] data)
        {
            StartSessionNoticePacket packet = new StartSessionNoticePacket(data);
            var notice = packet.GetPacket();
            NetDebug.LogError("Start Session, Seed : " + notice.stageRandomSeed);
            ConnectUDPServer();
            StageGenerator.Instance.SetSeed(notice.stageRandomSeed);
            Notice.Instance.Send(NoticeName.OnStartSession);
            GlobalGameManager.Instance.LoadGameScene();


            return;
        }

        public void OnReceiveMatchingRequest(int node, PacketId id, byte[] data) {
            MatchingRequestPacket packet = new MatchingRequestPacket(data);
            var request = packet.GetPacket();

            //_matchingView.AddNode(node, request.accountId, request.accountLength, request.accountName);

            NetDebug.LogError(string.Format("Matching Request From node [{0}]\r\n{1}-{2}", node, request.accountId, request.accountName));
            if (_playerNames.ContainsKey(node))
            {
                _playerNames[node] = request.accountName;
            }
            else {
                _playerNames.Add(node, request.accountName);
            }
            //check smth
            MatchingResponse response = new MatchingResponse()
            {
                connectionState = true,
                nodeIndex = node + 1,
                sessionIndex = -1
            };
            SendMatchingResponse(node, response);
            var ep = NetworkModule.Instance.GetReliableEndPoint(node);
            if (ep != null) {
                _matchingCTRL.AddNode(node, request.accountId,
                    response.nodeIndex + NetConfig.GAME_PORT, ep.Address.ToString(), request.accountName);
            }
        }

        public void OnReceiveMatchingResponse(int node, PacketId id, byte[] data) {
            MatchingResponsePacket packet = new MatchingResponsePacket(data);
            var response = packet.GetPacket();
            NetDebug.LogError("Matching Response : " + response.nodeIndex + " " + response.connectionState + " " + response.sessionIndex);
            if (response.connectionState) {
                Notice.Instance.Send(NoticeName.OnReceiveMatchingResponse, response.nodeIndex, response.sessionIndex);
                
            }
            
        }

        public void OnReceiveGameServerRequest(int node, PacketId id, byte[] data) {
            GameServerRequestPacket packet = new GameServerRequestPacket(data);
            var req = packet.GetPacket();
            NetDebug.Log("Recieved Request " + req.request);
            switch (req.request) {
                case GameServerRequestType.MatchingData:
                    //broadcast to all
                    SendMatchingData();
                    break;
                case GameServerRequestType.SceneLoadCompleted:
                    OnNodeLoadCompleted(node);
                    break;
            }
        }

        public void OnReceiveMatchingReadyState(int node, PacketId id, byte[] data) {
            MatchingReadyStatePacket packet = new MatchingReadyStatePacket(data);
            var red = packet.GetPacket();
            NetDebug.LogError("Received Ready State: " + node);
            if (_matchingCTRL)
            {
                _matchingCTRL.SetNodeReadyState(red.accountId, red.isReady);
            }
            if (MatchingPanel.Instance) {
                MatchingPanel.Instance.SetMatchingReadyState(red.accountId, red.isReady);
            }
            SendMatchingData();
        }

        public void OnReceiveStageStartTime(int node, PacketId packetId, byte[] data) {
            StageStartTimePacket packet = new StageStartTimePacket(data);
            StageStartTime time = packet.GetPacket();
            GameManager.CurrentGameManager.StartMultiStage(time.startTime);
        }

        public void OnReceivePlayerAnimTrigger(int node, PacketId packetId, byte[] data) {
            PlayerAnimTriggerPacket packet = new PlayerAnimTriggerPacket(data);
            PlayerAnimTrigger trigger = packet.GetPacket();
            if (GameManager.CurrentGameManager != null) {
                GameManager.CurrentGameManager.OnRemoteChararcterAnimTrigger(node, trigger.animKey);
            }
        }

        public void OnReceivePlayerItemInfo(int node, PacketId packetId, byte[] data) {
            PlayerItemInfoPacket packet = new PlayerItemInfoPacket(data);
            PlayerItemInfo info = packet.GetPacket();

            if (info.accountId == GlobalParameters.Param.accountId) return;

            if (GameManager.CurrentGameManager != null) {
                GameManager.CurrentGameManager.OnRemoteCharacterItemInfoSetted(info);
            }
            if (IsHost) {
                SendPlayerItemInfo(packet);
            }

        }

        public void OnReceivePlayerStateInfo(int node, PacketId packetId, byte[] data) {
            PlayerStateInfoPacket packet = new PlayerStateInfoPacket(data);
            PlayerStateInfo info = packet.GetPacket();

            if (GameManager.CurrentGameManager != null) {

            }
        }
        #endregion


        #region Send

        void SendMatchingResponse(int node, MatchingResponse response)
        {

            MatchingResponsePacket packet = new MatchingResponsePacket(response);
            Network.SendReliable(node, packet);
            
        }

        void SendConnectionReflection(int node, SessionSyncInfo info, bool isConnection)
        {

            SessionSyncInfoReflection reflection = new SessionSyncInfoReflection()
            {
                nodeIndex = node,
                isConnection = isConnection,
                nodeAccountId = info.accountId,
                nodeServerPort = info.serverPort,
                ipLength = info.ipLength,
                nodeIp = info.ip
            };

            if (_reflectionDics.ContainsKey(node))
            {
                Debug.Log("AddInfo1 " + node);
                _reflectionDics[node] = reflection;
            }
            else
            {
                Debug.Log("AddInfo2 " + node);
                _reflectionDics.Add(node, reflection);
            }

            SessionSyncInfoReflectionPacket packet = new SessionSyncInfoReflectionPacket(reflection);
            Network.SendReliableToAll(packet);
        }

        public void SendDisconnectReflection(int node)
        {
            string dis = "disconnect";
            SessionSyncInfoReflection reflection = new SessionSyncInfoReflection()
            {
                nodeIndex = node,
                isConnection = false,
                nodeAccountId = 0,
                nodeServerPort = 0,
                ipLength = dis.Length,
                nodeIp = dis
            };

            if (_reflectionDics.ContainsKey(node))
            {
                Debug.Log("Remove " + node);
                _reflectionDics.Remove(node);
            }

            SessionSyncInfoReflectionPacket packet = new SessionSyncInfoReflectionPacket(reflection);
            Network.SendReliableToAll(packet);
        }

        public void SendMatchingRequest() {
            MatchingRequest request = new MatchingRequest() {
                accountId = GlobalParameters.Param.accountId,
                accountLength = GlobalParameters.Param.accountName.Length,
                accountName = GlobalParameters.Param.accountName
            };

            MatchingRequestPacket packet = new MatchingRequestPacket(request);
            Network.SendReliable(Network.GetServerNode(), packet);
           // NetDebug.LogError("Sending Matching Request");
        }

        public void SendMatchingData() {
            if (_matchingCTRL != null) {
                var data = _matchingCTRL.GetMatchingData();
                MatchingDataPacket packet = new MatchingDataPacket(data);
                Network.SendReliableToAll(packet);
            }
        }

        public void SendGameServerRequest(GameServerRequestType type) {
            //NetDebug.LogError("Make Request " + type);
            GameServerRequest request = new GameServerRequest() { request = type };
            GameServerRequestPacket packet = new GameServerRequestPacket(request);
            Network.SendReliable(Network.GetServerNode(), packet);
        }

        public void SendMatchingReady(bool state) {
            Matching.MatchingReadyState ready = new Matching.MatchingReadyState()
            {
                isReady = state,
                accountId = GlobalParameters.Param.accountId,
            };
            MatchingReadyStatePacket packet = new MatchingReadyStatePacket(ready);
            Network.SendReliable(Network.GetServerNode(), packet);
        }

        public void SendStageStartTime(DateTime startTime) {
            StageStartTime stageStart = new StageStartTime() { startTime = startTime };
            StageStartTimePacket packet = new StageStartTimePacket(stageStart);
            Network.SendReliableToAll(packet);
        }

        public void SendPlayerAnimTrigger(string animation) {
            PlayerAnimTrigger trigger = new PlayerAnimTrigger();
            trigger.playerId = GlobalGameManager.Param.accountId;
            trigger.animKey = animation;
            trigger.animKeyLength = animation.Length;

            PlayerAnimTriggerPacket packet = new PlayerAnimTriggerPacket(trigger);
            Network.SendUnreliableToAll(packet);
        }

        public void SendPlayerItemAcquire(int playerItem, bool isAcquire) {
            PlayerItemInfo info = new PlayerItemInfo()
            {
                accountId = GlobalGameManager.Param.accountId,
                itemId = playerItem,
                isAcquire = isAcquire
            };

            PlayerItemInfoPacket packet = new PlayerItemInfoPacket(info);

            if (IsHost) {
                SendPlayerItemInfo(packet);
            }
            else  Network.SendReliable(Network.GetServerNode(), packet);
        }

        void SendPlayerItemInfo(PlayerItemInfoPacket packet) {
            Network.SendReliableToAll(packet);
        }

        public void SendPlayerStateInfo(int health, bool isDead) {
            PlayerStateInfo info = new PlayerStateInfo() {
                accountId = GlobalParameters.Param.accountId,
                playerHp = health,
                isPlayerDead = isDead
            };

            PlayerStateInfoPacket packet = new PlayerStateInfoPacket(info);
            Network.SendUnreliableToAll(packet);
        }
        #endregion

        public void MakeMatchingView() {
            GameObject g = new GameObject("Matching");
            g.transform.SetParent(transform);
            g.transform.localPosition = Vector3.zero;
            g.transform.localScale = Vector3.one;

            Matching.MatchingView view = g.AddComponent<Matching.MatchingView>();

            view.SessionId = DateTime.Now.Ticks;
            view.ServerAccountId = GlobalParameters.Param.accountId;
            view.AddHost();
            
            this._matchingCTRL = view;
        }

        public void DestroyMatchingView() {
            if (_matchingCTRL != null) {
                Destroy(_matchingCTRL);
            }
        }

        public string GetPlayerName(int nodeIndex) {
            string output = "";
            _playerNames.TryGetValue(nodeIndex, out output);
            return output;
        }

        public void SetHostReady(bool state) {
            if (_matchingCTRL) {
                _matchingCTRL.SetNodeReadyState(GlobalParameters.Param.accountId, state);
            }
            if (MatchingPanel.Instance) {
                MatchingPanel.Instance.SetMatchingReadyState(GlobalParameters.Param.accountId, state);
            }
            SendMatchingData();
        }

        public void OnNotice(string notice, params object[] param)
        {
            if (notice == NoticeName.OnPlayerDisconnected)
            {

            }
            if (notice == NoticeName.OnPlayerConnected)
            {
                if (IsHost)
                {
                    int node = (int)param[0];
                    //SendMatchingResponse(node);
                }
            }
        }

        public void ObserveNotices()
        {
            Notice.Instance.Observe(NoticeName.OnPlayerDisconnected, this);
            Notice.Instance.Observe(NoticeName.OnPlayerConnected, this);
        }

        public void RemoveNotices()
        {
            Notice.Instance.Remove(NoticeName.OnPlayerDisconnected, this);
            Notice.Instance.Remove(NoticeName.OnPlayerConnected, this);
        }

        void PrintRefInfo()
        {
            NetDebug.Log("PrintReflection");
            foreach (var kv in _reflectionDics)
            {
                NetDebug.Log(string.Format("[{0}]{1}:{2}", kv.Key, kv.Value.nodeIndex, kv.Value.nodeServerPort));
            }
        }

        public void ConnectUDPServer()
        {

            var list = _matchingCTRL._nodes;
            foreach (var node in list)
            {
                //connect
                if (node.accountId == GlobalParameters.Param.accountId) continue;
                NetDebug.LogError("Connect UDP " + node.accountId + " " + node.port);
                int clientNode = Network.Connect(node.ip, node.port, NetworkModule.ConnectionType.UDP);
                if (clientNode >= 0)
                {
                    if (node.nodeIndex == -1) {
                        if (GlobalGameManager.Instance.IsHost == false)
                        {
                            Network.SetClientNode(0, clientNode);
                            
                            GlobalGameManager.Instance.AddRemotePlayer(10000, node);
                            continue;
                        }
                    }
                    
                    Network.SetClientNode(node.nodeIndex, clientNode);
                    GlobalGameManager.Instance.AddRemotePlayer(clientNode, node);
                }
                else
                {
                    string err = "Error in Establishing Unreliable Connection : " + node.accountId;
                    NetDebug.LogError(err);
                }

            }

        }

        public SessionSyncInfoReflection GetSessionPlayerInfo(int node)
        {
            SessionSyncInfoReflection output;
            if (_reflectionDics.TryGetValue(node, out output))
            {

            }
            return output;
        }

        public void MakeRemotePlayer()
        {
            if (GameManager.CurrentGameManager == null) return;
            /*
            foreach (var kv in _reflectionDics)
            {
                string name = kv.Value.nodeIp;
                int index = kv.Key;

                GameManager.MakePlayerCharacter(name, index, false);
            }

            if (!IsHost)
            {
                GameManager.MakePlayerCharacter("host", Network.GetServerNode(), false);
            }*/

        }

        public void MakeNewSession()
        {
            _currentSessionInfo = new SessionInfo()
            {
                playerId = 0,
                state = MatchingState.Connect,
            };
        }

        public void OnStartLoadingGame() {
            _loadigWaitingNodes.AddRange(_nodes.Keys);
        }

        public void OnNodeLoadCompleted(int node) {
            if (_loadigWaitingNodes.Contains(node)) {
                _loadigWaitingNodes.Remove(node);
            }
            
        }

        public bool IsLoadEnded() {
            return _loadigWaitingNodes.Count == 0;
        }
    }

}