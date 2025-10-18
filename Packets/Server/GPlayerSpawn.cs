using NetworkObj.Utils;

namespace NetworkObj.Packets
{
    class GPlayerSpawn : ServerPacket
    {
        public uint m_iUserId;

        public uint m_iBirthPointIndex;

        public uint m_iWeaponIndex1;

        public uint m_iWeaponIndex2;

        public uint m_iWeaponIndex3;

        public long m_lLocalTime;

        public long m_lServerTime;

        public Writer Pack()
        {
            Writer p = new Writer();
            p.wuint(m_iUserId);
            p.wuint(m_iBirthPointIndex);
            p.wuint(m_iWeaponIndex1);
            p.wuint(m_iWeaponIndex2);
            p.wuint(m_iWeaponIndex3);
            p.wulong((ulong)m_lLocalTime);
            p.wulong((ulong)m_lServerTime);

            return Packet.Pack(Protocols.GC_USER_SPAWN, p);
        }
    }
}