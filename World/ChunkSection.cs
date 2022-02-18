using MCServer.Util;

namespace MCServer.World
{
    public class ChunkSection
    {
        private readonly BlockState[,,] _blocks = new BlockState[16, 16, 16];

        public BlockState GetBlock(int x, int y, int z)
        {
            CheckBounds(x, y, z);
            return _blocks[x, y, z];
        }

        public void SetBlock(int x, int y, int z, BlockState state)
        {
            CheckBounds(x, y, z);
            _blocks[x, y, z] = state;
        }

        public byte[] Serialize()
        {
            List<byte> data = new();

            short nonAir = (short)_blocks.OfType<BlockState>().Count(b => b.BlockId != 0);
            data.AddRange(nonAir.GetBytes(true));
            if (nonAir == 0)
            {
                // single valued palette

                // bits per entry
                data.Add(0);

                // block id
                data.AddRange(0.ToVarInt());

                // long array length (always 0 for this type)
                data.AddRange(0.ToVarInt());
            }
            else
            {
                // direct palette

                // bits per entry
                data.Add(15);

                // block ids
                CompactLongArray ids = new(15, 16 * 16 * 16);

                int i = 0;
                for (int x = 0; x < 16; x++)
                {
                    for (int y = 0; y < 16; y++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            ids[i++] = _blocks[x, y, z].StateId;
                        }
                    }
                }

                ICollection<long> longs = ids.Longs;
                data.AddRange(longs.Count.ToVarInt());
                foreach (long l in longs)
                {
                    data.AddRange(l.GetBytes(true));
                }
            }

            // TODO: biomes
            data.Add(6);

            CompactLongArray biomes = new(6, 64);
            for (int i = 0; i < 64; i++)
            {
                // desert
                biomes[i] = 5;
            }

            ICollection<long> biomeLongs = biomes.Longs;
            data.AddRange(biomeLongs.Count.ToVarInt());
            foreach (long l in biomeLongs)
            {
                data.AddRange(l.GetBytes(true));
            }

            Console.WriteLine(BitConverter.ToString(biomeLongs.First().GetBytes(false)));

            return data.ToArray();
        }

        private static void CheckBounds(int x, int y, int z)
        {
            if (x < 0 || x > 15 || y < 0 || y > 15 || z < 0 || z > 15)
            {
                throw new ArgumentOutOfRangeException($"Chunk sections are 16x16x16, ({x}, {y}, {z}) is out of range");
            }
        }

        public BlockState this[int x, int y, int z]
        {
            get => GetBlock(x, y, z);
            set => SetBlock(x, y, z, value);
        }
    }

    public struct BlockState
    {
        public int BlockId;
        public int StateId;

        public BlockState(Assets.Block block)
        {
            BlockId = block.Id;
            StateId = block.DefaultState;
        }
    }
}
