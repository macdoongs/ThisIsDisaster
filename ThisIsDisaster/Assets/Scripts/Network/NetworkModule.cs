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
                    _tcp.StartServer(port);
                    _tcp.RegisterEventHandler(OnEventHandling);
                }
                if (type == ConnectionType.UDP ||
                    type == ConnectionType.BOTH)
                {
                    _udp = new TransportUDP();
                    _udp.StartServer(port);
                    _udp.RegisterEventHandler(OnEventHandling);
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
                _tcp.StopServer();
            }

            if (_udp != null) {
                _udp.StopServer();
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
                        _tcp.RegisterEventHandler(OnEventHandling);
                    }
                    ret &= _tcp.Connect(address, port);
                }

                if (type == ConnectionType.UDP || type == ConnectionType.BOTH) {
                    if (_udp == null) {
                        _udp = new TransportUDP();
                        _udp.RegisterEventHandler(OnEventHandling);
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

            if (_udp != null && _udp.IsCommunicating()) {
                _isUdp = true;
            }

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

            if (_udp != null && _udp.IsCommunicating())
            {
                if (_handler != null && !_eventOccured)
                {
                    NetEventState state = new NetEventState(NetEventType.Connect, NetEventResult.Success)
                    {
                        endPoint = _udp.GetRemoteEndPoint()
                    };

                    //log
                    _handler(state);
                    _eventOccured = true;
                }
            }

            int packetId = (int)header.packetId;
            
            int packetSender = header.packetSender;
            Debug.LogError(string.Format("Recieved From {0} Packet {1}", header.packetId, header.packetSender));
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
                _handler(state);
            }
        }

    }
}