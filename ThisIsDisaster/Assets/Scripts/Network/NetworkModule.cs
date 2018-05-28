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
                        while (_sessionTCP.Receive(node, ref packet) > 0) {
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
            NetDebug.Log("Start Server");
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
            NetDebug.Log("Stop Serve called");
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
            foreach (NodeInfo info in _unreliableNode) {
                if (info != null) {
                    SendUnreliable<T>(info.node, packet);
                }
            }
        }

        private void Receive(int node, byte[] data) {
            PacketHeader header = new PacketHeader();
            PacketHeaderSerializer serializer = new PacketHeaderSerializer();
            //serializer.SetDesrializedData(data);
            
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
                            break;
                        }
                        else if (_reliableNode[i].node == -1) {
                            _reliableNode[i].node = node;
                        }
                    }
                    break;
                case NetEventType.Disconnect:
                    for (int i = 0; i < _reliableNode.Length; i++) {
                        if (_reliableNode[i] != null && _reliableNode[i].node == node) {
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
            //find gameserver
            //GameServer.Instance.StartServer();//
            GameServer.Instance.StartServer(playerNum);
            return true;
        }

        public void StopGameServer() {
            //GameServer.Instance.StopServer();
            GameServer.Instance.StopServer();
        }

        public void SetServerNode(int node) {
            _serverNode = node;
        }

        public int GetServerNode() {
            return _serverNode;
        }

        public void SetClientNode(int gid, int node) {
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




#if false
        public enum ConnectionType
        {
            TCP = 0,
            UDP,
            BOTH
        }

        public static NetworkModule Instance {
            private set;
            get;
        }

        private TransportTCP _tcp = null;
        private TransportUDP _udp = null;
        private Thread _thread = null;
        bool _isStarted = false;
        bool _isServer = false;
        const int _headerVersion = 1;

        private const int _packetMax = (int)PacketId.Max;

        public delegate void RecvNotifier(PacketId id, int senderId, byte[] data);
        private Dictionary<int, RecvNotifier> _notifier = new Dictionary<int, RecvNotifier>();
        private NetworkComponents.EventHandler _handler;

        bool _eventOccured = false;

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject); return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (IsConnected()) {
                byte[] packet = new byte[NetConfig.PACKET_SIZE];
                while (_tcp != null && _tcp.Receive(ref packet, packet.Length) > 0) {
                    ReceivePacket(packet);
                }

                while (_udp != null && _udp.Receive(ref packet, packet.Length) > 0) {
                    ReceivePacket(packet);
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (_isStarted)
            {
                //stop
                StopServer();
            }
        }

        public bool StartServer(int port, ConnectionType type)
        {

            Debug.Log("Network::StartServer called. port:" + port);

            try
            {
                if (type == ConnectionType.TCP ||
                    type == ConnectionType.BOTH)
                {
                    _tcp = new TransportTCP();
                    //_tcp.StartServer(port);
                    //_tcp.RegisterEventHandler(OnEventHandling);
                }
                if (type == ConnectionType.UDP ||
                    type == ConnectionType.BOTH)
                {
                    _udp = new TransportUDP();
                    //_udp.StartServer(port);
                    //_udp.RegisterEventHandler(OnEventHandling);
                }
            }
            catch
            {
                Debug.Log("Network::StartServer fail");
                return false;
            }

            Debug.Log("Network::Server started");

            _isServer = true;

            return LaunchThread();
        }

        public void StopServer() {
            _isStarted = false;
            if (_thread != null) {
                _thread.Join();
                _thread = null;
            }

            if (_tcp != null) {
                //_tcp.StopServer();
            }

            if (_udp != null) {
                //_udp.StopServer();
            }

            _notifier.Clear();
            _isServer = false;
            _eventOccured = false;
            //log
        }

        public bool Connect(string address, int port, ConnectionType type) {
            try
            {
                bool ret = true;
                if (type == ConnectionType.TCP || type == ConnectionType.BOTH) {
                    if (_tcp == null) {
                        _tcp = new TransportTCP();
                        //_tcp.RegisterEventHandler(OnEventHandling);
                    }
                    ret &= _tcp.Connect(address, port);
                }

                if (type == ConnectionType.UDP || type == ConnectionType.BOTH) {
                    if (_udp == null) {
                        _udp = new TransportUDP();
                        //_udp.RegisterEventHandler(OnEventHandling);
                    }

                    ret &= _udp.Connect(address, port);
                }

                if (!ret) {
                    if (_tcp != null) _tcp.Disconnect();
                    if (_udp != null) _udp.Disconnect();
                    return false;
                }
            }
            catch { return false; }

            return LaunchThread();
        }

        public bool Disconnect()
        {
            if (_tcp != null)
            {
                _tcp.Disconnect();
            }

            if (_udp != null)
            {
                _udp.Disconnect();
            }

            _isStarted = false;
            _eventOccured = false;
            return true;
        }


        public bool StartGameServer()
        {
            //log started
            return GameServer.Instance.StartServer();
        }

        public void StopGameServer()
        {
            //fing object
            GameObject NetworkObject = GameObject.Find("GameServer");
            if (NetworkObject)
            {
                GameObject.Destroy(NetworkObject);
            }
            //log
        }

        public void RegisterReceiveNotification(PacketId id, RecvNotifier notifier)
        {
            int index = (int)id;

            if (_notifier.ContainsKey(index))
            {
                _notifier.Remove(index);
            }

            _notifier.Add(index, notifier);
        }

        public void UnregisterReceiveNotification(PacketId id)
        {
            int index = (int)id;

            if (_notifier.ContainsKey(index))
            {
                _notifier.Remove(index);
            }
        }

        public void RegisterEventHandler(EventHandler handler)
        {
            _handler += handler;
        }

        public void UnregisterEventHandler(EventHandler handler)
        {
            _handler -= handler;
        }

        public bool IsConnected() {
            bool _isTcp = false;
            bool _isUdp = false;
            
            if (_tcp != null && _tcp.IsConnected())
            {
                _isTcp = true;
            }

            if (_udp != null && _udp.IsConnected())
            {
                _isUdp = true;
            }

            if (_tcp != null && _udp == null)
            {
                return _isTcp;
            }

            if (_tcp == null && _udp != null)
            {
                return _isUdp;
            }

            return (_isTcp && _isUdp);
        }

        public bool IsCommunicating() {
            bool _isTcp = false;
            bool _isUdp = false;

            if (_tcp != null && _tcp.IsConnected()) {
                _isTcp = true;
            }

            //if (_udp != null && _udp.IsCommunicating()) {
            //    _isUdp = true;
            //}

            if (_tcp != null && _udp == null) {
                return _isTcp;
            }

            if (_tcp == null && _udp != null)
            {
                return _isUdp;
            }

            return (_isTcp && _isUdp);
        }

        public bool IsServer() {
            return _isServer;
        }

        bool LaunchThread() {
            //log
            try
            {
                _thread = new Thread(new ThreadStart(Dispatch));
                _thread.Start();
            }
            catch {
                //fail log
                return false;
            }

            _isStarted = true;
            //log
            return true;
        }

        void Dispatch() {
            while (_isStarted) {
                if (_tcp != null) {
                    _tcp.Dispatch();
                }

                if (_udp != null) {
                    _udp.Dispatch();
                }
                Thread.Sleep(5);
            }
        }

        public int Send(PacketId id, byte[] data) {
            int sendSize = 0;

            if (_tcp != null)
            {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();

                header.packetId = id;
                header.packetSender = GlobalParameters.Param.accountId;

                byte[] headerData = null;
                if (serializer.Serialize(header) == true)
                {
                    headerData = serializer.GetSerializedData();
                }

                byte[] packetData = new byte[headerData.Length + data.Length];

                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, packetData, 0, headerSize);
                Buffer.BlockCopy(data, 0, packetData, headerSize, data.Length);

                sendSize = _tcp.Send(data, data.Length);
            }

            return sendSize;
        }

        public int SendReliable<T>(IPacket<T> packet) {
            int sendSize = 0;
            if (_tcp != null) {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                header.packetId = packet.GetPacketID();
                header.packetSender = GlobalParameters.Param.accountId;

                byte[] headerData = null;
                if (serializer.Serialize(header)) {
                    headerData = serializer.GetSerializedData();
                }

                byte[] packetData = packet.GetData();
                byte[] data = new byte[headerData.Length + packetData.Length];

                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
                Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);

                //log
                sendSize = _tcp.Send(data, data.Length);
            }

            return sendSize;
        }

        public void SendReliableToAll(PacketId id, byte[] data) {
            if (_tcp != null)
            {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();

                header.packetId = id;
                header.packetSender = GlobalParameters.Param.accountId;

                byte[] headerData = null;
                if (serializer.Serialize(header))
                {
                    headerData = serializer.GetSerializedData();
                }

                byte[] pData = new byte[headerData.Length + data.Length];
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, pData, 0, headerSize);
                Buffer.BlockCopy(data, 0, pData, headerSize, data.Length);
                //log;
                int sendSize = _tcp.Send(pData, pData.Length);
                if (sendSize < 0)
                {
                    //Communication error
                }
            }
        }

        public int SendUnreliable<T>(IPacket<T> packet)
        {
            int sendSize = 0;

            if (_udp != null)
            {
                PacketHeader header = new PacketHeader();
                PacketHeaderSerializer serializer = new PacketHeaderSerializer();
                header.packetId = packet.GetPacketID();
                header.packetSender = GlobalParameters.Param.accountId;

                byte[] headerData = null;
                if (serializer.Serialize(header))
                {
                    headerData = serializer.GetSerializedData();
                }
                byte[] packetData = packet.GetData();
                byte[] data = new byte[headerData.Length + packetData.Length];

                int headerSize = Marshal.SizeOf(typeof(PacketHeader));
                Buffer.BlockCopy(headerData, 0, data, 0, headerSize);
                Buffer.BlockCopy(packetData, 0, data, headerSize, packetData.Length);
                sendSize = _udp.Send(data, data.Length);
            }

            return sendSize;
        }

        public bool ReceivePacket(byte[] data)
        {   
            PacketHeader header = new PacketHeader();
            PacketHeaderSerializer serializer = new PacketHeaderSerializer();

            bool ret = serializer.Deserialize(data, ref header);
            if (!ret) return false;

            //if (_udp != null && _udp.IsCommunicating())
            //{
            //    if (_handler != null && !_eventOccured)
            //    {
            //        NetEventState state = new NetEventState(NetEventType.Connect, NetEventResult.Success)
            //        {
            //            endPoint = _udp.GetRemoteEndPoint()
            //        };

            //        //log
            //        _handler(state);
            //        _eventOccured = true;
            //    }
            //}

            int packetId = (int)header.packetId;
            
            int packetSender = header.packetSender;
            Debug.LogError(string.Format("Recieved From {1} Packet {0}", header.packetId, header.packetSender));
            if (_notifier.ContainsKey(packetId) && _notifier[packetId] != null)
            {
                int headerSize = Marshal.SizeOf(typeof(PacketHeader));//sizeof(PacketId) + sizeof(int);
                byte[] packetData = new byte[data.Length - headerSize];
                Buffer.BlockCopy(data, headerSize, packetData, 0, packetData.Length);
                _notifier[packetId]((PacketId)packetId, packetSender, packetData);
            }

            return ret;
        }

        public void OnEventHandling(NetEventState state)
        {
            if (_handler != null)
            {
                //_handler(state);
            }
        }
        
#endif
    }
}