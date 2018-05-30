using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkComponents.Packet
{
    public enum MatchingState {
        Connect,
        Disconnect
    }

    public struct SessionInfo {
        public MatchingState state;
        public int playerId;
        public int members;
        public EndPointData[] endPoints;
    }

    public struct EndPointData {
        public int port;
        public string ip;
        public const int IP_ADDR_LEGNTH = 32;
    }

    public class SessionInfoPacket : IPacket<SessionInfo>
    {
        public class SessionInfoPacketSerializer : Serializer {
            public bool Serialize(SessionInfo info) {
                Serialize((int)info.state);
                Serialize(info.playerId);
                Serialize(info.members);
                for (int i = 0; i < info.members; i++) {
                    EndPointData ep = info.endPoints[i];
                    Serialize(ep.port);
                    Serialize(ep.ip, EndPointData.IP_ADDR_LEGNTH);
                }
                return true;
            }

            public bool Deserialize(ref SessionInfo info) {
                int state = 0;
                Deserialize(ref state);
                info.state = (MatchingState)state;
                Deserialize(ref info.playerId);
                Deserialize(ref info.members);

                info.endPoints = new EndPointData[info.members];

                for (int i = 0; i < info.members; i++) {
                    Deserialize(ref info.endPoints[i].port);
                    Deserialize(ref info.endPoints[i].ip, EndPointData.IP_ADDR_LEGNTH);
                }

                return true;
            }
        }

        private SessionInfo _packet;

        public SessionInfoPacket(SessionInfo info) {
            _packet = info;
        }

        public SessionInfoPacket(byte[] data) {
            SessionInfoPacketSerializer serializer = new SessionInfoPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            SessionInfoPacketSerializer serializer = new SessionInfoPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public SessionInfo GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.SessionInfo;
        }
    }
}
