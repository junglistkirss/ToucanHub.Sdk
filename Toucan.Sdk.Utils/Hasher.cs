using System.Security.Cryptography;
using System.Text;

namespace Toucan.Sdk.Utils;

public static class Hasher
{
    public static string Hash(this Guid uid) => uid.ToString().ToSHA256();

    public static string New()
    {
        return Random().ToBytesHashed(SHA256.HashData);
    }

    public static string Randomize(object value) => Randomize(value.ToString()!);

    public static string Randomize(string value)
    {
        byte[] entropy = [.. Random(), .. SHA512.HashData(Encoding.UTF8.GetBytes(value))];
        return entropy.ToBytesHashed(SHA256.HashData);
    }

    private static byte[] Random()
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator? randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return randomNumber;
    }

    public static string Simple() => Guid.NewGuid().ToString().ToSHA256();

    public static string ToSHA1(this string value) => value.ToHashed(SHA1.HashData);

    public static string ToSHA256(this string value) => value.ToHashed(SHA256.HashData);

    public static string ToSHA512(this string value) => value.ToHashed(SHA512.HashData);

    public static string ToMD5(this string value) => value.ToHashed(MD5.HashData);

    private delegate byte[] HashProducer(byte[] data);

    private static string ToHashed(this string value, HashProducer producer) => Encoding.UTF8.GetBytes(value).ToBytesHashed(producer);

    private static string ToBytesHashed(this byte[] bytes, HashProducer producer)
    {
        byte[]? bytesHash = producer(bytes);
        return Convert.ToHexString(bytesHash);
    }
}
