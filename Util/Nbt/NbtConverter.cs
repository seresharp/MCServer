using System.Reflection;
using System.Runtime.CompilerServices;

namespace MCServer.Util.Nbt
{
    public static class NbtConverter
    {
        public static ITag ToNbt(object obj)
            => ToNbtInternal(obj, out _);

        private static ITag ToNbtInternal(object obj, out string? forceName)
        {
            forceName = null;

            return obj switch
            {
                sbyte b => new ByteTag(b),
                bool b => new ByteTag(b.AsSignedByte()),
                short s => new ShortTag(s),
                int i => new IntTag(i),
                long l => new LongTag(l),
                float f => new FloatTag(f),
                double d => new DoubleTag(d),
                string str => new StringTag(str),
                Enum e => new StringTag(e.ToString().ToSnakeCase()),
                IEnumerable<sbyte> bytes => new ByteArrayTag(bytes.ToArray()),
                IEnumerable<int> ints => new IntArrayTag(ints.ToArray()),
                IEnumerable<long> longs => new LongArrayTag(longs.ToArray()),
                IEnumerable<short> shorts => new ListTag<ShortTag>(shorts.Select(s => new ShortTag(s))),
                IEnumerable<float> floats => new ListTag<FloatTag>(floats.Select(f => new FloatTag(f))),
                IEnumerable<double> doubles => new ListTag<DoubleTag>(doubles.Select(d => new DoubleTag(d))),
                IEnumerable<string> strings => new ListTag<StringTag>(strings.Select(s => new StringTag(s))),
                IEnumerable<object> objects => new ListTag<CompoundTag>(objects.Select(o => ParseObject(o, out _))),
                ITag tag => tag,
                object => ParseObject(obj, out forceName),
                _ => throw new InvalidDataException($"Could not parse {obj} to nbt")
            };
        }

        // TODO: this is incredibly slow, make it not be that
        private static CompoundTag ParseObject(object obj, out string? forceName)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            forceName = null;
            CompoundTag tag = new();
            foreach (MemberInfo member in obj.GetType().GetMembers(flags)
                .Where(m => m is FieldInfo || m is PropertyInfo))
            {
                if (member.CustomAttributes.Any(attr => attr.AttributeType == typeof(NbtIgnoreAttribute))
                    || member.Name.StartsWith('<'))
                {
                    continue;
                }

                object? val = member switch
                {
                    FieldInfo field => field.GetValue(obj),
                    PropertyInfo prop => prop.GetGetMethod(true)?.Invoke(obj, null),
                    _ => throw new Exception("Unhandled member type")
                };

                if (member.Name == "NbtName" && val != null)
                {
                    forceName = (string)val;
                    continue;
                }

                if (val != null)
                {
                    ITag childTag = ToNbtInternal(val, out string? childName);
                    tag.AddChild(childName ?? member.Name.ToSnakeCase(), childTag);
                }
            }

            return tag;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NbtIgnoreAttribute : Attribute
    {
    }
}
