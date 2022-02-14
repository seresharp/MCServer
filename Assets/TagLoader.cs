using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

public class TagLoader
{
    private readonly Dictionary<string, Dictionary<string, Tag>> _tags;

    public IEnumerable<string> Types
        => _tags.Keys;

    public TagLoader(string jsonPath)
    {
        _tags = new();
        foreach ((string type, Tag[] tags) in
            JsonConvert.DeserializeObject<Dictionary<string, Tag[]>>(File.ReadAllText(jsonPath))
            ?? throw new Exception("Failed to load tags json"))
        {
            TagType tagType = Enum.Parse<TagType>(type.ToPascalCase());
            string typeId = type.ToIdentifier();

            _tags[typeId] = new();
            foreach (Tag t in tags)
            {
                t.Type = tagType;
                _tags[typeId][t.Name] = t;
            }
        }

        foreach ((string type, Dictionary<string, Tag> tags) in _tags)
        {
            foreach (Tag tag in tags.Values)
            {
                tag.PopulateValues(tags);
            }
        }
    }

    public Tag this[string type, string name]
        => _tags[type.ToIdentifier()][name.ToIdentifier()];

    public IEnumerable<Tag> this[string type]
    {
        get
        {
            foreach ((_, Tag t) in _tags[type.ToIdentifier()])
            {
                yield return t;
            }
        }
    }
}
