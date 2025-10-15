using System.Net.Sockets;

namespace NetworkObj
{
    public class User
    {
        public int UserId;
        public int RoomId;
        public bool RoomMaster;
        public int Avatar;
        public int Level;
        // private int Kills; (survival)
    }

    public class Room
    {
        //public int RoomId;
        public int Online;
        public string Password = string.Empty;
        public int Max = 4; // TODO: change to 3 if client is in survival (detect using 1u packet)
        public List<TcpClient> Players = new List<TcpClient>(4);
    }
}