using System.Net;
using System.Net.Sockets;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    class Listener
    {
        Clients clientHandler = new Clients();

        public async Task Main(IPAddress IP, int Port)
        {
            TcpListener listener = new TcpListener(IP, Port);

            while (true)
            {
                var clientReq = listener.AcceptTcpClientAsync();
                var ready = await Task.WhenAny(clientReq, Task.Delay(100));

                if (clientReq == ready)
                {
                    TcpClient client = clientReq.Result;
                    clientHandler.AddClient(client);

                    if (clientHandler.GetUser(client) == null) client.Close();

                    
                }
            }
        }
    }
}