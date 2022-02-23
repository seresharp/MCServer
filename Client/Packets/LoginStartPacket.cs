namespace MCServer.Client.Packets;

public class LoginStartPacket : ClientPacket
{
    public string Username { get; private set; } = null!;

    public override void ReadData(int id, byte[] data)
    {
        Id = id;

        int pos = 0;
        Username = ReadString(data, ref pos);
    }
}
