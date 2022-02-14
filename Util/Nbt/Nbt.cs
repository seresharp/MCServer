using System.Collections;
using System.Linq;

namespace MCServer.Util.Nbt;

public interface ITag
{
    public byte Id { get; }

    public byte[] GetBytes();
}

public class EndTag : ITag
{
    public byte Id => 0x00;

    public byte[] GetBytes()
        => new[] { Id };
}

public class ByteTag : ITag
{
    public byte Id => 0x01;

    public sbyte Value { get; init; }

    public ByteTag(sbyte value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id, Value.GetTwosComplement() };
}

public class ShortTag : ITag
{
    public byte Id => 0x02;

    public short Value { get; init; }

    public ShortTag(short value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.GetBytes(true)).ToArray();
}

public class IntTag : ITag
{
    public byte Id => 0x03;

    public int Value { get; init; }

    public IntTag(int value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.GetBytes(true)).ToArray();
}

public class LongTag : ITag
{
    public byte Id => 0x04;

    public long Value { get; init; }

    public LongTag(long value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.GetBytes(true)).ToArray();
}

public class FloatTag : ITag
{
    public byte Id => 0x05;

    public float Value { get; init; }

    public FloatTag(float value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.GetBytes(true)).ToArray();
}

public class DoubleTag : ITag
{
    public byte Id => 0x06;

    public double Value { get; init; }

    public DoubleTag(double value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.GetBytes(true)).ToArray();
}

public class ByteArrayTag : ITag
{
    public byte Id => 0x07;

    public sbyte[] Value { get; init; }

    public ByteArrayTag(params sbyte[] value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(Value.Select(b => b.GetTwosComplement())).ToArray();
}

public class StringTag : ITag
{
    public byte Id => 0x08;

    public string Value { get; init; }

    public StringTag(string value)
        => Value = value;

    public byte[] GetBytes()
        => new[] { Id }.Concat(((ushort)Value.Length).GetBytes(true)).Concat(Value.GetBytes()).ToArray();
}

public class ListTag<T> : ITag where T : class, ITag
{
    private readonly List<T> _tags = new();

    public byte Id => 0x09;

    public IList<T> Tags => _tags.AsReadOnly();

    public ListTag(params T[] tags)
        => _tags.AddRange(tags);

    public ListTag(IEnumerable<T> tags)
        => _tags.AddRange(tags);

    public T AddChild(T tag)
    {
        _tags.Add(tag);
        return tag;
    }

    public byte[] GetBytes()
    {
        List<byte> bytes = new();
        bytes.Add(Id);
        bytes.Add(_tags.Count > 0 ? _tags[0].Id : (byte)0x00);
        bytes.AddRange(_tags.Count.GetBytes(true));

        foreach (T tag in _tags)
        {
            // skip id byte since the list is prefixed with it
            bytes.AddRange(tag.GetBytes().Skip(1));
        }

        return bytes.ToArray();
    }

    public T this[int i]
    {
        get => _tags[i];
    }
}

public class CompoundTag : ITag
{
    private readonly Dictionary<string, ITag> _children = new();

    public byte Id => 0x0A;

    public IReadOnlyDictionary<string, ITag> Children => _children;

    public T AddChild<T>(string name, T tag) where T : ITag
    {
        _children.Add(name, tag);
        return tag;
    }

    public T GetChild<T>(string name) where T : ITag
    {
        return (T)_children[name];
    }

    public byte[] GetBytes()
        => GetBytes(false);

    public byte[] GetBytes(bool isRoot)
    {
        List<byte> bytes = new();
        bytes.Add(Id);

        if (isRoot)
        {
            bytes.AddRange(new byte[] { 0x00, 0x00 });
        }

        foreach ((string name, ITag tag) in _children)
        {
            // id goes before the name, but not if it's the root tag? why is this format the way it is
            bytes.Add(tag.Id);

            bytes.AddRange(((ushort)name.Length).GetBytes(true));
            bytes.AddRange(name.GetBytes());

            // skip id since it's already in
            bytes.AddRange(tag.GetBytes().Skip(1));
        }

        bytes.AddRange(new EndTag().GetBytes());

        return bytes.ToArray();
    }

    public object this[string name]
    {
        set
        {
            if (value is not ITag tag)
            {
                tag = NbtConverter.ToNbt(value);
            }

            AddChild(name, tag);
        }
    }
}

public class IntArrayTag : ITag
{
    private readonly List<int> _ints = new();

    public byte Id => 0x0B;

    public IList<int> Ints => _ints.AsReadOnly();

    public IntArrayTag(params int[] ints)
        => _ints.AddRange(ints);

    public byte[] GetBytes()
    {
        List<byte> bytes = new();
        bytes.Add(Id);
        bytes.AddRange(_ints.Count.GetBytes(true));

        foreach (int i in _ints)
        {
            bytes.AddRange(i.GetBytes(true));
        }

        return bytes.ToArray();
    }
}

public class LongArrayTag : ITag
{
    private readonly List<long> _longs = new();

    public byte Id => 0x0C;

    public IList<long> Longs => _longs.AsReadOnly();

    public LongArrayTag(params long[] longs)
        => _longs.AddRange(longs);

    public byte[] GetBytes()
    {
        List<byte> bytes = new();
        bytes.Add(Id);
        bytes.AddRange(_longs.Count.GetBytes(true));

        foreach (long l in _longs)
        {
            bytes.AddRange(l.GetBytes(true));
        }

        return bytes.ToArray();
    }
}
