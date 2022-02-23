using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MCServer.Client.Packets;

public abstract class ClientPacket
{
    private static readonly IPacketReader[] _Readers;

    static ClientPacket()
    {
        _Readers = new IPacketReader[]
        {
            new PacketReader<HandshakePacket>(0x00, ClientState.Default),
            new PacketReader<StatusRequestPacket>(0x00, ClientState.Status),
            new PacketReader<LoginStartPacket>(0x00, ClientState.Login),
            new PacketReader<PingPacket>(0x01, ClientState.Status),
            new PacketReader<PingPacket>(0x0F, ClientState.Play),
            new PacketReader<PlayerPositionAndLookPacket>(0x11, ClientState.Play),
            new PacketReader<PlayerPositionAndLookPacket>(0x12, ClientState.Play),
            new PacketReader<PlayerPositionAndLookPacket>(0x13, ClientState.Play)
        };
    }

    public static bool TryParse(byte[] buffer, ClientState clientState, [NotNullWhen(true)] out ClientPacket? packet, out string error)
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

            foreach (IPacketReader reader in _Readers)
            {
                if (reader.TryReadPacket(id, clientState, data, out packet))
                {
                    error = string.Empty;
                    return true;
                }
            }

            packet = null;
            error = "Invalid/unimplemented packet: " + id.ToString("X") + ", state:" + clientState;
            error += "\n" + BitConverter.ToString(buffer).Replace('-', ' ');
            return false;
        }
        catch (Exception e)
        {
            packet = null;
            error = e.GetType().Name + ": " + e.Message;
            return false;
        }
    }

    public int Id { get; protected set; }

    public abstract void ReadData(int id, byte[] data);

    public static int GetPacketLength(byte[] buffer, int pos)
        => ReadVarInt(buffer, ref pos) + pos;

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

    protected static float ReadFloat(byte[] buffer, ref int pos)
    {
        byte[] bytes = buffer[pos..(pos + 4)];
        FixEndianness(bytes);

        pos += 4;
        return BitConverter.ToSingle(bytes, 0);
    }

    protected static double ReadDouble(byte[] buffer, ref int pos)
    {
        byte[] bytes = buffer[pos..(pos + 8)];
        FixEndianness(bytes);

        pos += 8;
        return BitConverter.ToDouble(bytes, 0);
    }

    protected static bool ReadBool(byte[] buffer, ref int pos)
        => buffer[pos++] != 0x00;

    private static void FixEndianness(byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
        {
            return;
        }

        Array.Reverse(bytes);
    }
}
