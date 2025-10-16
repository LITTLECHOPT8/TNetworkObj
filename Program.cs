using System.Net;
using System.Threading.Tasks;
using NetworkObj.TCP;

namespace NetworkObj
{
    class Program
    {
        static async Task Main()
        {
            Listener Server = new Listener();
            IPAddress IP = IPAddress.Parse("127.0.0.1");
            int Port = 4201;

            await Server.Start(IP, Port);

            Console.ReadLine();
        }
    }
}