using System.Collections.ObjectModel;
using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

[Serializable]
public sealed class Tag
{
    private readonly string[] _rawValues;
    private ReadOnlyCollection<string>? _values;

    public TagType Type { get; set; }

    public string Name { get; init; }

    public ReadOnlyCollection<string> Values
        => _values ?? throw new InvalidOperationException($"Attempted to access {nameof(Values)} before calling {nameof(PopulateValues)}");

    [JsonConstructor]
    private Tag(string name, string[] values)
    {
        Name = name.ToIdentifier();

        _rawValues = values;
        for (int i = 0; i < _rawValues.Length; i++)
        {
            _rawValues[i] = _rawValues[i].ToIdentifier();
        }
    }

    public void PopulateValues(IReadOnlyDictionary<string, Tag> tags)
    {
        if (_values != null)
        {
            return;
        }

        List<string> parsed = new();
        foreach (string id in _rawValues)
        {
            if (!id.StartsWith('#'))
            {
                parsed.Add(id);
                continue;
            }

            Tag tag = tags[id[1..]];
            tag.PopulateValues(tags);
            parsed.AddRange(tag.Values);
        }

        _values = Array.AsReadOnly(parsed.ToArray());
    }
}

public enum TagType
{
    Block,
    Item,
    Fluid,
    EntityType,
    GameEvent
}
