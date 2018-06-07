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

    public class StartSessionNoticePacket : IPacket<StartSessionNotice>
    {
        public class StartSessionNoticePacketSerializer : Serializer {
            public bool Serialize(StartSessionNotice packet) {
                Serialize(packet.sessionId);
                Serialize(packet.stageRandomSeed);
                return true;
            }

            public bool Deserialize(ref StartSessionNotice data) {
                Deserialize(ref data.sessionId);
                Deserialize(ref data.stageRandomSeed);
                return true;
            }
        }

        StartSessionNotice _packet;

        public StartSessionNoticePacket(StartSessionNotice data) {
            _packet = data;
        }

        public StartSessionNoticePacket(byte[] data) {
            StartSessionNoticePacketSerializer serializer = new StartSessionNoticePacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }
        
        public byte[] GetData()
        {
            StartSessionNoticePacketSerializer serializer = new StartSessionNoticePacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public StartSessionNotice GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.StartSessionNotify;
        }
    }

    public class GameServerRequestPacket : IPacket<GameServerRequest> {

        public class GameServerRequestPacketSerializer : Serializer {
            public bool Serializer(GameServerRequest request) {
                Serialize((int)request.request);
                return true;
            }

            public bool Deserialize(ref GameServerRequest data) {
                int type = 0;
                Deserialize(ref type);
                data.request = (GameServerRequestType)type;
                return true;
            }
        }

        GameServerRequest _packet;

        public GameServerRequestPacket(GameServerRequest packet) {
            _packet = packet;
        }

        public GameServerRequestPacket(byte[] data) {
            GameServerRequestPacketSerializer serializer = new GameServerRequestPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            GameServerRequestPacketSerializer serializer = new GameServerRequestPacketSerializer();
            serializer.Serializer(_packet);
            return serializer.GetSerializedData();
        }

        public GameServerRequest GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.GameServerRequest;
        }
    }

    public class StageStartTimePacket : IPacket<StageStartTime>
    {
        public class StageStartTimePacketSerializer : Serializer {
            public bool Serialize(StageStartTime packet) {
                long time = packet.startTime.Ticks;
                Serialize(time);
                return true;
            }

            public bool Deserialize(ref StageStartTime data) {
                long time = 0;
                Deserialize(ref time);
                data.startTime = new DateTime(time);
                return true;
            }
        }

        StageStartTime packet;

        public StageStartTimePacket(StageStartTime packet)
        {
            this.packet = packet;
        }

        public StageStartTimePacket(byte[] data)
        {
            StageStartTimePacketSerializer serializer = new StageStartTimePacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref packet);
        }

        public byte[] GetData()
        {
            StageStartTimePacketSerializer serializer = new StageStartTimePacketSerializer();
            serializer.Serialize(packet);
            return serializer.GetSerializedData();
        }

        public StageStartTime GetPacket()
        {
            return packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.StageStartTime;
        }

    }

    public class PlayerStateInfoPacket : IPacket<PlayerStateInfo>{
        public class PlayerStateInfoPacketSerializer : Serializer
        {
            public bool Serialize(PlayerStateInfo packet) {
                Serialize(packet.accountId);
                Serialize(packet.playerHp);
                Serialize(packet.isPlayerDead);
                return true;
            }

            public bool Deserialize(ref PlayerStateInfo packet) {
                Deserialize(ref packet.accountId);
                Deserialize(ref packet.playerHp);
                Deserialize(ref packet.isPlayerDead);
                return true;
            }

        }

        private PlayerStateInfo packet;

        public PlayerStateInfoPacket(PlayerStateInfo packet) {
            this.packet = packet;
        }

        public PlayerStateInfoPacket(byte[] data) {
            PlayerStateInfoPacketSerializer serializer = new PlayerStateInfoPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref packet);
        }

        public PacketId GetPacketID()
        {
            return PacketId.CharacterData;
        }

        public PlayerStateInfo GetPacket()
        {
            return packet;
        }

        public byte[] GetData()
        {
            PlayerStateInfoPacketSerializer serializer = new PlayerStateInfoPacketSerializer();
            serializer.Serialize(packet);
            return serializer.GetSerializedData();
        }
    }

    public class PlayerItemInfoPacket : IPacket<PlayerItemInfo>{
        public class PlayerItemInfoPacketSerializer : Serializer {
            public bool Serialize(PlayerItemInfo packet) {
                Serialize(packet.accountId);
                Serialize(packet.itemId);
                Serialize(packet.isAcquire);
                return true;
            }

            public bool Deserialize(ref PlayerItemInfo packet) {
                Deserialize(ref packet.accountId);
                Deserialize(ref packet.itemId);
                Deserialize(ref packet.isAcquire);
                return true;
            }
        }

        private PlayerItemInfo _packet;

        public PlayerItemInfoPacket(PlayerItemInfo packet) {
            _packet = packet;
        }

        public PlayerItemInfoPacket(byte[] data) {
            PlayerItemInfoPacketSerializer serializer = new PlayerItemInfoPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public PacketId GetPacketID()
        {
            return PacketId.PlayerItemInfo;
        }

        public PlayerItemInfo GetPacket()
        {
            return _packet;
        }

        public byte[] GetData()
        {
            PlayerItemInfoPacketSerializer serializer = new PlayerItemInfoPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }
    }

    public class PlayerAnimTriggerPacket : IPacket<PlayerAnimTrigger>
    {
        public class PlayerAnimTriggerPacketSerializer : Serializer {
            public bool Serialize(PlayerAnimTrigger packet) {
                Serialize(packet.playerId);
                Serialize(packet.animKey.Length);
                Serialize(packet.animKey, packet.animKey.Length);
                return true;
            }

            public bool Deserialize(ref PlayerAnimTrigger data) {
                Deserialize(ref data.playerId);
                Deserialize(ref data.animKeyLength);
                Deserialize(ref data.animKey, data.animKeyLength);
                return true;
            }
        }

        private PlayerAnimTrigger _packet;

        public PlayerAnimTriggerPacket(PlayerAnimTrigger packet) {
            _packet = packet;
        }

        public PlayerAnimTriggerPacket(byte[] data) {
            PlayerAnimTriggerPacketSerializer serializer = new PlayerAnimTriggerPacketSerializer();
            serializer.SetDesrializedData(data);
            serializer.Deserialize(ref _packet);
        }

        public byte[] GetData()
        {
            PlayerAnimTriggerPacketSerializer serializer = new PlayerAnimTriggerPacketSerializer();
            serializer.Serialize(_packet);
            return serializer.GetSerializedData();
        }

        public PlayerAnimTrigger GetPacket()
        {
            return _packet;
        }

        public PacketId GetPacketID()
        {
            return PacketId.PlayerAnimTrigger;
        }
    }
}