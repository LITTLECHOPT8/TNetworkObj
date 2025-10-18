using NetworkObj.Utils;

public class GLeaveRoom : ServerPacket
{
    public uint m_iUserId;

    public Writer Pack()
    {
        Writer p = new Writer();
        p.wuint(m_iUserId);

        return Packet.Pack(Protocols.GC_LEAVE_ROOM_NOTIFY, p);
    }
}
