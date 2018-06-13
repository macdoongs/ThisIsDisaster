using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

namespace NetworkComponents
{
    public class NetworkModule : MonoBehaviour
    {
        private class NodeInfo {
            public int node;
            public NodeInfo(int node) {
                this.node = node;
            }
        }
        public delegate void RecvNotifier(int node, PacketId id, byte[] data);
        public enum ConnectionType {
            /// <summary>
            /// Reliable
            /// </summary>
            TCP = 0,
            
            /// <summary>
            /// Unreliable
            /// </summary>
            UDP,


            BOTH
        }

        public static NetworkModule Instance
        {
            private set;
            get;
        }

        private SessionTCP _sessionTCP = null;
        private SessionUDP _sessionUDP = null;
        private int _serverNode = -1;
        private int[] _clientNode = new int[NetConfig.PLAYER_MAX];
        private NodeInfo[] _reliableNode = new NodeInfo[NetConfig.PLAYER_MAX];
        private NodeInfo[] _unreliableNode = new NodeInfo[NetConfig.PLAYER_MAX];

        private const int _packetSize = NetConfig.PACKET_SIZE;
        private Dictionary<int, RecvNotifier> _notifier = new Dictionary<int, RecvNotifier>();
        private List<NetEventState> _eventQueue = new List<NetEventState>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject); return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        void Initialize() {
            _sessionTCP = new SessionTCP();
            _sessionTCP.RegisterEventHnadler(OnEventHandlingReliable);
            _sessionUDP = new SessionUDP();
            _sessionUDP.RegisterEventHnadler(OnEventHandlingUnreliable);
            for (int i = 0; i < _clientNode.Length; i++) {
                _clientNode[i] = -1;
            }
        }

        private void Update()
        {
            byte[] packet = new byte[_packetSize];
            for (int id = 0; id < _reliableNode.Length; id++) {
                if (_reliableNode[id] != null) {
                    int node = _reliableNode[id].node;
                    if (IsConnected(node)) {
                        int recvSize = -1;
                        while ((recvSize = _sessionTCP.Receive(node, ref packet)) > 0) {
                            NetDebug.Log("RecvSize : " + recvSize.ToString());
                            Receive(node, packet);
                        }
                    }
                }
            }

            for (int id = 0; id < _unreliableNode.Length; id++) {
                if (_unreliableNode[id] != null) {
                    int node = _unreliableNode[id].node;
                    if (IsConnected(node)) {
                        while (_sessionUDP.Receive(node, ref packet) > 0) {
                            Receive(node, packet);
                        }
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            NetDebug.Log("OnApplication Quilt");
            StopServer();
        }

        public bool StartServer(int port, int connecitonMax, ConnectionType type) {
            NetDebug.Log("Start Server : " + type);
            try
            {
                if (type == ConnectionType.TCP)
                {
                    _sessionTCP.StartServer(port, connecitonMax);
                }
                else if (type == ConnectionType.UDP)
                {
                    _sessionUDP.StartServer(port, connecitonMax);
                }
            }
            catch {
                NetDebug.LogError("Start server failed");
                return false;
            }
            NetDebug.Log("Server Started");
            return true;
        }

        public void StopServer() {
            NetDebug.Log("Stop Server called");
            if (_sessionUDP != null) {
                _sessionUDP.StopServer();
            }

            if (_sessionTCP != null) {
                _sessionTCP.StopServer();
            }
            NetDebug.Log("Server Stopped");
        }

        public int Connect(string address, int port, ConnectionType type) {
            int node = -1;

            if (type == ConnectionType.TCP && _sessionTCP != null) {
                node = _sessionTCP.Connect(address, port);
            }

            if (type == ConnectionType.UDP && _sessionUDP != null) {
                node = _sessionUDP.Connect(address, port);
            }

            return node;
        }

        public void Disconnect(int node) {
            if (_sessionTCP != null) {
                _sessionTCP.Disconnect(node);
            }
            if (_sessionUDP != null) {
                _sessionUDP.Disconnect(node);
            }
        }

        public void Disconnect() {

            if (_sessionTCP != null)
            {
                _sessionTCP.Disconnect();
            }
            if (_sessionUDP != null)
            {
                _sessionUDP.Disconnect();
            }

            _notifier.Clear();
        }

        public void RegisterReceiveNotification(PacketId id, RecvNotifier notifier) {
            int index = (int)id;
            if (_notifier.ContainsKey(index)) {
                _notifier[index] = notifier;
                return;
            }
            _notifier.Add(index, notifier);
        }

        public void ClearReceiveNotification() {
            _notifier.Clear();
        }

        public void UnregisterReceiveNotification(PacketId id) {
            int index = (int)id;
            if (_notifier.ContainsKey(index)) {
                _notifier.Remove(index);
            }
        }

        public NetEventState GetEventState() {
            if (_eventQueue.Count == 0) return null;
            NetEventState state = _eventQueue[0];
            _eventQueue.RemoveAt(0);
            return state;
        }

        public bool IsConnected(int node) {
            if (_sessionTCP != null) {
                if (_sessionTCP.IsConnected(node)) return true;
            }

            if (_sessionUDP != null) {
                if (_sessionUDP.IsConnected(node)) return true;
            }
            return false;
        }

        public bool IsServer()
        {
            if (_sessionTCP == null) return false;

            return _sessionTCP.IsServer();
        }

        public IPEndPoint GetLocalEndPoint(int node) {
            if (_sessionTCP == null) return default(IPEndPoint);
            return _sessionTCP.GetLocalEndPoint(node);
        }

        public int Send<T>(int node, PacketId id, IPacket<T> packet) {
            int sendSize = 0;
            if (_sessionTCP != null) {

                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                header.packetId = id;
                byte[] headerData = null;
                if (serializer.Serialize(header)) {
                    headerData = serializer.GetSerializedData();
                }
                byte[] packetData = packet.GetData();
                byte[] data = new byte[headerData.Length + packetData.Length];
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
                Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);
                sendSize = _sessionTCP.Send(node, data, data.Length);
            }
            return sendSize;
        }

        public int SendReliable<T>(int node, IPacket<T> packet) {
            int sendSize = 0;

            if (_sessionTCP != null) {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                header.packetId = packet.GetPacketID();
                byte[] headerData = null;
                if (serializer.Serialize(header)) {
                    headerData = serializer.GetSerializedData();
                }

                byte[] packetData = packet.GetData();
                byte[] data = new byte[headerData.Length + packetData.Length];
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
                Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);
                sendSize = _sessionTCP.Send(node, data, data.Length);
                if (sendSize < 0 && _eventQueue != null) {
                    NetEventState state = new NetEventState(NetEventType.SendError, NetEventResult.Failure)
                    {
                        node = node
                    };
                    _eventQueue.Add(state);
                }
            }

            return sendSize;
        }

        public void SendReliableToAll<T>(IPacket<T> packet) {
            foreach (NodeInfo info in _reliableNode) {
                if (info != null) {
                    int sendSize = SendReliable<T>(info.node, packet);
                    if (sendSize < 0 && _eventQueue != null) {
                        NetEventState state = new NetEventState(NetEventType.SendError, NetEventResult.Failure) {
                            node = info.node
                        };

                        _eventQueue.Add(state);
                    }
                }
            }
        }

        public void SendReliableToAll(PacketId id, byte[] data) {
            foreach (NodeInfo info in _reliableNode) {
                if (info != null && info.node >= 0) {
                    PacketHeader header = new PacketHeader();
                    PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                    header.packetId = id;
                    byte[] headerData = null;
                    if (serializer.Serialize(header)) {
                        headerData = serializer.GetSerializedData();
                    }

                    byte[] packetData = data;
                    byte[] pData = new byte[headerData.Length + packetData.Length];
                    int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                    Buffer.BlockCopy(headerData, 0, pData, 0, headerSize);
                    Buffer.BlockCopy(packetData, 0, pData, headerSize, packetData.Length);
                    int sendSize = _sessionTCP.Send(info.node, pData, pData.Length);
                    if (sendSize < 0 && _eventQueue != null) {
                        NetEventState state = new NetEventState(NetEventType.SendError, NetEventResult.Failure) {
                            node = info.node
                        };
                        _eventQueue.Add(state);
                    }
                }
            }
        }

        public int SendUnreliable<T>(int node, IPacket<T> packet) {
            int sendSize = 0;
            if (_sessionUDP != null) {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                header.packetId = packet.GetPacketID();
                byte[] headerData = null;
                if (serializer.Serialize(header)) {
                    headerData = serializer.GetSerializedData();
                }
                byte[] packetData = packet.GetData();
                byte[] data = new byte[headerData.Length + packetData.Length];
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
                Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);
                sendSize = _sessionUDP.Send(node, data, data.Length);
                if (sendSize < 0 && _eventQueue != null) {
                    NetEventState state = new NetEventState(NetEventType.SendError, NetEventResult.Failure) {
                        node = node
                    };
                    _eventQueue.Add(state);
                }
            }
            return sendSize;
        }

        public void SendUnreliableToAll<T>(IPacket<T> packet) {
            
            int sendSize = -1;
            foreach (NodeInfo info in _unreliableNode) {
                if (info != null) {
                    sendSize = SendUnreliable<T>(info.node, packet);
                }
            }
            //NetDebug.Log("Send Unreliable : " + packet.GetPacketID() + " " + sendSize);
        }

        private void Receive(int node, byte[] data) {
            PacketHeader header = new PacketHeader();
            PacketHeaderSerializer serializer = new PacketHeaderSerializer();
            
            bool ret = serializer.Deserialize(data, ref header);
            if (!ret) {
                NetDebug.Log("Invalid header data");
                return;
            }

            int packetId = (int)header.packetId;
            
            if (_notifier.ContainsKey(packetId) && _notifier[packetId] != null) {
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                byte[] packetData = new byte[data.Length - headerSize];
                Buffer.BlockCopy(data, headerSize, packetData, 0, packetData.Length);
                _notifier[packetId](node, header.packetId, packetData);
            }
        }

        public void OnEventHandlingReliable(int node, NetEventState state) {
            NetDebug.Log(string.Format("OnEventHandling-tcp\r\n{0}:{1}:{2}", node, state.type, state.result));
            switch (state.type) {
                case NetEventType.Connect:
                    for (int i = 0; i < _reliableNode.Length; i++) {
                        if (_reliableNode[i] == null)
                        {
                            NodeInfo info = new NodeInfo(node);
                            _reliableNode[i] = info;
                            Notice.Instance.Send(NoticeName.OnPlayerConnected, info.node);
                            break;
                        }
                        else if (_reliableNode[i].node == -1) {
                            _reliableNode[i].node = node;
                            Notice.Instance.Send(NoticeName.OnPlayerConnected, _reliableNode[i].node);
                            break;
                        }
                    }
                    break;
                case NetEventType.Disconnect:
                    for (int i = 0; i < _reliableNode.Length; i++) {
                        if (_reliableNode[i] != null && _reliableNode[i].node == node) {
                            //Notice.Instance.Send(NoticeName.OnPlayerDisconnected, node);
                            //send
                            //GameServer.Instance.SendDisconnectReflection(node);
                            _reliableNode[i].node = -1;
                            break;
                        }
                    }
                    break;
            }

            if (_eventQueue != null) {
                NetEventState eState = new NetEventState(state.type, NetEventResult.Success) { node = node };
                _eventQueue.Add(eState);
            }
        }

        public void OnEventHandlingUnreliable(int node, NetEventState state) {
            NetDebug.Log(string.Format("OnEventHandling-udp\r\n{0}:{1}:{2}", node, state.type, state.result));
            switch (state.type)
            {
                case NetEventType.Connect:
                    for (int i = 0; i < _unreliableNode.Length; i++)
                    {
                        if (_unreliableNode[i] == null)
                        {
                            NodeInfo info = new NodeInfo(node);
                            _unreliableNode[i] = info;
                            break;
                        }
                        else if (_unreliableNode[i].node == -1)
                        {
                            _unreliableNode[i].node = node;
                            break;
                        }
                    }
                    break;
                case NetEventType.Disconnect:
                    for (int i = 0; i < _unreliableNode.Length; i++)
                    {
                        if (_unreliableNode[i] != null && _unreliableNode[i].node == node)
                        {
                            _unreliableNode[i].node = -1;
                            break;
                        }
                    }
                    break;
            }

            if (_eventQueue != null)
            {
                NetEventState eState = new NetEventState(state.type, NetEventResult.Success) { node = node };
                _eventQueue.Add(eState);
            }
        }

        public bool StartGameServer(int playerNum)
        {
            return GameServer.Instance.StartServer(playerNum);
        }

        public void StopGameServer() {
            GameServer.Instance.StopServer();
        }

        public void SetServerNode(int node) {
            _serverNode = node;
        }

        public int GetServerNode() {
            return _serverNode;
        }

        public void SetClientNode(int gid, int node) {
            NetDebug.LogError("Set Client Node " + gid + " " + node);
            _clientNode[gid] = node;
        }

        public int GetClientNode(int gid)
        {
            return _clientNode[gid];
        }

        public int GetPlayerIdFromNode(int node) {
            for (int i = 0; i > _clientNode.Length; i++) {
                if (_clientNode[i] == node) return i;
            }
            return -1;
        }

        public void PrintNodeInfo() {

            Debug.Log("Reliable Node Info");
            foreach (var re in _reliableNode) {
                if (re == null) continue;
                Debug.Log(re.node.ToString() + " " + _sessionTCP.GetRemoteEndPoint(re.node).ToString());
            }

            Debug.Log("Unreliable Node Info");
            foreach (var ue in _unreliableNode) {
                if (ue == null) continue;
                Debug.Log(ue.node.ToString() + " " + _sessionUDP.GetRemoteEndPoint(ue.node).ToString());
            }
        }

        public List<IPEndPoint> GetReliableEndPoints() {
            List<IPEndPoint> output = new List<IPEndPoint>();
            foreach (var re in _reliableNode) {
                if (re == null) continue;
                var ep = _sessionTCP.GetRemoteEndPoint(re.node);
                if (ep == null) continue;
                if (ep != default(IPEndPoint)) {
                    output.Add(ep);
                }
            }
            return output;
        }

        public IPEndPoint GetReliableEndPoint(int node) {
            var ep = _sessionTCP.GetRemoteEndPoint(node);
            if (ep == null && ep == default(IPEndPoint)) return null;
            return ep;
        }
    }
}