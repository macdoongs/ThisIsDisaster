using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace NetworkComponents
{
    public class SessionUDP : Session<TransportUDP>
    {
        private Dictionary<string, int> _nodeAddress = new Dictionary<string, int>();

        public SessionUDP() {
            _nodeIndex = 10000;
        }

        public override bool CreateListener(int port, int connectionMax)
        {
            try
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                NetDebug.Log("Create UDP Listener : " + port);
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

        public override void AcceptClient()
        {

        }

        public override void DispatchReceive()
        {
            if (_listener != null && _listener.Poll(0, SelectMode.SelectRead)) {
                byte[] buffer = new byte[_mtu];
                IPEndPoint addr = new IPEndPoint(IPAddress.Any, 0);
                EndPoint endPoint = (EndPoint)addr;
                int recvSize = _listener.ReceiveFrom(buffer, SocketFlags.None, ref endPoint);
                if (recvSize == 0)
                {

                }
                else if (recvSize < 0) {
                    //통신에러
                }
                IPEndPoint iep = (IPEndPoint)endPoint;
                string nodeAddr = iep.Address.ToString() + ":" + iep.Port;
                int node = -1;
                string str = System.Text.Encoding.UTF8.GetString(buffer).Trim('\0');
                if (str.Contains(TransportUDP._requestData))
                {
                    string[] strAry = str.Split(':');
                    IPEndPoint ep = new IPEndPoint(IPAddress.Parse(strAry[0]), int.Parse(strAry[1]));

                    if (_nodeAddress.ContainsKey(nodeAddr))
                    {
                        node = _nodeAddress[nodeAddr];
                    }
                    else
                    {
                        NetDebug.Log("Not contains Key: " + nodeAddr);
                        node = GetNodeFromEndPoint(ep);
                        if (node >= 0)
                        {
                            _nodeAddress.Add(nodeAddr, node);
                        }
                    }

                }
                else {
                    if (_nodeAddress.ContainsKey(nodeAddr)) {
                        node = _nodeAddress[nodeAddr];
                    }
                }

                if (node >= 0) {
                    TransportUDP transport = _transports[node];
                    transport.SetReceiveData(buffer, recvSize, (IPEndPoint)endPoint);
                }
            }
        }

        private int GetNodeFromEndPoint(IPEndPoint endPoint) {
            foreach (int node in _transports.Keys) {
                TransportUDP transport = _transports[node];
                IPEndPoint transportEp = transport.GetRemoteEndPoint();
                if (transportEp != null) {
                    Debug.Log("NodeFromEP recv[node:" + node + "] " + ((IPEndPoint)endPoint).Address.ToString() + ":" + endPoint.Port 
                            + " transport:" + transportEp.Address.ToString() + ":" + transportEp.Port);
                    if (
                        transportEp.Port == endPoint.Port &&
                        transportEp.Address.ToString() == endPoint.Address.ToString()
                        )
                    {
                        return node;
                    }
                }
            }
            return -1;
        }
    }
}