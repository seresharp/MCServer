using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCServer.Util.Nbt;

namespace MCServer.Server.Packets;

public class ChunkDataPacket : ServerPacket
{
    public ChunkDataPacket()
    {
        Id = 0x22;
    }

    public override byte[] Build()
    {
        // TODO
        AddInt32(0);
        AddInt32(0);

        CompoundTag heightMap = new();
        heightMap.AddChild("MOTION_BLOCKING", new LongArrayTag(Enumerable.Repeat(long.MaxValue, 36).ToArray()));
        AddBytes(heightMap.GetBytes(true));
        AddVarInt(0);
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
