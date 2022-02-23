using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCServer.Client.Packets;

namespace MCServer.Client
{
    public class Player
    {
        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }

        public float Yaw { get; private set; }

        public float Pitch { get; private set; }

        public bool OnGround { get; private set; }

        public void UpdatePosition(PlayerPositionAndLookPacket packet)
        {
            if (packet.HasPositionData)
            {
                X = packet.X;
                Y = packet.Y;
                Z = packet.Z;
            }

            if (packet.HasRotationData)
            {
                Yaw = packet.Yaw;
                Pitch = packet.Pitch;
            }

            OnGround = packet.OnGround;
        }
    }
}
