using Core;
using System.Net;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// 언제 어디서나 접근할 수 있는 객체.
/// 이 객체는 반드시 1개만 존재해야 한다.
/// </summary>
internal class Singleton
{
    public string Id { get; set; } = null!;
    public string Nickname { get; set; } = null!;
    public Socket Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public event EventHandler<EventArgs>? LoginResponsed;
    public event EventHandler<EventArgs>? CreateRoomResponsed;
    public event EventHandler<EventArgs>? RoomListResponsed;
    public event EventHandler<EventArgs>? EnterRoomResponsed;
    public event EventHandler<EventArgs>? UserEnterResponsed;
    public event EventHandler<EventArgs>? UserLeaveResponsed;
    public event EventHandler<EventArgs>? ChatResponsed;

    private static Singleton? instance;
    public static Singleton Instance
    {
        get
        {
            if (instance == null)
                instance = new Singleton();
            return instance;
        }
    }

    private Singleton()
    {

    }

    public async Task ConnectAsync()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.200.120"), 20000);
        await Socket.ConnectAsync(endPoint);
        ThreadPool.QueueUserWorkItem(ReceiveAsync, Socket);
    }

    private async void ReceiveAsync(object? sender)
    {
        Socket socket = (Socket)sender!;
        byte[] headerBuffer = new byte[2];

        System.Timers.Timer timer= new System.Timers.Timer(5000);
        timer.Elapsed += async (s, e) =>
        {
            HeartbeatPacket packet = new HeartbeatPacket();
            await socket.SendAsync(packet.Serialize(), SocketFlags.None);
        };

        try
        {
            while (true)
            {
                #region 헤더버퍼 가져옮
                int n1 = await socket.ReceiveAsync(headerBuffer, SocketFlags.None);
                if (n1 < 1)
                {
                    Console.WriteLine("server disconnect");
                    timer.Dispose();
                    socket.Dispose();
                    return;
                }
                else if (n1 == 1)
                {
                    await socket.ReceiveAsync(new ArraySegment<byte>(headerBuffer, 1, 1), SocketFlags.None);
                }
                #endregion

                #region 데이터버퍼 가져옮
                short dataSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerBuffer));
                byte[] dataBuffer = new byte[dataSize];

                int totalRecv = 0;
                while (totalRecv < dataSize)
                {
                    int n2 = await socket.ReceiveAsync(new ArraySegment<byte>(dataBuffer, totalRecv, dataSize - totalRecv), SocketFlags.None);
                    totalRecv += n2;
                }
                #endregion

                PacketType packetType = (PacketType)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataBuffer));
                if (packetType == PacketType.LoginResponse)
                {
                    LoginResponsePacket packet = new LoginResponsePacket(dataBuffer);
                    LoginResponsed?.Invoke(packet, EventArgs.Empty);
                    timer.Start();
                }
                else if (packetType == PacketType.CreateRoomResponse)
                {
                    CreateRoomResponsePacket packet = new CreateRoomResponsePacket(dataBuffer);
                    CreateRoomResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.RoomListResponse)
                {
                    RoomListResponsePacket packet = new RoomListResponsePacket(dataBuffer);
                    RoomListResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.EnterRoomResponse)
                {
                    EnterRoomResponsePacket packet = new EnterRoomResponsePacket(dataBuffer);
                    EnterRoomResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.UserEnter)
                {
                    UserEnterPacket packet = new UserEnterPacket(dataBuffer);
                    UserEnterResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.UserLeave)
                {
                    UserLeavePacket packet = new UserLeavePacket(dataBuffer);
                    UserLeaveResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.Chat)
                {
                    ChatPacket packet = new ChatPacket(dataBuffer);
                    ChatResponsed?.Invoke(packet, EventArgs.Empty);
                }
                else if (packetType == PacketType.Duplicate)
                {
                    socket.Shutdown(SocketShutdown.Send);
                    MessageBox.Show("중복접속");
                    Environment.Exit(0);
                }
            }
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.ToString());
            Environment.Exit(1);
        }
    }
}
