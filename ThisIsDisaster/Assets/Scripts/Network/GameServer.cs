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

    public class GameServer : MonoBehaviour {
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
        }

        void InitializeNetworkModule() {

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
    }

}