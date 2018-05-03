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

        const string ITEM_OWNER_NONE = "";
        static int _serverPort = NetConfig.SERVER_PORT;
        const int SERVER_VERSION = 1;

        NetworkModule _network = null;


        public void StartServer() {

        }
    }
}