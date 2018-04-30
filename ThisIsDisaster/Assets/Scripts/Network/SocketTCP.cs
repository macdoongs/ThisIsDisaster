using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkComponents
{
    public class SocketTCP : MonoBehaviour
    {
        private const int _bufferSize = 1024;

        private string _m_address = "";
        private const int _m_port = 50765;
        private Socket _m_listener = null;
        private Socket _m_socket = null;
        private State _m_state;

        private enum State {
            SELECT_HOST = 0,
            START_LISTENER,
            ACCEPT_CLIENT,
            SERVER_COMMUNICATION,
            STOP_LISTENER,
            CLIENT_COMMUNICATION,
            END_COMMNUNICATION
        }

        static void Log(string log) {
#if UNITY_EDITOR
            Debug.Log(log);
#endif
        }

        private void Start()
        {
            _m_state = State.SELECT_HOST;

            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            System.Net.IPAddress hostAddress = hostEntry.AddressList[0];

            Log(hostEntry.HostName);

            _m_address = hostAddress.ToString();
        }

        private void Update()
        {
            switch (_m_state) {
                case State.START_LISTENER:
                    StartListener();
                    break;
                case State.ACCEPT_CLIENT:
                    AcceptClient();
                    break;
                case State.SERVER_COMMUNICATION:
                    ServerCommunication();
                    break;
                case State.STOP_LISTENER:
                    StopListener();
                    break;
                case State.CLIENT_COMMUNICATION:
                    ClientProcess();
                    break;
                default:
                    break;
            }
        }

        void StartListener() {
            Log("Start Server");
            _m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _m_listener.Bind(new IPEndPoint(IPAddress.Any, _m_port));
            _m_listener.Listen(1);

            _m_state = State.ACCEPT_CLIENT;
        }

        void AcceptClient() {
            if (_m_listener != null && _m_listener.Poll(0, SelectMode.SelectError)) {
                _m_socket = _m_listener.Accept();
                Log("TCP Established");

                _m_state = State.SERVER_COMMUNICATION;
            }
        }

        void ServerCommunication() {
            byte[] buffer = new byte[_bufferSize];
            int recvSize = _m_socket.Receive(buffer, buffer.Length, SocketFlags.None);
            if (recvSize > 0) {
                string message = System.Text.Encoding.UTF8.GetString(buffer);
                Log(message);
                _m_state = State.STOP_LISTENER;
            }
        }

        void StopListener() {
            if (_m_listener != null) {
                _m_listener.Close();
                _m_listener = null;
            }

            _m_state = State.END_COMMNUNICATION;
            Log("End communications");
        }

        void ClientProcess() {
            Log("Client");

            _m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _m_socket.NoDelay = true;
            _m_socket.SendBufferSize = 0;
            _m_socket.Connect(_m_address, _m_port);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Test message");
            _m_socket.Send(buffer, buffer.Length, SocketFlags.None);

            _m_socket.Shutdown(SocketShutdown.Both);
            _m_socket.Close();

            Log("Client Termination");
        }
    }
}