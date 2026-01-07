using System;

public static class VersionUtils
{
    /// <summary>
    /// Vergleicht zwei Versionsstrings.
    /// Gibt -1 zurück, wenn v1 &lt; v2, 0 wenn gleich, 1 wenn v1 &gt; v2
    /// </summary>
    public static int CompareVersions(string v1, string v2)
    {
        // Trenne evtl. Suffix (z.B. -beta)
        string[] v1Parts = v1.Split('-');
        string[] v2Parts = v2.Split('-');

        string[] numbers1 = v1Parts[0].Split('.');
        string[] numbers2 = v2Parts[0].Split('.');

        int length = Math.Max(numbers1.Length, numbers2.Length);

        for (int i = 0; i < length; i++)
        {
            int num1 = i < numbers1.Length ? int.Parse(numbers1[i]) : 0;
            int num2 = i < numbers2.Length ? int.Parse(numbers2[i]) : 0;

            if (num1 < num2) return -1;
            if (num1 > num2) return 1;
        }

        // Wenn numerische Teile gleich sind, dann Suffix berücksichtigen
        // z.B. "1.0.0-beta" < "1.0.0"
        bool hasSuffix1 = v1Parts.Length > 1;
        bool hasSuffix2 = v2Parts.Length > 1;

        if (hasSuffix1 && !hasSuffix2) return -1;
        if (!hasSuffix1 && hasSuffix2) return 1;

        if (hasSuffix1 && hasSuffix2)
        {
            // Optional: lexikografisch vergleichen
            return string.Compare(v1Parts[1], v2Parts[1], StringComparison.Ordinal);
        }

        return 0; // komplett gleich
    }

    /// <summary>
    /// Einfacher Bool Vergleich: v1 < v2
    /// </summary>
    public static bool IsLess(string v1, string v2)
    {
        return CompareVersions(v1, v2) < 0;
    }

    /// <summary>
    /// Einfacher Bool Vergleich: v1 > v2
    /// </summary>
    public static bool IsGreater(string v1, string v2)
    {
        return CompareVersions(v1, v2) > 0;
    }
}

