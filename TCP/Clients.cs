using System.Net.Sockets;
using System.Reflection.Metadata;
using NetworkObj;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    public static class Clients
    {
        private static readonly Dictionary<TcpClient, User> Users = new Dictionary<TcpClient, User>();

        public static void AddClient(TcpClient client)
        {
            int userId = new Random().Next(1, int.MaxValue);
            User user = new User();
            user.UserId = userId;
            Users.Add(client, user);
        }

        public static User GetUser(TcpClient client)
        {
            if (Users.TryGetValue(client, out User user))
            {
                return user;
            }

            return null;
        }

        public static async Task SendToClient(TcpClient Client, Writer Packet)
        {
            try
            {
                var Stream = Client.GetStream();
                if (Stream == null || !Stream.CanWrite)
                {
                    Console.WriteLine("Client disconnected during write, cleaning up.");
                    //DisconnectClient(Client);
                    return;
                }

                byte[] Data = Packet.array();
                if (Data.Length < 12)
                {
                    Console.WriteLine("Invalid packet (too small).");
                    return;
                }

                await Stream.WriteAsync(Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parsing error: {ex.Message}, disconnecting client.");
                //DisconnectClient(Client);
            }
        }
    }
}