using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents.Legacy
{
    public class TransportTCP
    {
        private Socket _listener = null;
        private List<Socket> _socket = null;
        private int _port = -1;
        private bool _isServer = false;
        private bool _isConnected = false;

        private PacketQueue _sendQueue = new PacketQueue();
        private PacketQueue _recvQueue = new PacketQueue();

        private EventHandler _handler;
        
        private System.Object _lockObj = new object();

        public TransportTCP()
        {
            _socket = new List<Socket>();
        }

        public bool StartServer(int port)
        {
            Debug.Log("TCP : StartServer, port : " + port);

            try
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    NoDelay = true
                };
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _listener.Listen(1);
                _port = port;
            }
            catch
            {
                return false;
            }
            _isServer = true;
            return true;
        }

        public void StopServer()
        {
            Debug.Log("TCP : StopServer");

            Disconnect();

            if (_listener != null)
            {
                _listener.Close();
                _listener = null;
            }

            _isServer = false;
        }

        public bool Connect(string address, int port)
        {
            try
            {
                lock (_lockObj)
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.NoDelay = true;
                    socket.Connect(address, port);
                    _socket.Add(socket);
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Connect Failed : {0} {1}\r\n{2}", address, port, e.ToString()));
#endif
                return false;
            }

            _isConnected = true;
            Debug.Log("TransportTcp::Connect called.");

            return true;
        }

        public bool Disconnect()
        {
            if (_socket != null)
            {
                lock (_lockObj)
                {
                    foreach (var socket in _socket)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }

                    _socket.Clear();
                    _socket = null;
                }

                if (_handler != null)
                {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.Disconnect,
                        result = NetEventResult.Success
                    };
                   // _handler(state);
                }
            }

            _isConnected = false;

            Debug.Log("TCP : Disconnect");
            return true;
        }

        public int Send(byte[] data, int size)
        {
            return _sendQueue.Enqueue(data, size);
        }

        public int Receive(ref byte[] buffer, int size)
        {
            return _recvQueue.Dequeue(ref buffer, size);
        }

        public bool IsConnected()
        {
            return _isConnected;
        }

        public bool IsServer()
        {
            return _isServer;
        }

        public void RegisterEventHandler(EventHandler handler)
        {
            _handler += handler;
        }

        public void UnregisterEventHandler(EventHandler handler)
        {
            _handler -= handler;
        }

        public void Dispatch()
        {
            AcceptClient();
            if (_isConnected && _socket != null) {
                lock (_lockObj) {
                    DispatchSend();
                    DispatchReceive();
                }
            }
        }

        public void AcceptClient()
        {
#if UNITY_EDITOR
            //Debug.Log("AcceptClient");
#else
            //Console.WriteLine("AcceptClient");
#endif
            if (_listener != null && _listener.Poll(0, SelectMode.SelectRead))
            {
                Socket socket = _listener.Accept();
                _socket.Add(socket);
                _isConnected = true;

                StringBuilder logger = new StringBuilder();
                IPEndPoint local = socket.LocalEndPoint as IPEndPoint;
                IPEndPoint remote = socket.RemoteEndPoint as IPEndPoint;
                logger.AppendFormat("Connected Client\r\n[Local Info]\tIP: {0}\t\tPort: {1}\r\n[Remote Info]\tIP: {2}\t\tPort: {3}",
                    local.Address, local.Port, remote.Address, remote.Port);
                Debug.LogError(logger.ToString());

                if (_handler != null)
                {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.Connect,
                        result = NetEventResult.Success
                    };

                    //_handler(state);
                }

            }
        }

        void LogSendState(Socket sendPoint) {
            StringBuilder sb = new StringBuilder();
            IPEndPoint from = sendPoint.LocalEndPoint as IPEndPoint;
            IPEndPoint to = sendPoint.RemoteEndPoint as IPEndPoint;
            sb.AppendFormat("[Local]\t\t[IP: {0}\t\tPort: {1}]\r\n[Remote]\t\t[IP: {2}\t\tPort: {3}]", from.Address, from.Port, to.Address, to.Port);
            Debug.LogError(sb.ToString());
        }

        void DispatchSend()
        {
            if (_socket == null) { return; }
            try
            {
                byte[] buffer = new byte[NetConfig.PACKET_SIZE];
                int sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                while (sendSize > 0)
                {
                    Debug.Log("Send TCP : " + sendSize);
                    foreach (Socket socket in _socket)
                    {
                        LogSendState(socket);
                        socket.Send(buffer, sendSize, SocketFlags.None);
                    }
                    sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                }
            }
            catch
            {
                if (_handler != null)
                {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.SendError,
                        result = NetEventResult.Failure
                    };
                   // _handler(state);
                }
            }
        }

        void DispatchReceive()
        {
            if (_socket == null) { return; }

            try
            {
                foreach (var socket in _socket)
                {
                    if (socket.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buffer = new byte[NetConfig.PACKET_SIZE];
                        int recvSize = socket.Receive(buffer, buffer.Length, SocketFlags.None);

                        Debug.Log("TCP Recevie Data, size : " + recvSize + " //port : " + _port);
                        if (recvSize == 0)
                        {
                            Disconnect();
                        }
                        else if (recvSize > 0)
                        {
                            _recvQueue.Enqueue(buffer, recvSize);
                        }
                    }
                }
            }
            catch
            {
                if (_handler != null)
                {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.RecvError,
                        result = NetEventResult.Failure
                    };
                    //_handler(state);
                }
            }
        }
    }
}

namespace NetworkComponents {
    public class TransportTCP : ITransport
    {
        private int _nodeId = -1;
        private Socket _socket = null;
        private bool _isConnected = false;
        private PacketQueue _sendQueue = new PacketQueue();
        private PacketQueue _recvQueue = new PacketQueue();
        private EventHandler _handler;
        private const int _packetSize = NetConfig.PACKET_SIZE;
        public string transportName = "";

        public TransportTCP() { }

        public TransportTCP(Socket socket, string name)
        {
            _socket = socket;
            transportName = name;
        }

        public bool Initialize(Socket socket)
        {
            _socket = socket;
            _isConnected = true;
            return true;
        }

        public bool IsConnected()
        {
            return _isConnected;
        }

        public int GetNodeId()
        {
            return _nodeId;
        }

        public void SetNodeId(int node)
        {
            _nodeId = node;
        }

        public bool Terminate()
        {
            _socket = null;
            return true;
        }

        public bool Connect(string ipAddress, int port)
        {
            NetDebug.Log("Transport connect called");
            if (_socket != null) return false;
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(ipAddress, port);
                _socket.NoDelay = true;

                _isConnected = true;
                NetDebug.Log("connection success");
            }
            catch (SocketException e)
            {
                _socket = null;
                _isConnected = false;
                NetDebug.LogError(e.ToString());
            }

            NetDebug.Log("TransportTCP Connected: " + _isConnected.ToString());
            if (_handler != null)
            {
                NetEventState state = new NetEventState(NetEventType.Connect,
                    _isConnected ? NetEventResult.Success : NetEventResult.Failure);
                _handler(this, state);

            }
            return _isConnected;
        }

        public void Disconnect()
        {
            _isConnected = false;
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
            if (_isConnected && _socket != null)
            {
                DispatchSend();
                DispatchReceive();
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
                    int sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    while (sendSize > 0)
                    {
                        try
                        {
                            int sendResult = _socket.Send(buffer, sendSize, SocketFlags.None);
                            if (sendResult < 0)
                            {
                                NetDebug.Log("Transport send error : " + sendResult);
                            }
                        }
                        catch (SocketException e)
                        {
                            NetDebug.LogError("transport send error " + e.Message);
                            if (_handler != null)
                            {
                                NetEventState state = new NetEventState(NetEventType.SendError, NetEventResult.Failure)
                                {
                                    node = _nodeId,
                                };
                                _handler(this, state);
                            }
                        }
                        sendSize = _sendQueue.Dequeue(ref buffer, buffer.Length);
                    }
                }
            }
            catch { return; }
        }

        void DispatchReceive()
        {
            if (_socket == null) return;
            try
            {
                while (_socket.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[_packetSize];
                    int recvSize = _socket.Receive(buffer, buffer.Length, SocketFlags.None);
                    if (recvSize == 0)
                    {
                        NetDebug.Log("TCP Disconnect recv from other");
                        Disconnect();
                    }
                    else if (recvSize > 0)
                    {
                        _recvQueue.Enqueue(buffer, recvSize);
                    }
                }
            }
            catch { return; }
        }

        public IPEndPoint GetLocalEndPoint()
        {
            if (_socket == null) return default(IPEndPoint);
            return _socket.LocalEndPoint as IPEndPoint;
        }


        public IPEndPoint GetRemoteEndPoint()
        {
            if (_socket == null) return default(IPEndPoint);
            return _socket.RemoteEndPoint as IPEndPoint;
        }

        public int Receive(ref byte[] buffer, int size)
        {
            return _recvQueue.Dequeue(ref buffer, size);
        }

        public int Send(byte[] data, int size)
        {
            return _sendQueue.Enqueue(data, size);
        }

        public void SetServerPort(int port)
        {
            
        }

        public void RegisterEventHandler(EventHandler handler)
        {
            _handler += handler;
        }


        public void UnregisterEventHandler(EventHandler handler)
        {
            _handler -= handler;
        }
    }

}