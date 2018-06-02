using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace NetworkComponents
{
    public class SessionTCP : Session<TransportTCP>
    {
        public override void AcceptClient()
        {
            if ((_listener != null) && _listener.Poll(0, SelectMode.SelectRead)) {
                NetDebug.Log("TCP Accept client");
                Socket socket = _listener.Accept();
                int node = -1;
                try
                {
                    NetDebug.Log("TCP Create transport");
                    TransportTCP transport = new TransportTCP();
                    transport.Initialize(socket);
                    transport.transportName = "serverSocket";
                    NetDebug.Log("TCP Join Session");
                    node = JoinSession(transport);
                }
                catch {
                    NetDebug.LogError("TCP Connect Failed");
                    return;
                }

                if (node >= 0 && _handler != null) {
                    NetEventState state = new NetEventState(NetEventType.Connect, NetEventResult.Success);
                    _handler(node, state);
                }

                NetDebug.Log(string.Format("TCP Connected from client : port[{0}], node[{1}]" , _port, node));
            }
        }

        public override bool CreateListener(int port, int connectionMax)
        {
            try
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _listener.Listen(connectionMax);
            }
            catch { return false; }
            return true;
        }

        public override bool DestroyListener()
        {
            if (_listener == null) return false;
            _listener.Close();
            _listener = null;
            return true;
        }

        public override NetworkModule.ConnectionType GetConnectionType()
        {
            return NetworkModule.ConnectionType.TCP;
        }
    }
}