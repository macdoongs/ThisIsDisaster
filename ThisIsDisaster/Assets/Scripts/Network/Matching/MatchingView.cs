using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using UnityEngine;

namespace NetworkComponents.Matching {
    public struct MatchingReadyState {
        public int accountId;
        public bool isReady;
    }

    public struct MatchingInfo {
        public int nodeIndex;
        public int accountId;
        public IPEndPoint endPoint;

        public MatchingInfo(int node, int account, IPEndPoint endPoint) {
            nodeIndex = node;
            accountId = account;
            this.endPoint = endPoint;
        }

        public MatchingInfo(int node, int account, int port, string ip) {
            nodeIndex = node;
            accountId = account;
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        
        public MatchingInfo(MatchingNode node) {
            nodeIndex = node.nodeIndex;
            accountId = node.accountId;
            endPoint = new IPEndPoint(IPAddress.Parse(node.ip), node.port);
        }
    }

    [System.Serializable]
    public struct MatchingNode
    {
        public int nodeIndex;
        public int accountId;
        public int playerNameLength;
        public string playerName;
        public bool isReady;
        public int port;
        public string ip;

        public const int IP_LENGTH = 32;
    }

    public struct MatchingData
    {
        public long sessionId;
        public int serverAccountId;
        public int nodeCount;
        public MatchingNode[] nodes;
    }

    public class MatchingView : MonoBehaviour
    {
        public long SessionId = 0;
        public int ServerAccountId = 0;
        public int NodeCount = 0;

        public List<MatchingNode> _nodes = new List<MatchingNode>();
        
        private void OnEnable()
        {
            NetworkModule.Instance.RegisterReceiveNotification(PacketId.MatchingData, OnReceiveMatchingData);
        }

        private void OnDisable()
        {
            NetworkModule.Instance.UnregisterReceiveNotification(PacketId.MatchingData);
        }

        public void AddHost() {
            MatchingNode node = new MatchingNode() {
                accountId = GlobalParameters.Param.accountId,
                nodeIndex = -1,
                ip = GameServer.Instance.LocalHost,
                port = GameServer.Instance.UdpServerPort,
                isReady = false,
                playerNameLength = GlobalParameters.Param.accountName.Length,
                playerName = GlobalParameters.Param.accountName
            };
            _nodes.Add(node);
            NodeCount = 1;
        }

        public void AddNode(int nodeIndex, int accountId, int port, string ip, string name) {
            MatchingNode node = new MatchingNode()
            {
                accountId = accountId,
                nodeIndex = nodeIndex,
                ip = ip,
                port = port,
                playerNameLength =  name.Length,
                playerName = name,
                isReady = false,
            };
            _nodes.Add(node);
            NodeCount++;
            Debug.LogError("Add Matching Node : " + nodeIndex + " " + accountId + " " + ip + " " + port);
        }

        public void SetNodeReadyState(int nodeAccount, bool state) {
            try
            {
                int index = -1;
                foreach (var node in _nodes) {
                    index++;
                    if (node.accountId == nodeAccount) {
                        break;
                    }
                }
                var target = _nodes[index];
                MatchingNode newData = new MatchingNode() {
                    accountId = target.accountId,
                    nodeIndex = target.nodeIndex,
                    ip = target.ip,
                    port = target.port,
                    playerNameLength = target.playerNameLength,
                    playerName = target.playerName,
                    isReady = state
                };
                _nodes[index] = newData;

            }
            catch {

            }
        }

        public bool RemoveNode(int nodeIndex) {
            try
            {
                var node = _nodes.Find((x => x.nodeIndex == nodeIndex));
                _nodes.Remove(node);
                NodeCount--;
                return true;
            }
            catch {

            }
            return false;
        }

        public void OnReceiveMatchingData(int node, PacketId packetId, byte[] data) {
            Packet.MatchingDataPacket packet = new Packet.MatchingDataPacket(data);
            var matching = packet.GetPacket();

            this.SessionId = matching.sessionId;
            this.ServerAccountId = matching.serverAccountId;
            this.NodeCount = matching.nodeCount;

            _nodes.Clear();
            _nodes.AddRange(matching.nodes);

            if (MatchingPanel.Instance != null) {
                MatchingPanel.Instance.SetMatchingData(matching);
            }

            PrintCurrentInfo();
        }

        void PrintCurrentInfo() {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Session ID : {0}\r\nHost Account ID:{1}\r\nCurrent Player Count : {2}\r\n", SessionId, ServerAccountId, NodeCount);
            foreach (var v in _nodes) {
                builder.AppendFormat("Player [{0}] Account:{1}//{2}:{3}", v.accountId, v.accountId, v.ip, v.port);
                builder.AppendLine();
            }
            Debug.LogError(builder.ToString());
        }

        void UpdateInfo() {

        }

        public MatchingData GetMatchingData() {
            MatchingData output = new MatchingData
            {
                sessionId = SessionId,
                serverAccountId = ServerAccountId,
                nodeCount = NodeCount,
                nodes = new MatchingNode[NodeCount]
            };
            for (int i = 0; i < NodeCount; i++) {
                MatchingNode node = _nodes[i];
                output.nodes[i] = node;
            }
            if (MatchingPanel.Instance != null)
            {
                MatchingPanel.Instance.SetMatchingData(output);
            }

            return output;
        }
    }

}
