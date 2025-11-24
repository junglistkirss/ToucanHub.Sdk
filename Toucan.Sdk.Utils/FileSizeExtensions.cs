namespace Toucan.Sdk.Utils;

public static class FileSizeExtensions
{
    private static readonly string[] Extensions = ["B", "kB", "MB", "GB"];

    public static string ToReadableSize(this int value) => ((long)value).ToReadableSize();

    public static string ToReadableSize(this long value)
    {
        if (value < 0)
            return string.Empty;

        double d = value;
        int u = 0;

        const int multiplier = 1024;

        while ((d >= multiplier || -d >= multiplier) && u < Extensions.Length - 1)
        {
            d /= multiplier;
            u++;
        }

        if (u >= Extensions.Length - 1)
            u = Extensions.Length - 1;

        return $"{Math.Round(d, 1).ToString(CultureInfo.InvariantCulture)} {Extensions[u]}";
    }
}
