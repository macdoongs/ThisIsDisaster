using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkComponents
{
    public interface IPacket<T>
    {
        PacketId GetPacketID();
        T GetPacket();
        byte[] GetData();
    }
}
