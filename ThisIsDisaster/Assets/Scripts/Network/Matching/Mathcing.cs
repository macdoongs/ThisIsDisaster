using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace NetworkComponents.Matching {
    //Using Thread? or Just use Game Object Script?
    //use Game Object Script

    public class Mathcing : MonoBehaviour
    {
        public class Member
        {
            public int node = -1;
            public int sessionId = -1;
            IPEndPoint endPoint = null;
        }

        private class SessionInfo {
            public int node = -1;
            public int sessionId = -1;
            public bool isClosed = false;
            public float elapsedTime = 0.0f;
            public int[] members = Enumerable.Repeat(-1, _maxPlayer).ToArray();
        }

        private enum State
        {
            Init = 0,
            CreateSessionInfo,

            Waiting,

            JoinSession,
            StartStageRecevied,
            StartStageNotificated,
            End
        }

        private const int _maxPlayer = NetConfig.PLAYER_MAX;
        private int _counter = 0;

        private SessionInfo _sessionInfo = null;

        private NetworkModule _network = null;
        private NetworkModule Network {
            get {
                if (_network == null) {
                    _network = NetworkModule.Instance;
                }
                return _network;
            }
        }

        private Member[] _sessionMembers = new Member[_maxPlayer];
        private Dictionary<int, Member> _members = new Dictionary<int, Member>();

        private float _timer = 0f;
        private State _state = State.Init;
        private State _nextStep = State.Init;
        private bool _isHost = false;

        public void InitHost() {
            _state = State.Init;
            _nextStep = State.CreateSessionInfo;
            //session id will get from aws server, but now for test, use random integer number

            _isHost = true;

            if (Network != null) {
                //initialize notifier
                Network.StartServer(NetConfig.MATCHING_SERVER_PORT, 4, NetworkModule.ConnectionType.TCP);
            }


        }

        public void InitGuest()
        {
            _state = State.Init;
            //_sessionInfo = new SessionInfo();
            //session id will get from aws server, but now for test, use random integer number

            _isHost = false;

            if (Network != null)
            {
                //packet notifier
                if (_isHost)
                {
                    Network.StartServer(NetConfig.MATCHING_SERVER_PORT, 4, NetworkModule.ConnectionType.TCP);
                }
                else
                {
                    Network.Connect("", NetConfig.MATCHING_SERVER_PORT, NetworkModule.ConnectionType.TCP);
                }
            }
        }
        
        //update
        void DispatchHost() {
            switch (_state) {
                case State.CreateSessionInfo:
                    //wait
                    _state = State.Waiting;
                    break;
            }
        }

        void DispatchGuest() {
            switch (_state) {

            }
        }

        void Wait() {

        }

        void ConnectSessionMember() {
            _state = State.End;
        }

        void DisconnectSession() {
            foreach (int node in _sessionInfo.members) {
                if (node != -1) {
                    Network.Disconnect(node);
                    _members.Remove(node);
                }
            }
        }

        public bool IsMachingFinished() {
            return _state == State.End;
        }

        public bool IsHost() {
            return _isHost;
        }

        public int GetPlayerId() {
            //Get Player ID owned when sync with server
            return -1;
        }

        public Member[] GetMembers() {
            return _sessionMembers;
        }

        public int GetMemberNum() {
            int output = 0;
            foreach (var kv in _members) {
                if (kv.Value != null) {
                    if (kv.Value.node != -1) {
                        output++;
                    }
                }
            }
            return output;
        }

        void CreateSession(int node, int sessionId)
        {
            SessionInfo newInfo = new SessionInfo
            {
                sessionId = sessionId
            };
            newInfo.members[0] = node;
            _members[node].sessionId = sessionId;

            this._sessionInfo = newInfo;
            _state = State.CreateSessionInfo;
        }
    }
}
