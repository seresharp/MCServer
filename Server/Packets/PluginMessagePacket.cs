namespace MCServer.Server.Packets;

public class PluginMessagePacket : ServerPacket
{
    public string Channel { get; init; }

    public byte[] Data { get; init; }

    public PluginMessagePacket(string channel, params byte[] data)
    {
        Id = 0x18;

        Channel = channel;
        Data = data;
    }

    public override byte[] Build()
    {
        AddString(Channel);
        AddBytes(Data);

        return base.Build();
    }
}
