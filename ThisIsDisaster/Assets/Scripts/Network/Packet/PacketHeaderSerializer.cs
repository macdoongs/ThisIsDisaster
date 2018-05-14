using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace NetworkComponents
{
    public class PacketHeaderSerializer : Serializer {
        public bool Serialize(PacketHeader data) {
            Clear();
            bool ret = true;
            ret &= Serialize((int)data.packetId);
            ret &= Serialize(data.packetSender);
            return ret;
        }

        public bool Deserialize(byte[] data, ref PacketHeader serialized) {
            bool ret = SetDesrializedData(data);
            if (!ret) return false;
            int packetId = 0;
            int packetSender = 0;
            ret &= Deserialize(ref packetId);
            ret &= Deserialize(ref packetSender);
            serialized.packetId = (PacketId)packetId;
            serialized.packetSender = packetSender;
            return ret;
        }
    }
}
