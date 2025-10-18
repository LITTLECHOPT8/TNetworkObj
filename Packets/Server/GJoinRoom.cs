using NetworkObj.Utils;

public class GJoinRoom : ServerPacket
{
    public uint m_iResult;

    public uint m_iRoomId;

    public long m_lLocalTime;

    public long m_lServerTime;

    public uint m_iUserId;

    public uint m_room_index;

    public uint m_map_id;

    public Writer Pack()
    {
        Writer p = new Writer();
        p.wuint(m_iResult);
        p.wuint(m_iRoomId);
        p.wulong((ulong)m_lLocalTime);
        p.wulong((ulong)m_lServerTime);
        p.wuint(m_iUserId);
        p.wuint(m_room_index);
        p.wuint(m_map_id);

        return Packet.Pack(Protocols.GC_JOIN_ROOM, p);
    }
}
