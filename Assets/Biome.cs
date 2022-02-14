using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using MCServer.Util;
using MCServer.Util.Nbt;

namespace MCServer.Assets;

[Serializable]
public sealed class Biome : IMinecraftObject
{
    [Util.Nbt.NbtIgnore]
    public string Name { get; init; }

    [JsonProperty("id")]
    [Util.Nbt.NbtIgnore]
    public int Id { get; init; }

    [JsonProperty("effects")]
    public BiomeEffects Effects { get; init; } = null!;

    [JsonProperty("depth")]
    public float Depth { get; init; }

    [JsonProperty("temperature")]
    public float Temperature { get; init; }

    [JsonProperty("scale")]
    public float Scale { get; init; }

    [JsonProperty("downfall")]
    public float Downfall { get; init; }

    public BiomeCategory Category { get; init; }

    [JsonProperty("precipitation")]
    public Precipitation Precipitation { get; init; }

    [JsonProperty("temperature_modifier")]
    public TempModifier? TemperatureModifier { get; init; }

    [JsonConstructor]
    [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Json.NET")]
    private Biome(string name, string category)
    {
        Name = name.ToIdentifier();
        Category = Enum.Parse<BiomeCategory>(category.ToPascalCase());
    }
}

[Serializable]
public sealed class BiomeEffects
{
    [JsonProperty("music")]
    public BiomeMusic? Music { get; init; }

    [JsonProperty("mood_sound")]
    public BiomeMood? MoodSound { get; init; }

    [JsonProperty("foliage_color")]
    public int? FoliageColor { get; init; }

    [JsonProperty("sky_color")]
    public int SkyColor { get; init; }

    [JsonProperty("water_fog_color")]
    public int WaterFogColor { get; init; }

    [JsonProperty("fog_color")]
    public int FogColor { get; init; }

    [JsonProperty("water_color")]
    public int WaterColor { get; init; }

    [JsonProperty("grass_color")]
    public int? GrassColor { get; init; }

    public GrassModifier? GrassColorModifier { get; init; }

    [JsonProperty("ambient_sound")]
    public string? AmbientSound { get; init; }

    [JsonProperty("additions_sound")]
    public BiomeAdditionalSound? AdditionsSound { get; init; }

    [JsonProperty("particle")]
    public BiomeParticle? Particle { get; init; }

    [JsonConstructor]
    [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Json.NET")]
    private BiomeEffects(string grass_color_modifier)
    {
        if (grass_color_modifier != null)
        {
            GrassColorModifier = Enum.Parse<GrassModifier>(grass_color_modifier.ToPascalCase());
        }
    }
}

[Serializable]
public sealed class BiomeAdditionalSound
{
    [JsonProperty("sound")]
    public string Sound { get; init; } = null!;

    [JsonProperty("tick_chance")]
    public double TickChance { get; init; }

    private BiomeAdditionalSound() { }
}

[Serializable]
public sealed class BiomeMood
{
    [JsonProperty("sound")]
    public string Sound { get; init; } = null!;

    [JsonProperty("offset")]
    public double Offset { get; init; }

    [JsonProperty("tick_delay")]
    public int TickDelay { get; init; }

    [JsonProperty("block_search_extent")]
    public int BlockSearchExtent { get; init; }

    private BiomeMood() { }
}

[Serializable]
public sealed class BiomeMusic
{
    [JsonProperty("max_delay")]
    public int MaxDelay { get; init; }

    [JsonProperty("sound")]
    public string Sound { get; init; } = null!;

    [JsonProperty("min_delay")]
    public int MinDelay { get; init; }

    [JsonProperty("replace_current_music")]
    public bool ReplaceCurrentMusic { get; init; }

    private BiomeMusic() { }
}

[Serializable]
public sealed class BiomeParticle
{
    private readonly Options options;

    [JsonProperty("probability")]
    public float Probability { get; init; }

    [NbtIgnore]
    public string Type => options.Type;

    [JsonConstructor]
    [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Json.NET")]
    private BiomeParticle(string type)
    {
        options = new(type);
    }

    private class Options
    {
        public string Type { get; init; }

        public Options(string type)
            => Type = type;
    }
}

public enum Precipitation
{
    None,
    Rain,
    Snow
}

public enum TempModifier
{
    Frozen
}

public enum GrassModifier
{
    Swamp,
    DarkForest
}

public enum BiomeCategory
{
    None,
    Mountain,
    Forest,
    Ocean,
    Nether,
    Taiga,
    Icy,
    Jungle,
    Beach,
    ExtremeHills,
    Savanna,
    Mesa,
    Plains,
    Desert,
    Swamp,
    River,
    TheEnd,
    Mushroom,
    Underground
}
