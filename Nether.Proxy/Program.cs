using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Nether.Proxy
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //TCP downstream dest port
            const int downStreamPort = 25565;
            //TCP listener port
            const int proxyPort = 4441;
            var listener = new TcpListener(IPAddress.Loopback, proxyPort);
            listener.Start();

            while (true)
            {
                var socket = await listener.AcceptTcpClientAsync();
                var clientStream = socket.GetStream();
                var client = new TcpClient();
                await client.ConnectAsync(IPAddress.Loopback, downStreamPort);
                var downstream = client.GetStream();

                HandleTcpConnection();

                void HandleTcpConnection()
                {
                    Console.WriteLine($"New client connected {socket.Client.RemoteEndPoint}");
                    _ = Task.Run(async () => { await clientStream.CopyToAsync(downstream); });
                    _ = Task.Run(async () => { await downstream.CopyToAsync(clientStream); });
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }
    }
}