using System.Net.Sockets;
using System.Reflection.Metadata;
using NetworkObj;

namespace NetworkObj.TCP
{
    public class Clients
    {
        private readonly Dictionary<TcpClient, User> Users = new Dictionary<TcpClient, User>();

        public void AddClient(TcpClient client)
        {
            int userId = new Random().Next(1, int.MaxValue);
            User user = new User();
            user.UserId = userId;
            Users.Add(client, user);
        }

        public User GetUser(TcpClient client)
        {
            if (Users.TryGetValue(client, out User user))
            {
                return user;
            }

            return null;
        }
<<<<<<< Updated upstream
=======

        public static async Task SendToClient(TcpClient Client, Writer Packet)
        {
            try
            {
                var Stream = Client.GetStream();
                if (Stream == null || !Stream.CanWrite)
                {
                    Logger.Warning($"Client {Client.Client.RemoteEndPoint} disconnected during write");
                    //DisconnectClient(Client);
                    return;
                }

                byte[] Data = Packet.array();
                if (Data.Length < 12)
                {
                    Logger.Error($"Packet too short ({Data.Length}/12 bytes)");
                    return;
                }

                await Stream.WriteAsync(Data, 0, Data.Length);
            }
            catch (Exception ex)
            {
                Logger.Error($"Client exception ({Client.Client.RemoteEndPoint}): {ex.Message}");
                //DisconnectClient(Client);
            }
        }
>>>>>>> Stashed changes
    }
}