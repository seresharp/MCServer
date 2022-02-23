namespace MCServer.Client.Packets
{
    public class PlayerPositionAndLookPacket : ClientPacket
    {
        public bool HasPositionData { get; private set; }

        public double X { get; private set; }

        public double Y { get; private set; }

        public double Z { get; private set; }

        public bool OnGround { get; private set; }

        public bool HasRotationData { get; private set; }

        public float Yaw { get; private set; }

        public float Pitch { get; private set; }

        public override void ReadData(int id, byte[] data)
        {
            Id = id;
            int pos = 0;

            if (id is 0x11 or 0x12)
            {
                HasPositionData = true;

                X = ReadDouble(data, ref pos);
                Y = ReadDouble(data, ref pos);
                Z = ReadDouble(data, ref pos);
            }

            if (id is 0x12 or 0x13)
            {
                HasRotationData = true;
                Yaw = ReadFloat(data, ref pos);
                Pitch = ReadFloat(data, ref pos);
            }

            OnGround = ReadBool(data, ref pos);
        }
    }
}
