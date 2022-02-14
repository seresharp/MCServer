using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class PlayerInfoPacket : ServerPacket
    {
        public PlayerInfoPacket()
        {
            Id = 0x36;
        }

        public override byte[] Build()
        {
            // TODO
            AddVarInt(0);
            AddVarInt(0);

            return base.Build();
        }
    }
}
