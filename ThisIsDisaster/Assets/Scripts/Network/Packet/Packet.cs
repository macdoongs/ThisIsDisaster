using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkComponents
{
    public class GameSyncPacket : IPacket<GameSyncData>
    {
        public class GameSyncSerializer : Serializer
        {
            public bool Serialize(GameSyncData packet)
            {
                Serialize(packet.serverVersion);
                Serialize(packet.accountName, GameSyncData.MAX_ACCOUNT_LENGTH);
                Serialize(packet.accountId);
                Serialize(packet.stageGenSeed);
                return true;
            }

            public bool Deserialize(ref GameSyncData data)
            {
                if (GetDataSize() == 0) return false;
                Deserialize(ref data.serverVersion);
                Deserialize(ref data.accountName, GameSyncData.MAX_ACCOUNT_LENGTH);
                Deserialize(ref data.accountId);
                Deserialize(ref data.stageGenSeed);
                return true;
            }
        }

        GameSyncData _packet;

        public GameSyncPacket(GameSyncData data)
        {
            _packet = data;
        }

        public GameSyncPacket(byte[] data)
        {
            GameSyncSerializer serializer = new GameSyncSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            GameSyncSerializer serializer = new GameSyncSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public GameSyncData GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.GameSyncInfo;
        }
    }

    public class CharacterMovingPacket : IPacket<CharacterData>
    {
        class CharacterMovingSerializer : Serializer
        {
            public bool Serialize(CharacterData packet)
            {
                Serialize(packet.index);
                Serialize(packet.dataNum);

                for (int i = 0; i < packet.dataNum; ++i)
                {
                    Serialize(packet.coordinates[i].x);
                    Serialize(packet.coordinates[i].y);
                    Serialize(packet.coordinates[i].z);
                }
                return true;
            }

            public bool Deserialize(ref CharacterData data)
            {
                if (GetDataSize() == 0)
                {
                    return false;
                }

                Deserialize(ref data.index);
                Deserialize(ref data.dataNum);

                data.coordinates = new CharacterCoordinates[data.dataNum];
                for (int i = 0; i < data.dataNum; i++)
                {
                    Deserialize(ref data.coordinates[i].x);
                    Deserialize(ref data.coordinates[i].y);
                    Deserialize(ref data.coordinates[i].z);
                }

                return true;
            }
        }

        CharacterData _packet;

        public CharacterMovingPacket(CharacterData data)
        {
            _packet = data;
        }

        public CharacterMovingPacket(byte[] data)
        {
            CharacterMovingSerializer serializer = new CharacterMovingSerializer();

            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public PacketId GetPacketID()
        {
            return PacketId.Coordinates;
        }

        public CharacterData GetPacket()
        {
            return _packet;
        }

        public byte[] GetData()
        {
            CharacterMovingSerializer serializer = new CharacterMovingSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }
    }

    public class SessionSyncInfoPacket : IPacket<SessionSyncInfo>
    {
        public class SessionSyncInfoSerializer : Serializer
        {
            public bool Serialize(SessionSyncInfo packet)
            {
                Serialize(packet.accountId);
                Serialize(packet.serverPort);
                Serialize(packet.ip.Length);
                Serialize(packet.ip, packet.ip.Length);
                return true;
            }

            public bool Deserialize(ref SessionSyncInfo data)
            {
                if (GetDataSize() == 0) return false;
                Deserialize(ref data.accountId);
                Deserialize(ref data.serverPort);
                Deserialize(ref data.ipLength);
                Deserialize(ref data.ip, data.ipLength);
                return true;
            }
        }

        SessionSyncInfo _packet;

        public SessionSyncInfoPacket(SessionSyncInfo data)
        {
            _packet = data;
        }

        public SessionSyncInfoPacket(byte[] data)
        {
            SessionSyncInfoSerializer serializer = new SessionSyncInfoSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public PacketId GetPacketID()
        {
            return PacketId.SessionInfoSync;
        }

        public SessionSyncInfo GetPacket()
        {
            return _packet;
        }

        public byte[] GetData()
        {
            SessionSyncInfoSerializer serializer = new SessionSyncInfoSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }
    }

    public class SessionSyncInfoReflectionPacket : IPacket<SessionSyncInfoReflection>
    {
        class SessionSyncInfoReflectionPacketSerializer : Serializer
        {
            public bool Serialize(SessionSyncInfoReflection packet) {
                Serialize(packet.nodeIndex);
                Serialize(packet.isConnection);
                Serialize(packet.nodeAccountId);
                Serialize(packet.nodeServerPort);
                Serialize(packet.nodeIp.Length);
                Serialize(packet.nodeIp, packet.nodeIp.Length);
                return true;
            }

            public bool Deserialize(ref SessionSyncInfoReflection data) {
                Deserialize(ref data.nodeIndex);
                Deserialize(ref data.isConnection);
                Deserialize(ref data.nodeAccountId);
                Deserialize(ref data.nodeServerPort);
                Deserialize(ref data.ipLength);
                Deserialize(ref data.nodeIp, data.ipLength);
                return true;
            }
        }

        SessionSyncInfoReflection _packet;

        public SessionSyncInfoReflectionPacket(SessionSyncInfoReflection data) {
            _packet = data;
        }

        public SessionSyncInfoReflectionPacket(byte[] data) {
            SessionSyncInfoReflectionPacketSerializer serializer = new SessionSyncInfoReflectionPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            SessionSyncInfoReflectionPacketSerializer serializer = new SessionSyncInfoReflectionPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public SessionSyncInfoReflection GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.SessionInfoSyncReflection;
        }
    }
}