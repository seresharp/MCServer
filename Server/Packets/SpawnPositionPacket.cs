using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCServer.Server.Packets
{
    public class SpawnPositionPacket : ServerPacket
    {
        public SpawnPositionPacket()
        {
            Id = 0x4B;
        }

        public override byte[] Build()
        {
            // TODO
            AddInt64(0);
            AddFloat(0);

            return base.Build();
        }
    }
}
