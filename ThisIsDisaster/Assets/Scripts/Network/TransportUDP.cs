using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents.Legacy
{
    public class TransportUDP
    {
        private Socket _listener = null;
        private Socket _socket = null;
        private int _port = -1;
        private IPEndPoint _endPoint = null;
        private IPEndPoint _remoteEndPoint = null;
        private PacketQueue _sendQueue = new PacketQueue();
        private PacketQueue _recvQueue = new PacketQueue();
        private bool _isConnected = false;
        private bool _isCommunicating = false;

        private const int _timeOutSec = 60 * 3;
        private DateTime _timeOutTicker;
        private const int _keepAliveInter = 10;
        private DateTime _keepAliveTicker;

        private EventHandler _handler;
        private const string _requestDataLog = "KeepAlive.";

        public bool StartServer(int port)
        {
            //log
            try
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _port = port;
                _isCommunicating = false;
            }
            catch {
                return false;
            }
            return true;
        }

        public void StopServer() {
            Disconnect();
            if (_listener != null) {
                _listener.Close();
                _listener = null;
            }
            _isCommunicating = false;
            //log
        }

        public bool Connect(string ipAddr, int port) {
            try
            {
                IPAddress addr = IPAddress.Parse(ipAddr);
                _endPoint = new IPEndPoint(addr, port);
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            }
            catch {
                return false;
            }
            _isConnected = true;
            //log
            return true;
        }

        public bool Disconnect() {
            if (_socket != null) {
                _socket.Close();
                _socket = null;
                if (_handler != null) {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.Disconnect,
                        result = NetEventResult.Success
                    };
                    //_handler(state);
                }
            }

            _isConnected = false;
            //log
            return true;
        }

        public int Send(byte[] data, int size) {
            return _sendQueue.Enqueue(data, size);
        }

        public int Receive(ref byte[] buffer, int size) {
            return _recvQueue.Dequeue(ref buffer, size);
        }

        public bool IsConnected() {
            return _isConnected;
        }

        public bool IsCommunicating() {
            return _isCommunicating;
        }

        public IPEndPoint GetRemoteEndPoint() {
            return _remoteEndPoint;
        }

        public void RegisterEventHandler(EventHandler handler) {
            _handler += handler;
        }

        public void UnregisterEventHandler(EventHandler handler) {
            _handler -= handler;
        }

        public void Dispatch() {
            AcceptClient();
            if (_socket != null || _listener != null) {
                DispatchSend();
                DispatchReceive();
                CheckTimeout();
            }

            if (_socket != null) {
                TimeSpan ts = DateTime.Now - _keepAliveTicker;
                if (ts.Seconds > _keepAliveInter) {
                    byte[] request = System.Text.Encoding.UTF8.GetBytes(_requestDataLog);
                    _socket.SendTo(request, request.Length, SocketFlags.None, _endPoint);
                    _keepAliveTicker = DateTime.Now;
                }
            }
        }

        void AcceptClient() {
            if (!_isCommunicating && _listener != null && _listener.Poll(0, SelectMode.SelectRead)) {
                _isCommunicating = true;
                _timeOutTicker = DateTime.Now;
                if (_handler != null) {
                    NetEventState state = new NetEventState() {
                        type = NetEventType.Connect,
                        result = NetEventResult.Success
                    };

                   // _handler(state);
                }
            }
        }

        void DispatchSend() {
            if (_socket == null) return;

            try {
                if (_socket.Poll(0, SelectMode.SelectWrite)) {
                    byte[] buffer = new byte[NetConfig.PACKET_SIZE];
                    int sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    while (sendSize > 0) {
                        _socket.SendTo(buffer, sendSize, SocketFlags.None, _endPoint);
                        sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    }
                }
            }
            catch { return; }
        }

        void DispatchReceive() {
            if (_listener == null) return;

            try {
                while (_listener.Poll(0, SelectMode.SelectRead)) {
                    byte[] buffer = new byte[NetConfig.PACKET_SIZE];
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint remoteEp = (EndPoint)sender;
                    int recvSize = _listener.ReceiveFrom(buffer, SocketFlags.None, ref remoteEp);
                    _remoteEndPoint = (IPEndPoint)remoteEp;

                    if (_endPoint == null) {
                        _endPoint = _remoteEndPoint;
                    }

                    string str = System.Text.Encoding.UTF8.GetString(buffer);
                    if (_requestDataLog.CompareTo(str.Trim('\0')) == 0)
                    {
                        //keep alive
                    }
                    else if (recvSize == 0)
                    {
                        Disconnect();
                    }
                    else if (recvSize > 0) {
                        _recvQueue.Enqueue(buffer, recvSize);
                        _timeOutTicker = DateTime.Now;
                    }
                }
            }
            catch { return; }
        }

        void CheckTimeout() {
            TimeSpan ts = DateTime.Now - _timeOutTicker;
            if (_isConnected && _isCommunicating && ts.Seconds > _timeOutSec) {
                Debug.Log("Timeout Discconect udp");
                Disconnect();
            }
        }
    }
}

namespace NetworkComponents {
    public class TransportUDP : ITransport
    {
        private int _nodeId = -1;
        private Socket _socket = null;
        private bool _isConnected = false;
        private IPEndPoint _localEndPoint = null;
        private IPEndPoint _remoteEndPoint = null;
        private PacketQueue _sendQueue = new PacketQueue();
        private PacketQueue _recvQueue = new PacketQueue();
        private EventHandler _handler;
        private const int _packetSize = NetConfig.PACKET_SIZE;
        private bool _isRequested = false;
        private const int _timeOutSec = 10;
        private DateTime _timeOutTicker;
        private const int _keepAliveInter = 1;
        private DateTime _keepAliveTicker;
        private bool _isFirst = false;
        public const string _requestData = "KeepAlive.";
        private int _serverPort = -1;

        public TransportUDP() { }

        public TransportUDP(Socket socket) { _socket = socket; }

        public bool Initialize(Socket socket)
        {
            _socket = socket;
            _isRequested = true;
            return true;
        }

        public bool Terminate()
        {
            _socket = null;
            return true;
        }

        public int GetNodeId()
        {
            return _nodeId;
        }

        public void SetNodeId(int node)
        {
            _nodeId = node;
        }

        public IPEndPoint GetLocalEndPoint()
        {
            return _localEndPoint;
        }

        public IPEndPoint GetRemoteEndPoint()
        {
            return _remoteEndPoint;
        }

        public int Send(byte[] data, int size)
        {
            if (_sendQueue == null) return 0;
            return _sendQueue.Enqueue(data, size);
        }

        public int Receive(ref byte[] buffer, int size)
        {
            if (_recvQueue == null) return 0;
            return _recvQueue.Dequeue(ref buffer, size);
        }

        public bool Connect(string ipAddress, int port)
        {
            if (_socket == null)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                NetDebug.Log("Create new UDP Socket");
                
            }
            try
            {
                //string hostName = Dns.GetHostName();
                //IPAddress[] adrList = Dns.GetHostAddresses(hostName);
                //foreach (var addr in adrList)
                //{
                //    if (addr.AddressFamily == AddressFamily.InterNetwork)
                //    {
                //        _localEndPoint = new IPEndPoint(addr, _serverPort);
                //        break;
                //    }
                //}

                _localEndPoint = new IPEndPoint(IPAddress.Parse(Network.player.ipAddress),
                                port);
                
                _isRequested = true;
                NetDebug.Log("UDP SetUp success");
            }
            catch
            {
                _isRequested = false;
                NetDebug.LogError("UDP Connect Failed");
            }

            NetDebug.Log("Transport UDP Connection : " + _isRequested);
            if (_handler != null)
            {
                NetEventState state = new NetEventState(NetEventType.Connect, _isRequested ? NetEventResult.Success : NetEventResult.Failure);
                _handler(this, state);
                NetDebug.Log("Event Handler Called");
            }
            _keepAliveTicker = DateTime.Now;
            _isFirst = true;
            return _isRequested;
        }

        public void Disconnect()
        {
            _isRequested = false;
            if (_socket != null)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
            if (_handler != null)
            {
                NetEventState state = new NetEventState(NetEventType.Disconnect, NetEventResult.Success);
                _handler(this, state);
            }
        }

        public void Dispatch()
        {
            DispatchSend();
            CheckTimeout();
            if (_socket != null)
            {
                TimeSpan ts = DateTime.Now - _keepAliveTicker;
                if (ts.Seconds > _keepAliveInter || _isFirst)
                {
                    string message = _localEndPoint.Address.ToString() + ":" + _serverPort + ":" + _requestData;
                    byte[] request = System.Text.Encoding.UTF8.GetBytes(message);
                    _socket.SendTo(request, request.Length, SocketFlags.None, _remoteEndPoint);
                    _keepAliveTicker = DateTime.Now;
                    _isFirst = false;
                    NetDebug.LogError("Send UDP Message : " + message);
                }
            }
        }

        void DispatchSend()
        {
            if (_socket == null) return;
            try
            {
                if (_socket.Poll(0, SelectMode.SelectWrite))
                {
                    byte[] buffer = new byte[_packetSize];
                    int sendSie = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    while (sendSie > 0)
                    {
                        _socket.SendTo(buffer, sendSie, SocketFlags.None, _remoteEndPoint);
                        sendSie = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    }
                }
            }
            catch { return; }
        }

        public void SetReceiveData(byte[] data, int size, IPEndPoint endPoint)
        {
            string str = System.Text.Encoding.UTF8.GetString(data).Trim('\0');
            if (str.Contains(_requestData))
            {
                if (_isConnected == false && _handler != null)
                {
                    NetEventState state = new NetEventState(NetEventType.Connect, NetEventResult.Success);
                    _handler(this, state);
                    IPEndPoint ep = _localEndPoint;
                    NetDebug.Log("UDP connected from client. " + ep.Address.ToString() + " : " + ep.Port);
                }
                _isConnected = true;
                _timeOutTicker = DateTime.Now;
            }
            else if (size > 0)
            {
                _recvQueue.Enqueue(data, size);
            }
        }

        void CheckTimeout()
        {
            TimeSpan ts = DateTime.Now - _timeOutTicker;
            if (_isRequested && _isConnected && ts.Seconds > _timeOutSec)
            {
                NetDebug.Log("Dissconnect by timeout");
                Disconnect();
            }
        }

        public bool IsConnected()
        {
            return _isConnected;
        }

        public bool IsRequested()
        {
            return _isRequested;
        }

        public void RegisterEventHandler(EventHandler handler)
        {
            _handler += handler;
        }

        public void UnregisterEventHandler(EventHandler handler)
        {
            _handler -= handler;
        }

        public void SetServerPort(int port)
        {
            NetDebug.LogError("UDP Set ServerPort : " + port);
            _serverPort = port;
        }
    }
}