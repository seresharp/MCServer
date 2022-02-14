namespace MCServer.Server.Packets
{
    public class CommandsPacket : ServerPacket
    {
        public CommandsPacket()
        {
            Id = 0x12;
        }

        public override byte[] Build()
        {
            // TODO
            // node array count
            AddVarInt(1);

            // root node with no children
            AddByte(0);
            AddVarInt(0);

            // root node index
            AddVarInt(0);

            return base.Build();
        }
    }
}
