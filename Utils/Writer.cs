using System.Text;

namespace NetworkObj.Utils
{
    public class Writer
    {
        private const int MaxSize = 2048;
        private byte[] buffer;
        private int position;

        public int Position => position;
        public byte[] Buffer => buffer;
        public int Length => position;

        public Writer()
        {
            buffer = new byte[0];
            position = 0;
        }

        public byte[] array()
        {
            byte[] result = new byte[position];
            Array.Copy(buffer, 0, result, 0, position);
            return result;
        }

        public bool wbyte(byte value)
        {
            Array.Resize(ref buffer, buffer.Length + 1);
            if (position + 1 > MaxSize) return false;
            buffer[position++] = value;
            return true;
        }

        public bool wstring(string value)
        {
            if (value == null) value = string.Empty;

            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            uint length = (uint)stringBytes.Length;
            Array.Resize(ref buffer, buffer.Length + stringBytes.Length);

            if (position + 2 + length > buffer.Length) return false;

            wuint(length);
            wbytes(stringBytes);

            return true;
        }

        public bool wushort(ushort value)
        {
            Array.Resize(ref buffer, buffer.Length + 2);
            if (position + 2 > MaxSize) return false;
            buffer[position++] = (byte)(value >> 8);
            buffer[position++] = (byte)value;
            return true;
        }

        public bool wuint(uint value)
        {
            Array.Resize(ref buffer, buffer.Length + 4);
            if (position + 4 > MaxSize) return false;
            buffer[position++] = (byte)(value >> 24);
            buffer[position++] = (byte)(value >> 16);
            buffer[position++] = (byte)(value >> 8);
            buffer[position++] = (byte)value;
            return true;
        }

        public bool wulong(ulong value)
        {
            Array.Resize(ref buffer, buffer.Length + 8);
            return wuint((uint)(value >> 32)) && wuint((uint)value);
        }

        public bool wbytes(byte[] data)
        {
            int count = data.Length;
            Array.Resize(ref buffer, buffer.Length + count);
            if (position + count > MaxSize) return false;
            Array.Copy(data, 0, buffer, position, count);
            position += count;
            return true;
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[position];
            Array.Copy(buffer, result, position);
            return result;
        }
    }
}