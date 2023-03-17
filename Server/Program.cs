namespace Server;

internal class Program
{
    const string ip = "127.0.0.1";

    static async Task Main(string[] args)
    {
        Server server = new Server(ip, 20000, 10);
        await server.StartAsync();
    }
}