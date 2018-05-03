using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using UnityEngine;

namespace NetworkComponents
{
    public enum PacketId {
        GameSyncInfo,
        CharacterData,
        ItemData,
        Moving,

        Max//dummy End
    }

    public struct PacketHeader {
        public PacketId packetId;
    }


}
