namespace MCServer.Client.Packets;

public class HandshakePacket : ClientPacket
{
    public int ProtocolVersion { get; private set; }

    public string Address { get; private set; } = null!;

    public ushort Port { get; private set; }

    public int NextState { get; private set; }

    public override void ReadData(int id, byte[] data)
    {
        Id = id;

        int pos = 0;
        ProtocolVersion = ReadVarInt(data, ref pos);
        Address = ReadString(data, ref pos);
        Port = ReadUInt16(data, ref pos);
        NextState = ReadVarInt(data, ref pos);
    }
}
