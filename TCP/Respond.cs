using NetworkObj.Packets;
using NetworkObj.Utils;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Reflection.Metadata;

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

            if (!new Reader(buffer).parse(buffer)) Logger.Error("Packet parser failed");

            rpacket = new Reader(buffer);
            uint packetType = rpacket.ptype();

            if (Clients.GetUser(client) == null)
            {
                Logger.Info($"Breaking {client.Client.RemoteEndPoint}'s connection (No user connected to client)");
                client.Close();
                break;
            }

            switch ((Protocols)packetType)
            {
                case Protocols.CG_HEARTBEAT:
                    await Heartbeat();
                    break;
                case Protocols.CG_CREATE_ROOM:
                    await CreateRoom();
                    break;
                case Protocols.CG_START_GAME:
                    await StartRoom();
                    break;
                case Protocols.CG_DESTROY_ROOM:
                    await DestroyRoom();
                    break;
                case Protocols.CG_LEAVE_ROOM:
                    await LeaveRoom();
                    break;
                case Protocols.CG_ROOM_INFO:
                    await RoomInfo();
                    break;
                case Protocols.CG_JOIN_ROOM:
                    await JoinRoom();
                    break;
                case Protocols.CG_USER_SPAWN:
                    await PlayerSpawn();
                    break;
                default:
                    Logger.Error($"{Enum.GetName(typeof(Protocols), (Protocols)packetType)} unimplemented");
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
        Rooms.GetRoom(RoomId).MapId = (int)mapId;

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

    async Task StartRoom()
    {
        User host = Clients.GetUser(client);
        if (host.RoomId == -1 || !host.RoomMaster) return;

        Logger.Info($"Room {host.RoomId} started");

        Logger.Info($"MAP ID: {Rooms.GetRoom(host.RoomId)?.MapId}");

        await Rooms.SendToRoom(host.RoomId, DefaultPacket(Protocols.GC_START_GAME_NOTIFY));
        await Rooms.SendToRoom(host.RoomId, DefaultPacket(Protocols.GC_START_GAME));
    }

    async Task DestroyRoom()
    {
        User host = Clients.GetUser(client);

        if (host.RoomId == -1 || !host.RoomMaster) return;

        Logger.Info($"Room {host.RoomId} destroyed");

        await Rooms.SendToRoom(host.RoomId, DefaultPacket(Protocols.GC_DESTROY_ROOM, true));
        await Rooms.DeleteRoom(host.RoomId);
    }

    async Task LeaveRoom()
    {
        User user = Clients.GetUser(client);
        if (user.RoomId == -1) return;
        if (user.RoomMaster) { await DestroyRoom(); return; }

        GLeaveRoom notify = new GLeaveRoom();
        notify.m_iUserId = (uint)user.UserId;

        await Rooms.SendToRoom(user.RoomId, notify.Pack());
        await Rooms.LeaveRoom(user.RoomId, client);
        await Clients.SendToClient(client, DefaultPacket(Protocols.GC_LEAVE_ROOM, true));
    }

    async Task RoomInfo()
    {
        uint roomId = rpacket.ruint();

        Room? room = Rooms.GetRoom((int)roomId);

        if (room == null) return;

        Logger.Log($"hi{(int)roomId}");

        User creator = Clients.GetUser(room.Players[0]);

        Logger.Log("hi1");

        GRoomInfo p = new GRoomInfo();

        Logger.Log("hi2");

        p.m_iResult = 0u;
        p.m_iMapId = (uint)room.MapId;
        p.m_room_status = 0u;
        p.m_password = room.Password;
        p.m_strCreaterNickname = creator.Name;
        p.m_Creater_level = (uint)creator.Level;
        p.m_iOnlineNum = (uint)room.Online;
        p.m_iMaxUserNum = (uint)room.Max;
        p.m_iRoomId = roomId;

        Logger.Log("hi3");

        await Clients.SendToClient(client, p.Pack());
    }

    async Task JoinRoom()
    {
        uint roomId = rpacket.ruint();
        ulong localTime = rpacket.rulong();
        string name = rpacket.rstring();
        uint avt = rpacket.ruint();
        uint days = rpacket.ruint();

        Logger.Log("JJJJ");

        User user = Clients.GetUser(client);
        Room? room = Rooms.GetRoom((int)roomId);

        GJoinRoom p = new GJoinRoom();
        if (room == null)
        {
            p.m_iResult = 2u;
            p.m_iRoomId = 0u;
        }
        else if (room.Online == 4)
        {
            p.m_iResult = 1u;
            p.m_iRoomId = 0u;
        }
        else
        {
            p.m_iResult = 0u;
            p.m_iRoomId = roomId;
        }

        p.m_map_id = (uint)room.MapId;
        p.m_lLocalTime = (long)localTime;
        p.m_lServerTime = (long)localTime;
        p.m_room_index = (uint)room.Online;
        p.m_iUserId = (uint)user.UserId;

        user.RoomId = (int)roomId;
        user.Index = room.Online;
        user.Name = name;
        user.Avatar = (int)avt;
        user.Level = (int)days;
        user.RoomMaster = false;

        room.Players.Add(client);

        room.Players.ForEach(async (TcpClient rando) => {
            if (!(client == rando))
            {
                User ruser = Clients.GetUser(rando);
                GJoinRoomNotify notify2 = new GJoinRoomNotify();
                notify2.m_room_index = (uint)ruser.Index;
                notify2.m_strNickname = ruser.Name;
                notify2.m_iLevel = (uint)ruser.Level;
                notify2.m_iAvatarType = (uint)ruser.Avatar;
                notify2.m_iUserId = (uint)ruser.UserId;
                await Clients.SendToClient(client, notify2.Pack());
            }
        });

        GJoinRoomNotify notify = new GJoinRoomNotify();

        notify.m_room_index = (uint)user.Index;
        notify.m_strNickname = user.Name;
        notify.m_iLevel = (uint)user.Level;
        notify.m_iAvatarType = (uint)user.Avatar;
        notify.m_iUserId = (uint)user.UserId;

        await Clients.SendToClient(client, p.Pack());
        await Rooms.SendToRoom((int)roomId, notify.Pack(), client);

        Logger.Info($"User {user.UserId} join Room {roomId}");
    }

    async Task PlayerSpawn()
    {
        ulong localTime = rpacket.rulong();
        uint bpIndex = rpacket.ruint();
        uint wp1 = rpacket.ruint();
        uint wp2 = rpacket.ruint();
        uint wp3 = rpacket.ruint();

        User user = Clients.GetUser(client);

        GPlayerSpawn p = new GPlayerSpawn();

        p.m_lLocalTime = (long)localTime;
        p.m_lServerTime = (long)localTime;
        p.m_iUserId = (uint)user.UserId;
        p.m_iBirthPointIndex = (uint)user.Index;
        p.m_iWeaponIndex1 = wp1;
        p.m_iWeaponIndex2 = wp2;
        p.m_iWeaponIndex3 = wp3;

        await Rooms.SendToRoom(user.RoomId, p.Pack(), client);
    }

    Writer DefaultPacket(Protocols packetType, bool result = false)
    {
        Writer packet = new Writer();

        if (result) packet.wuint(16u);
        if (!result) packet.wuint(12u);

        packet.wuint((uint)packetType);
        packet.wuint(1u);

        if (result) packet.wuint(0u);

        return packet;
    }
}