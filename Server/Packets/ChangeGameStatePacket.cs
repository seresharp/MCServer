using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class ChangeGameStatePacket : ServerPacket
    {
        public ChangeGameStatePacket()
        {
            Id = 0x1E;
        }

        public override byte[] Build()
        {
            // TODO
            AddByte(1);
            AddFloat(0);

            return base.Build();
        }
    }
}
