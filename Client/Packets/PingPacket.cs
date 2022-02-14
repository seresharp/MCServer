namespace MCServer.Client.Packets;

public class PingPacket : ClientPacket
{
    public int Id { get; init; }

    public long Payload { get; init; }

    public PingPacket(int id, byte[] data)
    {
        Id = id;

        int i = 0;
        Payload = ReadInt64(data, ref i);
    }
}
