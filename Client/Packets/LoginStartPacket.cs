namespace MCServer.Client.Packets;

public class LoginStartPacket : ClientPacket
{
    public string Username { get; init; }

    public LoginStartPacket(byte[] data)
    {
        int pos = 0;
        Username = ReadString(data, ref pos);
    }
}
