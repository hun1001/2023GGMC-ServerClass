using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

internal abstract class Program
{
    private static async Task Main()
    {
        using var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        serverSocket.Bind(serverEndPoint);
        serverSocket.Listen(10);
        Console.WriteLine("Server is listening on port 8080");
        while (true)
        {
            var clientSocket = await serverSocket.AcceptAsync();
            Console.WriteLine("Client connected");
            ThreadPool.QueueUserWorkItem(_ => ReadAsync(clientSocket));

            if (clientSocket.RemoteEndPoint is IPEndPoint remoteIpEndPoint)
            {
                Console.WriteLine($"Client IP: {remoteIpEndPoint.Address}, port: {remoteIpEndPoint.Port}");
            }
        }
    }

    private static async void ReadAsync(Socket clientSocket)
    {
        while (true)
        {
            var buffer = new byte[1024];
            var receivedBytes = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            var data = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Received: {data}");
            clientSocket.Send(buffer, 0, receivedBytes, SocketFlags.None);
            Console.WriteLine($"Sent: {data}");
        }
    }
}