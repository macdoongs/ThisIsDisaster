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
        private struct ItemState {
            public string itemId;
            public PickupState state;
            public string ownerId;
        }

        public static GameServer Instance {
            private set;
            get;
        }

        const string ITEM_OWNER_NONE = "";
        static int _serverPort = NetConfig.SERVER_PORT;
        const int SERVER_VERSION = 1;
        private bool _sendGameSync = false;

        NetworkModule _network = null;
        NetworkModule Network {
            get {
                if (_network == null) {
                    _network = NetworkComponents.NetworkModule.Instance;
                }
                return _network;
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance.gameObject != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //Network.RegisterReceiveNotification(PacketId.GameSyncInfo, )
            Network.RegisterEventHandler(OnEventHandling);
        }

        private void LateUpdate()
        {
            if (_sendGameSync) {
                //SendGameSyncInfo();
                //_sendGameSync = false;
            }
        }

        public bool StartServer() {
            return Network.StartServer(_serverPort, NetworkModule.ConnectionType.TCP);
        }

        public void StopServer() {
            Network.StopServer();
        }

        public void DisconnectClient() {
            Network.Disconnect();
        }

        public void SendGameSyncInfo() {
            Debug.LogError("Send Sync Info");
            GameSyncData data = new GameSyncData();
            data.serverVersion = SERVER_VERSION;
            data.accountName = GlobalGameManager.Param.accountName;
            data.accountId = GlobalGameManager.Param.accountId;

            GameSyncPacket packet = new GameSyncPacket(data);
            Network.SendReliable(packet);
        }

        public void OnEventHandling(NetEventState state) {
            switch (state.type) {
                case NetEventType.Connect:
                    SendGameSyncInfo();
                    //_sendGameSync = true;
                    break;
                case NetEventType.Disconnect:
                    DisconnectClient();
                    break;
            }
        }
    }
}