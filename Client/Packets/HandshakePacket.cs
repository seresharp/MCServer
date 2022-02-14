namespace MCServer.Client.Packets;

public class HandshakePacket : ClientPacket
{
    public int ProtocolVersion { get; init; }

    public string Address { get; init; }

    public ushort Port { get; init; }

    public int NextState { get; init; }

    public HandshakePacket(byte[] data)
    {
        int pos = 0;
        ProtocolVersion = ReadVarInt(data, ref pos);
        Address = ReadString(data, ref pos);
        Port = ReadUInt16(data, ref pos);
        NextState = ReadVarInt(data, ref pos);
    }
}
