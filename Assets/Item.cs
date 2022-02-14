using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

[Serializable]
public sealed class Item : IMinecraftObject
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("displayName")]
    public string DisplayName { get; init; }

    public string Name { get; init; }

    [JsonProperty("stackSize")]
    public int StackSize { get; init; }

    [JsonConstructor]
    [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Json.NET")]
    private Item(string name)
    {
        Name = name.ToIdentifier();

        DisplayName = null!;
    }
}
