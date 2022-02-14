namespace MCServer.Server.Packets;

public class PongPacket : ServerPacket
{
    public long Payload { get; init; }

    public PongPacket(int id, long payload)
    {
        Id = id;

        Payload = payload;
    }

    public override byte[] Build()
    {
        AddInt64(Payload);
        return base.Build();
    }
}
