using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkComponents {
    public class CharacterMovingPacket : IPacket<CharacterData>{
        class CharacterMovingSerializer : Serializer {
            public bool Serialize(CharacterData packet) {
                Serialize(packet.characterId, CharacterData.MAX_CHAR_ID);
                Serialize(packet.index);
                Serialize(packet.dataNum);

                for (int i = 0; i < packet.dataNum; ++i) {
                    Serialize(packet.coordinates[i].x);
                    Serialize(packet.coordinates[i].z);
                }
                return true;
            }

            public bool Deserialize(ref CharacterData data) {
                if (GetDataSize() == 0) {
                    return false;
                }

                Deserialize(ref data.characterId, CharacterData.MAX_CHAR_ID);
                Deserialize(ref data.index);
                Deserialize(ref data.dataNum);

                data.coordinates = new CharacterCoordinates[data.dataNum];
                for (int i = 0; i < data.dataNum; i++) {
                    Deserialize(ref data.coordinates[i].x);
                    Deserialize(ref data.coordinates[i].z);
                }

                return true;
            }
        }

        CharacterData _packet;

        public CharacterMovingPacket(CharacterData data) {
            _packet = data;
        }

        public CharacterMovingPacket(byte[] data) {
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
}