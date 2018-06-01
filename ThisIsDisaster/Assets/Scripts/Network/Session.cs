using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetworkComponents
{
    public abstract class Session<T> where T : ITransport, new() {
        protected Socket _listener = null;
        protected int _port = 0;
        protected int _nodeIndex = 0;
        protected Dictionary<int, T> _transports = null;
        protected bool _threadLoop = false;
        protected Thread _thread = null;
        protected System.Object _transperLock = new System.Object();
        protected System.Object _nodeIndexLock = new System.Object();
        protected bool _isServer = false;
        protected int _mtu;
        protected int _defaultMTUSize = 1500;//Ehternet v2

        public delegate void EventHandler(int node, NetEventState state);
        protected EventHandler _handler;

        public Session() {
            try
            {
                _transports = new Dictionary<int, T>();
                _mtu = _defaultMTUSize;
            }
            catch (Exception e) {
                NetDebug.Log(e.ToString());
            }
        }

        //~Session() {
        //    Disconnect();
        //}

        public bool StartServer(int port, int connectionMax) {
            bool ret = CreateListener(port, connectionMax);
            if (!ret) return false;
            if (!_threadLoop) {
                CreateThread();
            }

            _port = port;
            _isServer = true;
            return true;
        }

        public void StopServer() {
            _isServer = false;
            DestroyThread();
            DestroyListener();
            NetDebug.Log("Server stopped");
        }

        protected bool CreateThread() {
            NetDebug.LogError("Make Thread: " + GetConnectionType());
            try
            {
                _thread = new Thread(ThreadDispatch);
                _threadLoop = true;
                _thread.Start();
            }
            catch (Exception e) {
                NetDebug.LogError(e.ToString());
                return false; }
            return true;
        }

        protected bool DestroyThread() {
            if (_threadLoop) {
                _threadLoop = false;
                if (_thread != null) {
                    _thread.Join();
                    _thread = null;
                }
            }
            return true;
        }

        protected int JoinSession(Socket socket) {
            T transport = new T();

            if (socket != null) {
                transport.Initialize(socket);
            }

            return JoinSession(transport);
        }

        protected int JoinSession(T transport) {
            int node = -1;
            lock (_nodeIndexLock) {
                node = _nodeIndex;
                _nodeIndex++;
            }

            transport.SetNodeId(node);
            transport.RegisterEventHandler(OnEventHandling);
            try {
                lock (_transperLock) {
                    _transports.Add(node, transport);
                }
            }
            catch(System.Exception e){
                NetDebug.LogError(e.ToString());
                return -1;
            }
            return node;
        }

        protected bool LeaveSession(int node) {
            if (node < 0) return false;
            T transport = (T)_transports[node];
            if (transport == null) return false;
            lock (_transperLock) {
                transport.Terminate();
                _transports.Remove(node);
            }
            return true;
        }

        public bool IsServer() { return _isServer; }

        public bool IsConnected(int node) {
            if (!_transports.ContainsKey(node)) return false;
            return _transports[node].IsConnected();
        }

        public int GetSessionCount() {
            return _transports.Count;
        }

        public IPEndPoint GetLocalEndPoint(int node) {
            if (!_transports.ContainsKey(node)) {
                return default(IPEndPoint);
            }

            IPEndPoint ep;
            T transport = _transports[node];
            ep = transport.GetLocalEndPoint();
            return ep;
        }

        public IPEndPoint GetRemoteEndPoint(int node) {
            if (!_transports.ContainsKey(node)) {
                return default(IPEndPoint);
            }
            IPEndPoint ep;
            T transport = _transports[node];
            ep = transport.GetRemoteEndPoint();
            return ep;
        }

        int FindTransport(IPEndPoint sender) {
            foreach (var kv in _transports) {
                IPEndPoint ep = kv.Value.GetLocalEndPoint();
                if (ep.Address.ToString() == sender.Address.ToString()) {
                    return kv.Key;
                }
            }
            return -1;
        }

        public virtual void ThreadDispatch() {
            while (_threadLoop) {
                AcceptClient();
                Dispatch();
                Thread.Sleep(3);
            }
        }

        public virtual int Connect(string addr, int port) {
            NetDebug.Log("Connect called");
            if (!_threadLoop) {
                CreateThread();
            }

            int node = -1;
            bool ret = false;
            try
            {
                NetDebug.Log("Transport connect : " + addr +" : " + port);
                T transport = new T();
                transport.SetServerPort(port);
                ret = transport.Connect(addr, port);
                if (ret)
                {
                    node = JoinSession(transport);
                    NetDebug.Log("Join session : " + node);
                }
                else {
                    NetDebug.LogError("Failed To Session : " + node);
                }
            }

            catch {
                NetDebug.LogError("connecation failed, Execption");
            }
            if (_handler != null) {
                NetEventState state = new NetEventState(NetEventType.Connect, ret ? NetEventResult.Success : NetEventResult.Failure);
                _handler(node, state);
            }

            return node;
        }

        public virtual bool Disconnect(int node) {
            if (node < 0) return false;
            if (_transports == null) return false;

            if (!_transports.ContainsKey(node)) { return false; }

            T transport = _transports[node];
            if (transport != null) {
                transport.Disconnect();
                LeaveSession(node);
            }

            if (_handler != null) {
                NetEventState state = new NetEventState(NetEventType.Disconnect, NetEventResult.Success);
                _handler(node, state);
            }

            return true;
        }

        public virtual bool Disconnect() {
            DestroyThread();
            lock (_transperLock) {
                Dictionary<int, T> transports = new Dictionary<int, T>(_transports);
                foreach (T tr in transports.Values) {
                    tr.Disconnect();
                    tr.Terminate();
                }

            }
            return true;
        }

        public virtual int Send(int node, byte[] data, int size) {
            if (node < 0) return -1;
            int sendSize = 0;
            try
            {
                T transport = (T)_transports[node];
                sendSize = transport.Send(data, size);
            }
            catch {
                return -1;
            }
            return sendSize;
        }

        public virtual int Receive(int node, ref byte[] buffer) {
            if (node < 0) {
                return -1;
            }

            int recvSize = 0;
            try
            {
                T transport = _transports[node];
                recvSize = transport.Receive(ref buffer, buffer.Length);
            }
            catch { return -1; }

            return recvSize;
        }

        public virtual void Dispatch() {
            Dictionary<int, T> transports = new Dictionary<int, T>(_transports);
            foreach (T tr in transports.Values) {
                if (tr != null) {
                    tr.Dispatch();
                }
            }
            DispatchReceive();
        }

        public virtual void DispatchReceive() {

        }

        public void RegisterEventHnadler(EventHandler handler) {
            _handler += handler;
        }

        public void UnregisterEvenvtHandler(EventHandler handler) {
            _handler -= handler;
        }

        public virtual void OnEventHandling(ITransport transport, NetEventState state) {
            int node = transport.GetNodeId();
            NetDebug.Log(string.Format("Signal {0} : {1} : {2}", node, state.type, state.result));

            do
            {
                if (!_transports.ContainsKey(node)) {
                    NetDebug.LogError("Not Found " + node);
                    break;
                }

                switch (state.type) {
                    case NetEventType.Connect:
                        break;
                    case NetEventType.Disconnect:
                        LeaveSession(node);
                        break;
                }
            }
            while (false);

            if (_handler != null) {
                _handler(node, state);
            }
        }

        public virtual NetworkModule.ConnectionType GetConnectionType() {
            return NetworkModule.ConnectionType.BOTH;
        }

        public abstract bool CreateListener(int port, int connectionMax);
        public abstract bool DestroyListener();
        public abstract void AcceptClient();
    }
}