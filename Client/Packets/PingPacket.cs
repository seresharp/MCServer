namespace MCServer.Client.Packets;

public class PingPacket : ClientPacket
{
    public long Payload { get; private set; }

    public override void ReadData(int id, byte[] data)
    {
        Id = id;

        int i = 0;
        Payload = ReadInt64(data, ref i);
    }
}
