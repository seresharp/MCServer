using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MCServer.Client.Packets;

public abstract class ClientPacket
{
    public static int GetPacketLength(byte[] buffer, int pos)
        => ReadVarInt(buffer, ref pos) + pos;

    public static bool TryParse(byte[] buffer, int clientState, [NotNullWhen(true)] out ClientPacket? packet, out string error)
    {
        try
        {
            int pos = 0;
            int length = ReadVarInt(buffer, ref pos);

            int temp = pos;
            int id = ReadVarInt(buffer, ref temp);
            length -= temp - pos;
            pos = temp;

            byte[] data = buffer[pos..(pos + length)];

            switch ((PacketType)id)
            {
                case PacketType.Handshake when clientState == 0:
                    packet = new HandshakePacket(data);
                    break;
                case PacketType.Handshake when clientState == 1:
                    packet = new StatusRequestPacket();
                    break;
                case PacketType.Handshake when clientState == 2:
                    packet = new LoginStartPacket(data);
                    break;
                case PacketType.Ping when clientState == 1:
                case PacketType.KeepAlive when clientState == 3:
                    packet = new PingPacket(id, data);
                    break;
                default:
                    packet = null;
                    error = "Invalid/unimplemented packet: " + id.ToString("X") + ", state:" + clientState;
                    error += "\n" + BitConverter.ToString(buffer).Replace('-', ' ');
                    return false;
            }

            error = string.Empty;
            return true;
        }
        catch (Exception e)
        {
            packet = null;
            error = e.GetType().Name + ": " + e.Message;
            return false;
        }
    }

    protected static int ReadVarInt(byte[] buffer, ref int pos)
    {
        int value = 0;
        int length = 0;
        while (true)
        {
            byte b = buffer[pos++];
            value |= (b & 0x7F) << (length * 7);

            if (++length > 5)
            {
                throw new InvalidDataException("VarInt cannot contain more than 5 bytes");
            }

            if ((b & 0x80) != 0x80)
            {
                break;
            }
        }

        return value;
    }

    protected static string ReadString(byte[] buffer, ref int pos)
    {
        int len = ReadVarInt(buffer, ref pos);
        string str = Encoding.ASCII.GetString(buffer, pos, len);
        pos += len;

        return str;
    }

    protected static ushort ReadUInt16(byte[] buffer, ref int pos)
    {
        ushort us = (ushort)(BitConverter.IsLittleEndian
            ? buffer[pos] << 8 | buffer[pos + 1]
            : buffer[pos] | buffer[pos + 1] << 8);

        pos += 2;

        return us;
    }

    protected static long ReadInt64(byte[] buffer, ref int pos)
    {
        byte[] bytes = buffer[pos..(pos + 8)];
        FixEndianness(bytes);

        pos += 8;
        return BitConverter.ToInt64(bytes, 0);
    }

    private static void FixEndianness(byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
        {
            return;
        }

        Array.Reverse(bytes);
    }
}

public enum PacketType
{
    Handshake = 0x00,
    Ping = 0x01,
    KeepAlive = 0x0F
}
