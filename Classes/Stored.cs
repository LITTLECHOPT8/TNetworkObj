using System.Net.Sockets;

namespace NetworkObj
{
    public class User
    {
        public int UserId = -1;
        public int RoomId = -1;
        public bool RoomMaster = false;
        public int Avatar = 1;
        public int Level = 1;
        public string Name = "Player";
        public int Index = 0;
        // private int Kills; (survival)
    }

    public class Room
    {
        public int MapId = -1;
        public int Online = 1;
        public string Password = string.Empty;
        public int Max = 4; // TODO: change to 3 if client is in survival (detect using 1u packet)
        public List<TcpClient> Players = new List<TcpClient>(4);
    }
}