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
            return ret;
        }

        public bool Deserialize(byte[] data, ref PacketHeader serialized) {
            bool ret = SetDesrializedData(data);
            if (!ret) return false;
            int packetId = 0;
            ret &= Deserialize(ref packetId);
            serialized.packetId = (PacketId)packetId;
            return ret;
        }
    }
}
