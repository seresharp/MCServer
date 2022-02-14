using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using MCServer.Util;

namespace MCServer.Assets;

[Serializable]
public sealed class Block : IMinecraftObject
{
    [JsonProperty("id")]
    public int Id { get; init; }

    public string Name { get; init; }

    [JsonProperty("displayName")]
    public string DisplayName { get; init; }

    public double Hardness { get; init; }

    [JsonProperty("resistance")]
    public double Resistance { get; init; }

    [JsonProperty("minStateId")]
    public int MinState { get; init; }

    [JsonProperty("maxStateId")]
    public int MaxState { get; init; }

    [JsonProperty("defaultState")]
    public int DefaultState { get; init; }

    [JsonProperty("diggable")]
    public bool Diggable { get; init; }

    [JsonProperty("transparent")]
    public bool Transparent { get; init; }

    [JsonProperty("solid")]
    public bool Solid { get; init; }

    [JsonProperty("filterLight")]
    public int FilterLight { get; init; }

    [JsonProperty("emitLight")]
    public int EmitLight { get; init; }

    public string[] Material { get; init; }

    [JsonProperty("harvestTools")]
    public int[] HarvestTools { get; init; }

    public PropertyCollection Properties { get; init; }

    [JsonConstructor]
    [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Json.NET")]
    private Block(string name, double? hardness, string material, Property[] properties)
    {
        Name = name.ToIdentifier();

        Hardness = hardness ?? double.PositiveInfinity;
        Material = material.Split(';');
        Properties = new(properties);

        DisplayName = null!;
        HarvestTools = null!;
    }

    public int GetStateId(params (string name, string val)[] properties)
    {
        int id = MinState;
        foreach ((string name, string val) in properties)
        {
            id += Properties[name].GetStateModifier(val);
        }

        return id;
    }

    public class PropertyCollection : IReadOnlyList<Property>
    {
        private readonly Property[] _properties;
        private readonly Dictionary<string, Property> _propNameLookup;

        public int Count => _properties.Length;

        public PropertyCollection(params Property[] props)
        {
            _properties = props;
            _propNameLookup = _properties.ToDictionary(p => p.Name);

            int mul = 1;
            for (int i = Count - 1; i >= 0; i--)
            {
                this[i].SetMultiplier(mul);
                mul *= this[i].Values.Count;
            }
        }

        public IEnumerator<Property> GetEnumerator()
            => (IEnumerator<Property>)_properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public Property this[int index]
            => _properties[index];

        public Property this[string name]
            => _propNameLookup[name];
    }

    [Serializable]
    public class Property
    {
        private int _multiplier = 1;

        [JsonProperty("name")]
        public string Name { get; init; }

        public ReadOnlyCollection<string> Values { get; init; }

        [JsonConstructor]
        public Property(string type, int num_values, string[]? values)
        {
            Values = type switch
            {
                "bool" => Array.AsReadOnly(new[] { "true", "false" }),
                "enum" => Array.AsReadOnly(values ?? Array.Empty<string>()),
                "int" => Array.AsReadOnly(Enumerable.Range(0, num_values).Select(i => $"{i}").ToArray()),
                _ => throw new NotImplementedException("Property type " + type),
            };

            Name ??= string.Empty;
        }

        public int GetStateModifier(string val)
        {
            int i = Values.IndexOf(val);
            if (i == -1)
            {
                throw new InvalidOperationException($"Attempted to fetch index of non-existent value '{val}' on property '{Name}'");
            }

            return i * _multiplier;
        }

        public void SetMultiplier(int m)
            => _multiplier = m;
    }
}
