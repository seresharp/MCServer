using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCServer.Util.Nbt;
using MCServer.World;

namespace MCServer.Server.Packets;

public class ChunkDataPacket : ServerPacket
{
    public ChunkColumn Chunk { get; init; }

    public ChunkDataPacket(ChunkColumn chunk)
    {
        Id = 0x22;

        Chunk = chunk;
    }

    public override byte[] Build()
    {
        AddInt32(Chunk.X);
        AddInt32(Chunk.Z);

        Util.CompactLongArray motionBlocking = new(9, 256);
        int i = 0;
        for (int z = 0; z < 16; z++)
        {
            for (int x = 0; x < 16; x++)
            {
                motionBlocking[i++] = Chunk.GetHeight(x, z);
            }
        }

        CompoundTag heightMap = new();
        heightMap.AddChild("MOTION_BLOCKING", new LongArrayTag(motionBlocking.Longs.ToArray()));
        AddBytes(heightMap.GetBytes(true));

        AddBytes(Chunk.Serialize());

        // block entity count
        AddVarInt(0);

        // trust edges for light updates
        // not sure what that even means
        AddBool(true);

        // TODO: lighting info
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);

        return base.Build();
    }
}
