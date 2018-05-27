using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents {
    public delegate void EventHandler(ITransport transport, NetEventState state);

    public static class NetDebug
    {
        const string header = "[Network] ";
#if UNITY_EDITOR && NET_DEBUG || true
        public static void Log(string log)
        {
            UnityEngine.Debug.Log(header + log);
        }

        public static void LogError(string log)
        {
            UnityEngine.Debug.LogError(header + log);
        }
#endif
    }

    public interface ITransport {
        bool Initialize(Socket socket);
        bool Terminate();
        int GetNodeId();
        void SetNodeId(int node);
        IPEndPoint GetLocalEndPoint();
        IPEndPoint GetRemoteEndPoint();
        int Send(byte[] data, int size);
        int Receive(ref byte[] buffer, int size);
        bool Connect(string ipAddress, int port);
        void Disconnect();
        void Dispatch();
        bool IsConnected();
        void RegisterEventHandler(EventHandler handler);
        void UnregisterEventHandler(EventHandler handler);
        void SetServerPort(int port);
    }

}