using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class TimeUpdatePacket : ServerPacket
    {
        public TimeUpdatePacket()
        {
            Id = 0x59;
        }

        public override byte[] Build()
        {
            // TODO
            AddInt64(0);
            AddInt64(0);

            return base.Build();
        }
    }
}
