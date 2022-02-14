using Newtonsoft.Json;

namespace MCServer.Server.Packets;

public class StatusResponsePacket : ServerPacket
{
    public string MCVersion { get; init; }

    public int ProtocolVersion { get; init; }

    public int MaxPlayers { get; init; }

    public (string name, string id)[] Players { get; init; }

    public string Description { get; init; }

    public string Favicon { get; init; }

    public StatusResponsePacket(string mcVersion, int protocolVersion, string description, int maxPlayers,
        IEnumerable<(string, string)>? players = null, string? favicon = null)
    {
        Id = 0x00;

        MCVersion = mcVersion;
        ProtocolVersion = protocolVersion;
        Description = description;
        MaxPlayers = maxPlayers;
        Players = players?.ToArray() ?? Array.Empty<(string, string)>();
        Favicon = favicon ?? string.Empty;
    }

    public override byte[] Build()
    {
        var resp = new
        {
            version = new
            {
                name = MCVersion,
                protocol = ProtocolVersion
            },
            players = new
            {
                max = MaxPlayers,
                online = Players.Length,
                sample = Players.Select(p => new { p.name, p.id }).ToArray()
            },
            description = Description,
            favicon = Favicon
        };

        StringWriter writer = new();
        JsonSerializer.CreateDefault().Serialize(writer, resp);

        AddString(writer.ToString());

        return base.Build();
    }
}
