using System.Net.Sockets;
using NetworkObj.Packets;
using NetworkObj.Utils;

namespace NetworkObj.TCP;

class Responder
{
    private Reader rpacket = new Reader(new byte[0]);
    private TcpClient client = new TcpClient();

    public async Task Respond(TcpClient cli)
    {
        client = cli;
        Clients.AddClient(client);

        NetworkStream stream = client.GetStream();
        byte[] lengthBuffer = new byte[4];

        while (true)
        {
            if (!await Reader.rexact(stream, lengthBuffer, 0, 4)) return;

            Reader length = new Reader(lengthBuffer);
            uint len = length.ruint();

            if (len < 0 || len > 256) return; // i dont believe triniti usually sends packets even over 100 bytes

            byte[] buffer = new byte[len];
            Array.Copy(lengthBuffer, 0, buffer, 0, 4);

            int tr = (int)len - 4;
            if (tr > 0)
            {
                if (!await Reader.rexact(stream, buffer, 4, tr)) return;
            }

            if (!new Reader(buffer).parse(buffer)) Console.WriteLine("Packet couldn't be parsed!");

            rpacket = new Reader(buffer);
            uint packetType = rpacket.ptype();

            if (Clients.GetUser(client) == null)
            {
                Console.WriteLine($"Breaking {client.Client.RemoteEndPoint}'s connection.");
                client.Close();
                break;
            }

            switch (packetType)
            {
                case Protocols.CG_HEARTBEAT:
                    await Heartbeat();
                    break;
                case Protocols.CG_CREATE_ROOM:
                    await CreateRoom();
                    break;
                case Protocols.CG_START_GAME:
                    await DefaultPacket(Protocols.GC_START_GAME);
                    Console.WriteLine("Test");
                    break;
                default:
                    break;
            }
        }
    }

    async Task Heartbeat()
    {
        GHeartbeat p = new GHeartbeat();
        p.m_lLocalTime = rpacket.rulong();

        Writer pack = p.Pack();

        await Clients.SendToClient(client, pack);
    }

    async Task CreateRoom()
    {
        GCreateRoom p = new GCreateRoom();
        uint mapId = rpacket.ruint();
        ulong localtime = rpacket.rulong();
        string nickname = rpacket.rstring();
        uint avatar = rpacket.ruint();
        uint days = rpacket.ruint();
        string password = rpacket.rstring();

        User host = Clients.GetUser(client);
        host.RoomMaster = true;
        host.Name = nickname;
        host.Avatar = (int)avatar;
        host.Level = (int)days;

        int RoomId = Rooms.CreateRoom(client, password);

        host.RoomId = RoomId;
        p.m_iResult = 0u;
        p.m_iUserId = (uint)host.UserId;
        p.m_iRoomId = (uint)RoomId;
        p.m_lLocalTime = (long)localtime;
        p.m_lServerTime = (long)localtime;

        if (RoomId == -1 || RoomId == -2)
        {
            p.m_iResult = 1u;
            host.RoomMaster = false;
            host.RoomId = -1;
        }

        await Clients.SendToClient(client, p.Pack());
    }

    async Task DefaultPacket(uint packetType)
    {
        Writer packet = new Writer();

        packet.wuint(12u);
        packet.wuint(packetType);
        packet.wuint(1u);

        int roomId = Clients.GetUser(client).RoomId;
        Console.WriteLine(roomId);

        await Rooms.SendToRoom(roomId, packet);
    }
}