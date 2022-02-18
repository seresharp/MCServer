using MCServer.Assets;
using MCServer.Util;
using MCServer.Util.Nbt;

namespace MCServer.Server.Packets;

public class JoinGamePacket : ServerPacket
{
    private readonly MinecraftServer _server;

    public JoinGamePacket(MinecraftServer server)
    {
        Id = 0x26;

        _server = server;
    }

    public override byte[] Build()
    {
        // TODO
        AddInt32(56);
        AddBool(false);
        AddByte(1);
        AddSByte(-1);
        AddVarInt(_server.Dimensions.Count);
        foreach (Dimension dim in _server.Dimensions)
        {
            AddString(dim.Name);
        }

        CompoundTag dimCodec = (CompoundTag)NbtConverter.ToNbt(new
        {
            dims = new
            {
                NbtName = "minecraft:dimension_type",
                type = "minecraft:dimension_type",
                value = _server.Dimensions.Select(dim => new
                {
                    name = dim.Name,
                    id = dim.Id,
                    element = dim
                })
            },
            biomes = new
            {
                NbtName = "minecraft:worldgen/biome",
                type = "minecraft:worldgen/biome",
                value = _server.Biomes.Select(b => new
                {
                    name = b.Name,
                    id = b.Id,
                    element = b
                })
            }
        });

        AddBytes(dimCodec.GetBytes(true));
        AddBytes(dimCodec
            .GetChild<CompoundTag>("minecraft:dimension_type")
            .GetChild<ListTag<CompoundTag>>("value")[0]
            .GetChild<CompoundTag>("element")
            .GetBytes(true));

        AddString(_server.Dimensions[0].Name);
        AddInt64(420);
        AddVarInt(20);
        AddVarInt(5);
        AddVarInt(5);
        AddBool(false);
        AddBool(true);
        AddBool(false);
        AddBool(false);

        return base.Build();
    }
}
