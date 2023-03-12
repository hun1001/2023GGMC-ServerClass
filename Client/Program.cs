using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

internal class Program
{
    private static void Main(string[] args)
    {
        using Socket clientSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), 5000);

        bool isError = false;

        try
        {
            clientSocket.Connect(endPoint);
        }
        catch
        {
            isError = true;
        }

        Console.WriteLine(isError ? "Connect Fail" : "Connect Succes");

        while (true)
        {
            string str = Console.ReadLine();

            if (str == "/exit")
            {
                return;
            }

            byte[] buffer = Encoding.UTF8.GetBytes(str);
            clientSocket.Send(buffer);

            var buffer2 = new byte[256];
            int bytesRead = clientSocket.Receive(buffer2);

            if(bytesRead < 1)
            {
                Console.WriteLine("Server Connect End");
                return;
            }

            string str2 = Encoding.UTF8.GetString(buffer2);
            Console.WriteLine($"R: {str2}");
        }
    }
}