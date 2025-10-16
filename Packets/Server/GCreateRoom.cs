using NetworkObj.Utils;

public class GCreateRoom : ServerPacket
{
    public uint m_iResult;

    public uint m_iUserId;

    public uint m_iRoomId;

    public long m_lLocalTime;

    public long m_lServerTime;

    public Writer Pack()
    {
        Writer p = new Writer();
        p.wuint(m_iResult);
        p.wuint(m_iUserId);
        p.wuint(m_iRoomId);
        p.wulong((ulong)m_lLocalTime);
        p.wulong((ulong)m_lServerTime);

        return Packet.Pack(Protocols.GC_CREATE_ROOM, p);
    }
}
