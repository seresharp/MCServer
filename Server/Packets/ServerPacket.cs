using System.Text;
using MCServer.Util;

namespace MCServer.Server.Packets;

public abstract class ServerPacket
{
    private readonly List<byte> data = new();

    public int Id { get; init; }

    protected void AddBytes(IEnumerable<byte> bytes)
    {
        data.AddRange(bytes);
    }

    protected void AddString(string str)
    {
        byte[] b = Encoding.ASCII.GetBytes(str);
        data.Add((byte)b.Length);
        data.AddRange(b);
    }

    protected void AddInt32(int i)
    {
        byte[] bytes = BitConverter.GetBytes(i);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        data.AddRange(bytes);
    }

    protected void AddInt64(long l)
    {
        byte[] bytes = BitConverter.GetBytes(l);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        data.AddRange(bytes);
    }

    protected void AddFloat(float f)
    {
        AddBytes(f.GetBytes(true));
    }

    protected void AddDouble(double d)
    {
        AddBytes(d.GetBytes(true));
    }

    protected void AddBool(bool b)
    {
        data.Add(b ? (byte)0x01 : (byte)0x00);
    }

    protected void AddByte(byte b)
    {
        data.Add(b);
    }

    protected void AddSByte(sbyte sb)
    {
        byte b = (byte)~sb;
        b++;

        data.Add(b);
    }

    protected void AddVarInt(int i)
    {
        data.AddRange(i.ToVarInt());
    }

    public virtual byte[] Build()
    {
        data.InsertRange(0, Id.ToVarInt());
        data.InsertRange(0, data.Count.ToVarInt());

        Console.WriteLine("built packet of size " + data.Count);

        return data.ToArray();
    }
}
