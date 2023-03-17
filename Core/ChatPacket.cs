using System.Net;
using System.Text;

namespace Core;

public class ChatPacket : IPacket
{
    public string Nickname { get; private set; }
    public string Message { get; private set; }

    public ChatPacket(string nickname, string message)
    {
        Message = message;
        Nickname = nickname;
    }

    public ChatPacket(byte[] buffer)
    {
        int offset = 2;

        short idSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        offset += sizeof(short);

        Message = Encoding.UTF8.GetString(buffer, offset, idSize);
        offset += idSize;

        short nicknameSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        offset += sizeof(short);

        Nickname = Encoding.UTF8.GetString(buffer, offset, nicknameSize);
    }

    public byte[] Serialize()
    {
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.Chat));
        byte[] message = Encoding.UTF8.GetBytes(Message);
        byte[] messageSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)message.Length));
        byte[] nickname = Encoding.UTF8.GetBytes(Nickname);
        byte[] nicknameSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)nickname.Length));

        short dataSize = (short)(packetType.Length + message.Length + messageSize.Length + nickname.Length + nicknameSize.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        byte[] buffer = new byte[2 + dataSize];

        int offset = 0;

        Array.Copy(header, 0, buffer, offset, header.Length);
        offset += header.Length;

        Array.Copy(packetType, 0, buffer, offset, packetType.Length);
        offset += packetType.Length;

        Array.Copy(messageSize, 0, buffer, offset, messageSize.Length);
        offset += messageSize.Length;

        Array.Copy(message, 0, buffer, offset, message.Length);
        offset += message.Length;

        Array.Copy(nicknameSize, 0, buffer, offset, nicknameSize.Length);
        offset += nicknameSize.Length;

        Array.Copy(nickname, 0, buffer, offset, nickname.Length);

        return buffer;
    }
}
