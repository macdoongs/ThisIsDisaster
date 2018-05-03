using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using UnityEngine;

namespace NetworkComponents
{
    public delegate void EventHandler(NetEventState state);
    public class NetConfig
    {
        public static int PLAYER_MAX = 4;
        public static int SERVER_PORT = 50764;
        public static int GAME_PORT = 50765;

        public const int PACKET_SIZE = 1400;
    }

    public enum NetEventType
    {
        Connect = 0,
        Disconnect,
        SendError,
        RecvError
    }

    public enum NetEventResult
    {
        Failure = -1,
        Success = 0
    }

    public class NetEventState
    {
        public NetEventType type;
        public NetEventResult result;
        public IPEndPoint endPoint;

        public NetEventState() { }

        public NetEventState(NetEventType type, NetEventResult result) {
            this.type = type;
            this.result = result;
        }
    }
}