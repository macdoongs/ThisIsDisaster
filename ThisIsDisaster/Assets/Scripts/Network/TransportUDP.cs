using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents
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
                    _handler(state);
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

                    _handler(state);
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
