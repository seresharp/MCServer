using Newtonsoft.Json;

namespace MCServer.Assets
{
    public class Dimension : IMinecraftObject
    {
        [JsonProperty("name")]
        [Util.Nbt.NbtIgnore]
        public string Name { get; set; } = null!;

        [JsonProperty("id")]
        [Util.Nbt.NbtIgnore]
        public int Id { get; set; }

        [JsonProperty("logical_height")]
        public int LogicalHeight { get; set; }

        [JsonProperty("infiniburn")]
        public string Infiniburn { get; set; } = null!;

        [JsonProperty("effects")]
        public string Effects { get; set; } = null!;

        [JsonProperty("ambient_light")]
        public float AmbientLight { get; set; }

        [JsonProperty("respawn_anchor_works")]
        public bool RespawnAnchorWorks { get; set; }

        [JsonProperty("has_raids")]
        public bool HasRaids { get; set; }

        [JsonProperty("min_y")]
        public int MinY { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("natural")]
        public bool Natural { get; set; }

        [JsonProperty("coordinate_scale")]
        public float CoordinateScale { get; set; }

        [JsonProperty("piglin_safe")]
        public bool PiglinSafe { get; set; }

        [JsonProperty("bed_works")]
        public bool BedWorks { get; set; }

        [JsonProperty("has_skylight")]
        public bool HasSkylight { get; set; }

        [JsonProperty("has_ceiling")]
        public bool HasCeiling { get; set; }

        [JsonProperty("ultrawarm")]
        public bool Ultrawarm { get; set; }

        [JsonProperty("fixed_time", NullValueHandling = NullValueHandling.Ignore)]
        public long? FixedTime { get; set; }
    }
}
