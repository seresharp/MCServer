using MCServer.Assets;
using MCServer.Util;

namespace MCServer.Server.Packets
{
    public class TagsPacket : ServerPacket
    {
        private readonly MinecraftServer _server;

        public TagsPacket(MinecraftServer server)
        {
            Id = 0x67;

            _server = server;
        }

        public override byte[] Build()
        {
            AddVarInt(_server.Tags.Types.Count());
            foreach (string type in _server.Tags.Types)
            {
                AddString(type);
                AddVarInt(_server.Tags[type].Count());
                foreach (Tag tag in _server.Tags[type])
                {
                    AddString(tag.Name);
                    AddVarInt(tag.Values.Count);
                    foreach (string value in tag.Values)
                    {
                        int id = tag.Type switch
                        {
                            TagType.Block => _server.Blocks[value].Id,
                            TagType.Item => _server.Items[value].Id,
                            TagType.Fluid => (int)Enum.Parse<Fluids>(value.FromIdentifier().ToPascalCase()),
                            TagType.EntityType => _server.Entities[value].Id,
                            TagType.GameEvent => (int)Enum.Parse<GameEvents>(value.FromIdentifier().ToPascalCase()),
                            _ => throw new InvalidDataException()
                        };

                        AddVarInt(id);
                    }
                }
            }

            return base.Build();
        }
    }
}
