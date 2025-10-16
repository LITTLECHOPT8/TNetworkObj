using System.Net;
using System.Net.Sockets;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    class Listener
    {
        public async Task Start(IPAddress IP, int Port)
        {
            TcpListener listener = new TcpListener(IP, Port);
            listener.Start();

            Console.WriteLine($"Server started at {IP}:{Port}");

            while (true)
            {
                var clientReq = listener.AcceptTcpClientAsync();
                var ready = await Task.WhenAny(clientReq, Task.Delay(100));

                if (clientReq == ready)
                {
                    TcpClient client = clientReq.Result;

                    Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await new Responder().Respond(client);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error with client {client.Client.RemoteEndPoint}: {ex.Message}");
                        }
                    });
                }
            }
        }
    }
}