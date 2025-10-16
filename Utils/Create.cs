namespace NetworkObj.Utils;

public class Packet
{
    public static Writer Pack(uint type, Writer pack)
    {
        Writer packet = new Writer();
        packet.wuint((uint)(12 + pack.Length));
        Console.WriteLine((uint)(12 + pack.Length));
        packet.wuint(type);
        packet.wuint(1u);
        packet.wbytes(pack.array());
        return packet;
    }
}

public interface ServerPacket
{
    Writer Pack();
}

public interface ClientPacket
{
    bool parse(Reader writer);
}