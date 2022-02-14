namespace MCServer.Server.Packets;

public class LoginSuccessPacket : ServerPacket
{
    public string Uuid { get; init; }

    public string Username { get; init; }

    public LoginSuccessPacket(string uuid, string username)
    {
        Id = 0x02;

        Uuid = uuid;
        Username = username;
    }

    public override byte[] Build()
    {
        AddBytes(GuidHelper.FromStringHash(Uuid).ToByteArray());
        AddString(Username);

        return base.Build();
    }
}
