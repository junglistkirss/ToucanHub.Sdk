namespace ToucanHub.Sdk.Utils;

public static class FileSizeExtensions
{
    private static readonly string[] Extensions = ["B", "kB", "MB", "GB", "TB", "PB", "EB"];

    public static string ToReadableSize(this int value, int precision = 0) => ((long)value).ToReadableSize(precision);

    public static string ToReadableSize(this long value, int precision = 0)
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

        return $"{Math.Round(d, precision).ToString(CultureInfo.InvariantCulture)} {Extensions[u]}";
    }
}
