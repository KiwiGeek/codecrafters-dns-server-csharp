namespace CodeCrafters.DnsServer.Extensions;

public static class ByteArrayExtensions
{
    public static string ToHexString(this byte[] bytes) => Convert.ToHexStringLower(bytes);
    public static string ToUrlEncodedString(this byte[] bytes) => string.Concat(bytes.Select(b => $"%{b:X2}"));

    public static byte[][] ToChunks(this byte[] bytes, int chunkSize)
    {
        List<byte[]> results = [];
        int index = -chunkSize;
        while (index < bytes.Length)
        {
            results.Add(bytes.Skip(index+=chunkSize).Take(chunkSize).ToArray());
        }
        return results.ToArray();
    }
    
    public static byte[] SetBitMsbFirst(this byte[] self, int index, bool value)
    {
        int byteIndex = index / 8;
        int bitIndex = 7 - (index % 8);
        byte mask = (byte)(1 << bitIndex);

        self[byteIndex] = (byte)(value
            ? self[byteIndex] | mask
            : self[byteIndex] & ~mask);

        return self;
    }
    
    public static byte[] SetBit(this byte[] self, int index, bool value)
    {
        int byteIndex = index / 8;
        int bitIndex = index % 8;
        byte mask = (byte)(1 << bitIndex);

        self[byteIndex] = (byte)(value ? (self[byteIndex] | mask) : (self[byteIndex] & ~mask));
        return self;
    }

    public static byte[] ToggleBit(this byte[] self, int index)
    {
        int byteIndex = index / 8;
        int bitIndex = index % 8;
        byte mask = (byte)(1 << bitIndex);

        self[byteIndex] ^= mask;
        return self;
    }

    public static bool GetBit(this byte[] self, int index)
    {
        int byteIndex = index / 8;
        int bitIndex = index % 8;
        byte mask = (byte)(1 << bitIndex);

        return (self[byteIndex] & mask) != 0;
    }
}