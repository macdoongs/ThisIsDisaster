using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents
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
            catch
            {
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
                    _handler(state);
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
            Debug.Log("AcceptClient");
#else
        Console.WriteLine("AcceptClient");
#endif
            if (_listener != null && _listener.Poll(0, SelectMode.SelectRead))
            {
                Socket socket = _listener.Accept();
                _socket.Add(socket);
                _isConnected = true;
                if (_handler != null)
                {
                    NetEventState state = new NetEventState()
                    {
                        type = NetEventType.Connect,
                        result = NetEventResult.Success
                    };

                    _handler(state);
                }


                Debug.Log("Connected from client, port : " + _port);
            }
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
                    foreach (Socket socket in _socket)
                    {
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
                    _handler(state);
                }
            }
        }

        void DispatchReceive()
        {
            if (_socket != null) { return; }

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
                    _handler(state);
                }
            }
        }
    }
}