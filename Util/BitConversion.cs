using System.Text;

namespace MCServer.Util;

public static class BitConversion
{
    public static byte GetTwosComplement(this sbyte sb)
    {
        byte b = (byte)~sb;
        return ++b;
    }

    public static byte AsByte(this bool b)
    {
        return b ? (byte)0x01 : (byte)0x00;
    }

    public static byte[] GetBytes(this string str, bool includeLength = false)
    {
        List<byte> bytes = new();
        if (includeLength)
        {
            bytes.AddRange(str.Length.ToVarInt());
        }

        bytes.AddRange(Encoding.ASCII.GetBytes(str));
        return bytes.ToArray();
    }

    public static byte[] GetBytes(this short s, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(s);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this ushort us, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(us);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this int i, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this uint ui, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(ui);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this long l, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(l);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this ulong ul, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(ul);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this float f, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(f);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] GetBytes(this double d, bool bigEndian)
    {
        byte[] bytes = BitConverter.GetBytes(d);
        CheckEndianness(bytes, bigEndian);
        return bytes;
    }

    public static byte[] ToVarInt(this int i)
    {
        List<byte> b = new();

        while (true)
        {
            if ((i & ~0x7F) == 0)
            {
                b.Add((byte)i);
                return b.ToArray();
            }

            b.Add((byte)((i & 0x7F) | 0x80));
            i >>= 7;
        }
    }

    private static void CheckEndianness(byte[] bytes, bool bigEndian)
    {
        if (bigEndian == BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
    }
}
