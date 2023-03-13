using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

internal abstract class Program
{
    private static async Task Main()
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);

        await clientSocket.ConnectAsync(clientEndPoint);

        while (true)
        {
            var data = Console.ReadLine();
            var buffer = Encoding.UTF8.GetBytes(data);
            await clientSocket.SendAsync(buffer, SocketFlags.None);
            Console.WriteLine($"Sent: {data}");
            var receivedBytes = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            data = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
            Console.WriteLine($"Received: {data}");
        }
    }
}