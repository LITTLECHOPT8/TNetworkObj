using System.Net.Sockets;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    public class Rooms
    {
        private readonly Dictionary<int, Room> rooms = new Dictionary<int, Room>();
        Clients clients = new Clients();

        public int CreateRoom(TcpClient client, string Password)
        {
            User user = clients.GetUser(client);

            if (user == null)
            {
                new Log("Disconnect client.");
                return -1;
            }

            int roomId = new Random().Next(1, 9999);
            if (rooms.TryGetValue(roomId, out Room _))
            {
                new Log("Already existing room tried to get created, recreating...");
                //CreateRoom(client, Password);
                return -2;
            }

            Room room = new Room();
            //room.RoomId = roomId;
            room.Players.Add(client);
            room.Online = 1;
            room.Max = 4;
            room.Password = Password;

            return roomId;
        }
    }
}