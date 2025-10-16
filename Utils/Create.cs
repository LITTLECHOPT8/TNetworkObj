namespace NetworkObj.Utils;

public class Packet
{
    public static Writer Pack(Protocols type, Writer pack)
    {
        Writer packet = new Writer();
        packet.wuint((uint)(12 + pack.Length));
        packet.wuint((uint)type);
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