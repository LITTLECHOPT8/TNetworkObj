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
    }
}