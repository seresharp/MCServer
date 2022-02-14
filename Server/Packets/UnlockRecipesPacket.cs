using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class UnlockRecipesPacket : ServerPacket
    {
        public UnlockRecipesPacket()
        {
            Id = 0x39;
        }

        public override byte[] Build()
        {
            // TODO
            AddVarInt(0);
            for (int i = 0; i < 8; i++)
            {
                AddBool(false);
            }

            AddVarInt(0);
            AddVarInt(0);

            return base.Build();
        }
    }
}
