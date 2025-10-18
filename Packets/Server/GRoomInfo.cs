using NetworkObj.Utils;
using System.Text;

namespace NetworkObj.Packets
{
    class GRoomInfo : ServerPacket
    {
        public uint m_iResult;

        public uint m_iRoomId;

        public uint m_iMapId;

        public string m_strCreaterNickname;

        public uint m_iOnlineNum;

        public uint m_iMaxUserNum;

        public uint m_room_status;

        public uint m_Creater_level;

        public string m_password;

        public Writer Pack()
        {
            Writer p = new Writer();
            p.wuint(m_iResult);
            p.wuint(m_iRoomId);
            p.wuint(m_iMapId);

            byte[] nick = new byte[16];
            byte[] unick = Encoding.ASCII.GetBytes(m_strCreaterNickname);
            Array.Copy(unick, 0, nick, 0, (uint)Math.Min(16, unick.Length));
            p.wuint((uint)Math.Min(16, unick.Length));
            p.wbytes(nick);

            p.wuint(m_iOnlineNum);
            p.wuint(m_iMaxUserNum);
            p.wuint(m_room_status);
            p.wuint(m_Creater_level);

            byte[] pasw = new byte[7];
            byte[] upasw = Encoding.ASCII.GetBytes(m_password);
            Array.Copy(upasw, 0, pasw, 0, (uint)Math.Min(7, upasw.Length));
            p.wuint((uint)Math.Min(7, upasw.Length));
            p.wbytes(pasw);

            return Packet.Pack(Protocols.GC_ROOM_INFO, p);
        }
    }
}