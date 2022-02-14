using System.Collections;
using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

public class GenericLoader<T> : IEnumerable<T> where T : IMinecraftObject
{
    private readonly T[] _items;

    private readonly Dictionary<int, T> _idLookup;
    private readonly Dictionary<string, T> _nameLookup;

    public GenericLoader(string jsonPath)
    {
        _items = JsonConvert.DeserializeObject<T[]>(File.ReadAllText(jsonPath))
            ?? throw new Exception("Failed to load items json");

        _idLookup = _items.ToDictionary(i => i.Id);
        _nameLookup = _items.ToDictionary(i => i.Name);
    }

    public T this[int id]
        => _idLookup[id];

    public T this[string name]
        => _nameLookup[name.ToIdentifier()];

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)_items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _items.GetEnumerator();
    }
}
