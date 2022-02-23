namespace MCServer.Client.Packets;

public class StatusRequestPacket : ClientPacket
{
    public override void ReadData(int id, byte[] data)
    {
        Id = id;
        // packet is empty
    }
}
