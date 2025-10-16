using System.Net.Sockets;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    public static class Rooms
    {
        private static readonly Dictionary<int, Room> rooms = new Dictionary<int, Room>();

        public static int CreateRoom(TcpClient client, string Password)
        {
            User user = Clients.GetUser(client);

            if (user == null)
            {
                Logger.Info("Disconnect client, no info was found");
                return -1;
            }

            int roomId = new Random().Next(1, 9999);
            if (rooms.TryGetValue(roomId, out Room _))
            {
                Logger.Log("Already existing room tried to get created, recreating...");
                //CreateRoom(client, Password);
                return -2;
            }

            Room room = new Room();
            //room.RoomId = roomId;
            room.Players.Add(client);
            room.Online = 1;
            room.Max = 4;
            room.Password = Password;

            rooms.Add(roomId, room);

            string logHelp = (Password != String.Empty) ? $"password {Password}" : "no password";
            Logger.Log($"Room {roomId} was created with {logHelp}");

            return roomId;
        }

        public static async Task SendToRoom(int roomId, Writer packet, TcpClient? except = null)
        {
            if (!rooms.TryGetValue(roomId, out Room room)) return;

            byte[] data = packet.array();

            List<Task> sendTasks = new List<Task>();

            foreach (var client in room.Players)
            {
                if (client == null || !client.Connected || client == except)
                    continue;

                var stream = client.GetStream();
                if (stream != null && stream.CanWrite)
                {
                    sendTasks.Add(stream.WriteAsync(data, 0, data.Length));
                }
            }

            await Task.WhenAll(sendTasks);
        }
    }
}