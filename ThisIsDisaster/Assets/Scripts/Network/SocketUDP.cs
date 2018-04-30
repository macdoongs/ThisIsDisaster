using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using UnityEngine;


namespace NetworkComponents {
    public class SocketUDP : MonoBehaviour {
        const int _bufferSize = 1024;
        private string m_address = "";
        private const int m_port = 50765;
        private Socket m_socket = null;
        private State m_state;

        private enum State {
            SELECT_HOST=  0,
            CREATE_LISTENER,
            RECEIVE_MESSAGE,
            CLOSE_LISTENER,
            SEND_MESAGE,
            END_COMMUNICATION
        }

        static void Log(string log) {
#if UNITY_EDITOR
            Debug.Log(log);
#endif
        }

        private void Start()
        {
            m_state = State.SELECT_HOST;

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            System.Net.IPAddress hostAddress = hostEntry.AddressList[0];
            Log(hostEntry.HostName);
            m_address = hostAddress.ToString();
        }

        private void Update()
        {
            switch (m_state) {
                case State.CREATE_LISTENER:
                    CreateListener();
                    break;
                case State.RECEIVE_MESSAGE:
                    ReceiveMessage();
                    break;
                case State.CLOSE_LISTENER:
                    CloseListener();
                    break;
                case State.SEND_MESAGE:
                    SendMessage();
                    break;

                default: break;
            }   
        }

        void CreateListener() {
            Log("Start Comm");

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_socket.Bind(new IPEndPoint(IPAddress.Any, m_port));
            m_state = State.RECEIVE_MESSAGE;
        }

        void ReceiveMessage() {
            byte[] buffer = new byte[_bufferSize];
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)sender;

            if (m_socket.Poll(0, SelectMode.SelectRead)) {
                int recvSize = m_socket.ReceiveFrom(buffer, SocketFlags.None, ref senderRemote);
                if (recvSize > 0) {
                    string message = System.Text.Encoding.UTF8.GetString(buffer);
                    Log(message);
                    m_state = State.CLOSE_LISTENER;
                }
            }
        }

        void CloseListener() {
            if (m_socket != null) {
                m_socket.Close();
                m_socket = null;
            }

            m_state = State.END_COMMUNICATION;
        }

        void SendMessage() {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("UDP test");
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(m_address), m_port);
            m_socket.SendTo(buffer, buffer.Length, SocketFlags.None, endPoint);

            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();
            m_state = State.END_COMMUNICATION;
        }
    }
}