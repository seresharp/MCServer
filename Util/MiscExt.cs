using System.Collections;

namespace MCServer.Util;

public static class MiscExt
{
    public static string ToPascalCase(this string str)
    {
        List<char> chars = str.ToList();
        int i;
        while ((i = chars.IndexOf('_')) != -1)
        {
            chars.RemoveAt(i);
            if (chars.Count <= i)
            {
                break;
            }

            chars[i] = char.ToUpper(chars[i]);
        }

        chars[0] = char.ToUpper(chars[0]);
        return new(chars.ToArray());
    }

    public static string ToSnakeCase(this string str)
    {
        // assume it's already snake case. not the best, but it'll do
        if (str.Contains('_'))
        {
            return str;
        }

        List<char> chars = str.ToList();
        for (int i = 0; i < chars.Count; i++)
        {
            if (!char.IsUpper(chars[i]))
            {
                continue;
            }

            if (i > 0)
            {
                chars.Insert(i++, '_');
            }

            chars[i] = char.ToLower(chars[i]);
        }

        return new(chars.ToArray());
    }

    public static string ToIdentifier(this string str)
        => str.Contains(':')
            ? str
            : $"minecraft:{str}";

    public static string FromIdentifier(this string str)
        => str.Contains(':')
            ? str[(str.IndexOf(':') + 1)..]
            : str;

    public static sbyte AsSignedByte(this bool b)
        => (sbyte)(b ? 1 : 0);
}
