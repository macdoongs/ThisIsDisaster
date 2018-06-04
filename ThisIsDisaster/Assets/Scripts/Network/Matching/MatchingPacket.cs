using System.Collections;
using System.Collections.Generic;
using NetworkComponents.Matching;

namespace NetworkComponents.Packet {

    public class MatchingRequestPacket : IPacket<MatchingRequest>
    {
        public class MatchingRequestPacketSerializer : Serializer {
            public bool Serialize(MatchingRequest packet) {

                Serialize(packet.accountId);
                packet.accountLength = packet.accountName.Length;
                Serialize(packet.accountLength);
                Serialize(packet.accountName, packet.accountLength);

                return true;
            }

            public bool Deserialize(ref MatchingRequest data) {
                Deserialize(ref data.accountId);
                Deserialize(ref data.accountLength);
                Deserialize(ref data.accountName, data.accountLength);
                return true;
            }
        }

        private MatchingRequest _packet;

        public MatchingRequestPacket(MatchingRequest packet) {
            _packet = packet;
        }

        public MatchingRequestPacket(byte[] data) {
            MatchingRequestPacketSerializer serializer = new MatchingRequestPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            MatchingRequestPacketSerializer serializer = new MatchingRequestPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public MatchingRequest GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.MatchingRequest;
        }
        
    }

    public class MatchingResponsePacket : IPacket<MatchingResponse>
    {
        public class MatchingResponsePacketSerializer : Serializer {
            public bool Serialize(MatchingResponse packet) {
                Serialize(packet.connectionState);
                Serialize(packet.nodeIndex);
                Serialize(packet.sessionIndex);
                return true;
            }
            public bool Deserialize(ref MatchingResponse data) {
                Deserialize(ref data.connectionState);
                Deserialize(ref data.nodeIndex);
                Deserialize(ref data.sessionIndex);
                return true;
            }
        }

        MatchingResponse _packet;

        public MatchingResponsePacket(MatchingResponse packet) {
            _packet = packet;
        }

        public MatchingResponsePacket(byte[] data) {
            MatchingResponsePacketSerializer serializer = new MatchingResponsePacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            MatchingResponsePacketSerializer serializer = new MatchingResponsePacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public MatchingResponse GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.MatchingResponse;
        }
    }

    public class MatchingDataPacket : IPacket<MatchingData>
    {
        public class MatchingDataPacketSerializer : Serializer {
            public bool Serialize(MatchingData packet) {
                Serialize(packet.sessionId);
                Serialize(packet.serverAccountId);
                Serialize(packet.nodeCount);
                
                for (int i = 0; i < packet.nodeCount; i++) {
                    Serialize(packet.nodes[i].nodeIndex);
                    Serialize(packet.nodes[i].playerName.Length);
                    Serialize(packet.nodes[i].playerName, packet.nodes[i].playerName.Length);
                    Serialize(packet.nodes[i].isReady);
                    Serialize(packet.nodes[i].accountId);
                    Serialize(packet.nodes[i].port);
                    Serialize(packet.nodes[i].ip, MatchingNode.IP_LENGTH);
                }
                return true;
            }

            public bool Deserialize(ref MatchingData data) {
                Deserialize(ref data.sessionId);
                Deserialize(ref data.serverAccountId);
                Deserialize(ref data.nodeCount);
                data.nodes = new MatchingNode[data.nodeCount];
                for (int i = 0; i < data.nodeCount; i++) {
                    Deserialize(ref data.nodes[i].nodeIndex);
                    Deserialize(ref data.nodes[i].playerNameLength);
                    Deserialize(ref data.nodes[i].playerName, data.nodes[i].playerNameLength);
                    Deserialize(ref data.nodes[i].isReady);
                    Deserialize(ref data.nodes[i].accountId);
                    Deserialize(ref data.nodes[i].port);
                    Deserialize(ref data.nodes[i].ip, MatchingNode.IP_LENGTH);
                }
                return true;
            }
        }

        MatchingData _packet;

        public MatchingDataPacket(MatchingData packet) {
            _packet = packet;
        }

        public MatchingDataPacket(byte[] data) {
            MatchingDataPacketSerializer serializer = new MatchingDataPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            MatchingDataPacketSerializer serializer = new MatchingDataPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public MatchingData GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.MatchingData;
        }
    }

    public class MatchingReadyStatePacket : IPacket<MatchingReadyState>
    {
        public class MatchingReadyStatePacketSerializer : Serializer {
            public bool Serialize(MatchingReadyState packet) {
                Serialize(packet.accountId);
                Serialize(packet.isReady);
                return true;
            }

            public bool Deserialize(ref MatchingReadyState data) {
                Deserialize(ref data.accountId);
                Deserialize(ref data.isReady);
                return true;
            }
        }

        private MatchingReadyState packet;

        public MatchingReadyStatePacket(MatchingReadyState packet) {
            this.packet = packet;
        }

        public MatchingReadyStatePacket(byte[] data) {
            MatchingReadyStatePacketSerializer serializer = new MatchingReadyStatePacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref packet);
        }

        public byte[] GetData()
        {
            MatchingReadyStatePacketSerializer serializer = new MatchingReadyStatePacketSerializer();
            serializer.Serialize(packet);
            return serializer.GetSerializedData();
        }

        public MatchingReadyState GetPacket()
        {
            return packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.MatchingReady;
        }
    }
}