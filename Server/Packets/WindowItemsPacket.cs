using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class WindowItemsPacket : ServerPacket
    {
        public WindowItemsPacket()
        {
            Id = 0x14;
        }

        public override byte[] Build()
        {
            // TODO
            AddByte(0);
            AddVarInt(420);
            AddVarInt(45);
            for (int i = 0; i < 45; i++)
            {
                AddBool(false);
            }

            AddBool(false);

            return base.Build();
        }
    }
}
