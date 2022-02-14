using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class DeclareRecipesPacket : ServerPacket
    {
        public DeclareRecipesPacket()
        {
            Id = 0x66;
        }

        public override byte[] Build()
        {
            // TODO
            AddVarInt(0);

            return base.Build();
        }
    }
}
