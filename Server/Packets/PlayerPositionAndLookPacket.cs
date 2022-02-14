using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets;

public class PlayerPositionAndLookPacket : ServerPacket
{
    public PlayerPositionAndLookPacket()
    {
        Id = 0x38;
    }

    public override byte[] Build()
    {
        // TODO
        AddDouble(0);
        AddDouble(0);
        AddDouble(0);
        AddFloat(0);
        AddFloat(0);
        AddByte(0);
        AddVarInt(623);
        AddBool(true);

        return base.Build();
    }
}
