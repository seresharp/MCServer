using System.Collections.ObjectModel;

namespace MCServer.Util
{
    public class CompactLongArray
    {
        private readonly long[] _longs;

        public int BitsPerEntry { get; init; }

        public int MaxValue { get; init; }

        private int EntriesPerLong { get; init; }

        public int Length { get; init; }

        public ReadOnlyCollection<long> Longs => Array.AsReadOnly(_longs);

        public CompactLongArray(int bitsPerEntry, int length)
        {
            // has to fit into an int
            // could do uint to get that 32nd bit, but then it'd be annoying to work with
            if (bitsPerEntry <= 0 || bitsPerEntry > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerEntry));
            }

            Length = length;

            BitsPerEntry = bitsPerEntry;
            MaxValue = (int)Math.Pow(2, BitsPerEntry) - 1;
            EntriesPerLong = 64 / bitsPerEntry;

            _longs = new long[(int)Math.Ceiling(Length / (float)EntriesPerLong)];
        }

        private void CheckValid(int item)
        {
            if (item < 0 || item > MaxValue)
            {
                throw new ArgumentOutOfRangeException($"Value {item} is outside of the range 0 - {MaxValue} and cannot be stored in this {nameof(CompactLongArray)}");
            }
        }

        public int this[int index]
        {
            get
            {
                int arrayIndex = index / EntriesPerLong;
                int offset = (index % EntriesPerLong) * BitsPerEntry;

                return (int)((_longs[arrayIndex] >> offset) & ((1L << BitsPerEntry) - 1));
            }
            set
            {
                CheckValid(value);
                int arrayIndex = index / EntriesPerLong;
                int offset = (index % EntriesPerLong) * BitsPerEntry;

                _longs[arrayIndex] |= (long)value << offset;
            }
        }
    }
}
