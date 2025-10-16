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
<<<<<<< Updated upstream
=======
            listener.Start();

            Logger.Info($"Server started at {IP}:{Port}");
>>>>>>> Stashed changes

            while (true)
            {
                var clientReq = listener.AcceptTcpClientAsync();
                var ready = await Task.WhenAny(clientReq, Task.Delay(100));

                if (clientReq == ready)
                {
                    TcpClient client = clientReq.Result;
                    clientHandler.AddClient(client);

<<<<<<< Updated upstream
                    if (clientHandler.GetUser(client) == null) client.Close();

                    
=======
                    Logger.Info($"Client {client.Client.RemoteEndPoint} connected");

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await new Responder().Respond(client);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Error with client {client.Client.RemoteEndPoint}: {ex.Message}");
                        }
                    });
>>>>>>> Stashed changes
                }
            }
        }
    }
}