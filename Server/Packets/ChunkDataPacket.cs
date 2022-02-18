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
    public int X { get; init; }

    public int Z { get; init; }

    public ChunkDataPacket(int x, int z)
    {
        Id = 0x22;

        X = x;
        Z = z;
    }

    public override byte[] Build()
    {
        AddInt32(X);
        AddInt32(Z);

        Util.CompactLongArray motionBlocking = new(9, 256);
        for (int i = 0; i < motionBlocking.Length; i++)
        {
            motionBlocking[i] = 15;
        }

        CompoundTag heightMap = new();
        heightMap.AddChild("MOTION_BLOCKING", new LongArrayTag(motionBlocking.Longs.ToArray()));
        AddBytes(heightMap.GetBytes(true));

        ChunkColumn chunk = new(384, 0, 0);
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    chunk[x, y, z] = new BlockState { BlockId = 8, StateId = 9 };
                }
            }
        }

        AddBytes(chunk.Serialize());

        AddVarInt(0);
        AddBool(true);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);
        AddVarInt(0);

        return base.Build();
    }
}
