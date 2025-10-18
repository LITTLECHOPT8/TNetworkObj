using NetworkObj.Utils;
using System.Text;

public class GJoinRoomNotify : ServerPacket
{
    public uint m_iUserId;

    public string m_strNickname;

    public uint m_iAvatarType;

    public uint m_iLevel;

    public uint m_room_index;

    public Writer Pack()
    {
        Writer p = new Writer();
        p.wuint(m_iUserId);

        byte[] nick = new byte[16];
        byte[] unick = Encoding.ASCII.GetBytes(m_strNickname);
        Array.Copy(unick, 0, nick, 0, (uint)Math.Min(16, unick.Length));
        p.wuint((uint)Math.Min(16, unick.Length));
        p.wbytes(nick);

        p.wuint(m_iAvatarType);
        p.wuint(m_iLevel);
        p.wuint(m_room_index);

        return Packet.Pack(Protocols.GC_JOIN_ROOM_NOTIFY, p);
    }
}
