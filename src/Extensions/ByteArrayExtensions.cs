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
    
    public static uint GetBits(
        this byte[] buffer,
        int startBit,
        int bitCount)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        if (bitCount is < 1 or > 32)
            throw new ArgumentOutOfRangeException(nameof(bitCount));

        if (startBit < 0 ||
            (long)startBit + bitCount > (long)buffer.Length * 8)
            throw new ArgumentOutOfRangeException(nameof(startBit));

        uint result = 0;

        for (int i = 0; i < bitCount; i++)
        {
            int position = startBit + i;
            int byteIndex = position / 8;
            int bitIndex = 7 - (position % 8);

            bool isSet = (buffer[byteIndex] & (1 << bitIndex)) != 0;

            result = (result << 1) | (isSet ? 1u : 0u);
        }

        return result;
    }


    public static void SetLtoRBits(this byte[] buffer, int startBit, int bitCount, uint value)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(bitCount,32);
        ArgumentOutOfRangeException.ThrowIfNegative(startBit);
        ArgumentOutOfRangeException.ThrowIfZero(bitCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan((long)startBit + bitCount , (long)buffer.Length * 8);
        if (bitCount < 32 && (value >> bitCount) != 0) throw new ArgumentOutOfRangeException(nameof(value), "Value cannot fit in the specified number of bits.");
        // Write each bit into its destination.
        for (int i = 0; i < bitCount; i++)
        {
            int byteIndex = (startBit + i) / 8;
            int bitIndex = 7 - ((startBit + i) % 8);
            byte mask = (byte)(1 << bitIndex);
            if ((value & (1u << (bitCount - 1 - i))) != 0)
                buffer[byteIndex] |= mask;
            else
                buffer[byteIndex] &= (byte)~mask;
        }
    }
    
    public static void SetLtoRBits(this byte[] buffer, int startBit, int bitCount, bool value)
    {
        SetLtoRBits(buffer, startBit, bitCount, Convert.ToUInt32(value));
    }
    
    public static void SetLtoRBits(this byte[] buffer, int startBit, int bitCount, ushort value)
    {
        SetLtoRBits(buffer, startBit, bitCount, Convert.ToUInt32(value));
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