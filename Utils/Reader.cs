using System.Net.Sockets;
using System.Text;

namespace NetworkObj.Utils
{
    public class Reader
    {
        private readonly byte[] buffer;
        private readonly int length;
        private int position;

        public int Position => position;
        public int Length => length;

        public byte rb => rbyte();
        public ushort r16 => rushort();
        public uint r32 => ruint();
        public ulong r64 => rulong();
        public string rs => rstring();

        public Reader(byte[] source)
        {
            buffer = source ?? throw new ArgumentNullException(nameof(source));
            length = source.Length;
            position = 0;
        }

        public byte rbyte()
        {
            if (position + 1 > length)
                throw new InvalidOperationException("Not enough data to read Byte");

            return buffer[position++];
        }

        public ushort rushort()
        {
            if (position + 2 > length)
                throw new InvalidOperationException("Not enough data to read UInt16");

            ushort value = (ushort)((buffer[position] << 8) | buffer[position + 1]);
            position += 2;
            return value;
        }

        public uint ruint()
        {
            if (position + 4 > length)
                throw new InvalidOperationException("Not enough data to read UInt32");

            uint value = (uint)((buffer[position] << 24) |
                                (buffer[position + 1] << 16) |
                                (buffer[position + 2] << 8) |
                                buffer[position + 3]);
            position += 4;
            return value;
        }

        public ulong rulong()
        {
            uint high = ruint();
            uint low = ruint();
            return ((ulong)high << 32) | low;
        }

        public uint ptype()
        {
            ruint();
            uint packettype = ruint();
            ruint();
            return packettype;
        }

        public byte[] rbytes(int count)
        {
            if (position + count > length)
                throw new InvalidOperationException("Not enough data to read byte array");

            byte[] result = new byte[count];
            Array.Copy(buffer, position, result, 0, count);
            position += count;
            return result;
        }

        public string rstring()
        {
            uint strLength = ruint();
            if (position + strLength > length)
                throw new InvalidOperationException("Not enough data to read string");

            string result = Encoding.ASCII.GetString(buffer, position, (int)strLength);
            position += (int)strLength;
            return result;
        }

        public bool bytesleft(int count)
        {
            return position + count <= length;
        }

        public bool parse(byte[] Packet) // Shitty way to do it but idrc
        {
            Reader packet = new Reader(Packet);

            if (Packet.Length < 12) return false;

            uint PacketLength = packet.ruint();
            uint PacketType = packet.ruint();
            uint PacketVersion = packet.ruint();

            if (PacketVersion != 1u) return false;

            return true;
        }

        /* public void SkipInfo()
        {
            ReadUInt32();
            ReadUInt32();
            ReadUInt32();
        } */

        public static async Task<bool> rexact(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    return false;
                }

                totalRead += read;
            }
            return true;
        }

        public void Reset()
        {
            position = 0;
        }
    }
}