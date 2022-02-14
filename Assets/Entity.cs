using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

[Serializable]
public class Entity : IMinecraftObject
{
    [JsonProperty("id")]
    public int Id { get; init; }

    public string Name { get; init; }

    [JsonProperty("displayName")]
    public string DisplayName { get; init; }

    [JsonProperty("width")]
    public double Width { get; init; }

    [JsonProperty("height")]
    public double Height { get; init; }

    public EntityType Type { get; init; }

    [JsonConstructor]
    public Entity(string name, string type)
    {
        Name = name.ToIdentifier();

        Type = Enum.Parse<EntityType>(type.ToPascalCase());

        DisplayName = null!;
    }
}

public enum EntityType
{
    Living,
    Projectile,
    Animal,
    Ambient,
    Passive,
    Hostile,
    Player,
    WaterCreature,
    Mob,
    Other
}
