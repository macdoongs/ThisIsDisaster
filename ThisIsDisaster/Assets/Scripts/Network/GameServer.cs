using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

        private const int KEY_MASK = NetConfig.PLAYER_MAX;

        private int _playerNum = 0;
        private int _currentPartMask = 0;
        private bool _syncFlag = false;
        const string ITEM_OWNER_NONE = "";

        private float[] _keepAlive = new float[NetConfig.PLAYER_MAX];
        private const float KEEPALIVE_TIMEOUT = 10.0f;

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
        }

        private void Update()
        {
            EventHandling();
        }

        public bool StartServer(int playerNum) {
            _playerNum = playerNum;

            return Network.StartServer(NetConfig.SERVER_PORT, NetConfig.PLAYER_MAX, NetworkModule.ConnectionType.TCP);
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
                        NetDebug.Log("[Server] NET DISCONNECT");
                        DisconnectClient(state.node);
                        break;
                }
            }

        }

        public void OnReceiveSessionInfoSyncPacket(int node, PacketId id, byte[] data) {
            NetDebug.LogError("Received");
            SessionSyncInfoPacket packet = new SessionSyncInfoPacket(data);
            var info = packet.GetPacket();
            string log = string.Format("Session Sync Info From [{0}]\r\n{1}:{2}", info.accountId, info.ip, info.serverPort);
            NetDebug.LogError(log);

            SendConnectionReflection(node, info, true);

            //send reliable all
        }

        void SendConnectionReflection(int node, SessionSyncInfo info, bool isConnection) {

            SessionSyncInfoReflection reflection = new SessionSyncInfoReflection()
            {
                nodeIndex = node,
                isConnection = isConnection,
                nodeAccountId = info.accountId,
                nodeServerPort = info.serverPort,
                ipLength = info.ipLength,
                nodeIp = info.ip
            };

            SessionSyncInfoReflectionPacket packet = new SessionSyncInfoReflectionPacket(reflection);
            Network.SendReliableToAll(packet);
        }

        public void SendDisconnectReflection(int node) {
            string dis = "disconnect";
            SessionSyncInfoReflection reflection = new SessionSyncInfoReflection() {
                nodeIndex = node,
                isConnection = false,
                nodeAccountId = 0,
                nodeServerPort = 0,
                ipLength = dis.Length,
                nodeIp = dis
            };

            SessionSyncInfoReflectionPacket packet = new SessionSyncInfoReflectionPacket(reflection);
            Network.SendReliableToAll(packet);
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
                    _reflectionDics[info.nodeIndex] = info;
                }
                else
                {
                    _reflectionDics.Add(info.nodeIndex, info);
                }
            }
            else {
                _reflectionDics.Remove(info.nodeIndex);
            }
        }

        public void OnReceiveSessionStartNotice(int node, PacketId id, byte[] data)
        {
            StartSessionNoticePacket packet = new StartSessionNoticePacket(data);
            var notice = packet.GetPacket();
            NetDebug.LogError("Start Session, Seed : " + notice.stageRandomSeed);
            StageGenerator.Instance.SetSeed(notice.stageRandomSeed);
            Notice.Instance.Send(NoticeName.OnStartSession);
            //connect all guests for mesh topology
            GlobalGameManager.Instance.GenerateWorld();
        }

        public void OnNotice(string notice, params object[] param)
        {
            if (notice == NoticeName.OnPlayerDisconnected) {
                
            }
        }

        public void ObserveNotices()
        {
            Notice.Instance.Observe(NoticeName.OnPlayerDisconnected, this);
        }

        public void RemoveNotices()
        {
            Notice.Instance.Remove(NoticeName.OnPlayerDisconnected, this);
        }
    }

}