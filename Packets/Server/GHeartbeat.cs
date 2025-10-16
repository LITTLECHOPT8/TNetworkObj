using NetworkObj.Utils;

namespace NetworkObj.Packets
{
    class GHeartbeat : ServerPacket
    {
        public ulong m_lLocalTime;

        public Writer Pack()
        {
            Writer p = new Writer();
            p.wulong(m_lLocalTime);

            return Packet.Pack(Protocols.GC_HEARTBEAT, p);
        }
    }
}