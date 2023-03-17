using System.Net;
using System.Text;

namespace Core;

public class UserEnterPacket : IPacket
{
    public string Nickname { get; private set; }

    public UserEnterPacket(string nickname)
    {
        Nickname = nickname;
    }

    public UserEnterPacket(byte[] buffer)
    {
        int offset = 2;

        short idSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, offset));
        offset += sizeof(short);

        Nickname = Encoding.UTF8.GetString(buffer, offset, idSize);
    }

    public byte[] Serialize()
    {
        byte[] packetType = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)PacketType.UserEnter));
        byte[] nickname = Encoding.UTF8.GetBytes(Nickname);
        byte[] nicknameSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)nickname.Length));

        short dataSize = (short)(packetType.Length + nickname.Length + nicknameSize.Length);
        byte[] header = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(dataSize));

        byte[] buffer = new byte[2 + dataSize];

        int offset = 0;

        Array.Copy(header, 0, buffer, offset, header.Length);
        offset += header.Length;

        Array.Copy(packetType, 0, buffer, offset, packetType.Length);
        offset += packetType.Length;

        Array.Copy(nicknameSize, 0, buffer, offset, nicknameSize.Length);
        offset += nicknameSize.Length;

        Array.Copy(nickname, 0, buffer, offset, nickname.Length);

        return buffer;
    }
}
