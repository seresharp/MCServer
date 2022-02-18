using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCServer.Util;

namespace MCServer.World
{
    public class ChunkColumn
    {
        private readonly ChunkSection[] _sections;

        public int X { get; init; }

        public int Z { get; init; }

        public int Height { get; init; }

        public ChunkColumn(int height, int x, int z)
        {
            if (height % 16 != 0)
            {
                throw new ArgumentException("Height must be a multiple of 16");
            }

            X = x;
            Z = z;
            Height = height;

            _sections = new ChunkSection[Height / 16];
            for (int i = 0; i < _sections.Length; i++)
            {
                _sections[i] = new();
            }
        }

        public BlockState GetBlock(int x, int y, int z)
        {
            CheckBounds(x, y, z);
            return _sections[y / 16].GetBlock(x, y % 16, z);
        }

        public void SetBlock(int x, int y, int z, BlockState state)
        {
            CheckBounds(x, y, z);
            _sections[y / 16].SetBlock(x, y % 16, z, state);
        }

        public byte[] Serialize()
        {
            List<byte> data = new();
            data.AddRange(_sections.Length.ToVarInt());
            foreach (ChunkSection section in _sections)
            {
                data.AddRange(section.Serialize());
            }

            data.InsertRange(0, data.Count.ToVarInt());
            return data.ToArray();
        }

        private void CheckBounds(int x, int y, int z)
        {
            if (x < 0 || x > 15 || y < 0 || y > (Height - 1) || z < 0 || z > 15)
            {
                throw new ArgumentOutOfRangeException($"This chunk column is 16x{Height}x16, ({x}, {y}, {z}) is out of range");
            }
        }

        public BlockState this[int x, int y, int z]
        {
            get => GetBlock(x, y, z);
            set => SetBlock(x, y, z, value);
        }
    }
}
