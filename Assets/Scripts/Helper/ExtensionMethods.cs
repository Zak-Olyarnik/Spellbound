using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{
    public static string ToDelimitedString<T>(this IEnumerable<T> source)
    {
        return source.ToDelimitedString(x => x.ToString(),
            CultureInfo.CurrentCulture.TextInfo.ListSeparator);
    }

    public static string ToDelimitedString<T>(
        this IEnumerable<T> source, Func<T, string> converter)
    {
        return source.ToDelimitedString(converter,
            CultureInfo.CurrentCulture.TextInfo.ListSeparator);
    }

    public static string ToDelimitedString<T>(
        this IEnumerable<T> source, string separator)
    {
        return source.ToDelimitedString(x => x.ToString(), separator);
    }

    public static string ToDelimitedString<T>(this IEnumerable<T> source,
        Func<T, string> converter, string separator)
    {
        return string.Join(separator, source.Select(converter).ToArray());
    }

    public static IEnumerable<string> ReadLines(this System.IO.TextReader reader, char[] delimiter)
    {
        List<char> chars = new List<char>();
        int d = 0; /* Index of the current delimiter char to check against */
        while (reader.Peek() >= 0)
        {
            char c = (char)reader.Read();

            if (c == delimiter[d]) /* If the char matches the current delimiter char */
            {
                d++;
                if (d == delimiter.Length) /* If all the delimiter char's were found, add the word and continue */
                {
                    d = 0; /* Reset the delimiter index */
                    yield return new String(chars.ToArray());
                    chars.Clear();
                    continue;
                }

            }
            else if (d > 0) /* If the delimiter match failed, retroactively add those char's */
            {
                for (int i = 0; i < d; i++)
                    chars.Add(delimiter[i]);
                d = 0;
                chars.Add(c);
            }
            else
                chars.Add(c);
        }
    }

    public static bool ContainsChars(this string word, string letters)
    {
        string lowered = word.ToLower();
        foreach (char letter in letters)
        {
            if (!lowered.Contains(Char.ToLower(letter)))
                return false;
        }
        return true;
    }
}
