using System.Net;
using System.Text;

namespace CodeCrafters.DnsServer.Classes;

public class Answer
{
    public required string Name { get; init; }
    public required Type Type { get; init; }
    public required Class Class { get; init; }
    public required uint TimeToLive { get; init; }
    public required ushort Length { get; init; }
    public required IPAddress Address { get; init; }

    public byte[] ToBytes()
    {
        int lengthOfLabel = Name.Length + 2;
        byte[] responseBytes = new byte[lengthOfLabel+14];
        int i = 0;
        foreach (string s in Name.Split('.'))
        {
            responseBytes[i] = (byte)s.Length;
            Encoding.UTF8.GetBytes(s).CopyTo(responseBytes, i+1);
            i += s.Length+1;
        }
        responseBytes[lengthOfLabel-1] = 0;
        responseBytes[lengthOfLabel] = (byte)((ushort)Type >> 8);
        responseBytes[lengthOfLabel+1] = (byte)Type;
        responseBytes[lengthOfLabel+2] = (byte)((ushort)Class >> 8);
        responseBytes[lengthOfLabel+3] = (byte)Class;
        responseBytes[lengthOfLabel + 4] = (byte)(TimeToLive >> 24);
        responseBytes[lengthOfLabel + 5] = (byte)(TimeToLive >> 16);
        responseBytes[lengthOfLabel + 6] = (byte)(TimeToLive >> 8);
        responseBytes[lengthOfLabel + 7] = (byte)(TimeToLive);
        responseBytes[lengthOfLabel + 8] = (byte)(Length >> 8);
        responseBytes[lengthOfLabel + 9] = (byte)Length;
        responseBytes[lengthOfLabel + 10] = Address.GetAddressBytes()[0];
        responseBytes[lengthOfLabel + 11] = Address.GetAddressBytes()[1];
        responseBytes[lengthOfLabel + 12] = Address.GetAddressBytes()[2];
        responseBytes[lengthOfLabel + 13] = Address.GetAddressBytes()[3];
        return responseBytes;
    }

    public static Answer FromBytes(byte[] bytes, ref int index)
    {
        bool done = false;
        StringBuilder builder = new();
        while (!done)
        {
            int strLength = bytes[index];
            builder.Append(Encoding.UTF8.GetString(bytes, index+1, strLength));
            index += strLength + 1;
            done = bytes[index] == 0;
            if (!done) builder.Append('.');
        }
        string name = builder.ToString();
        Type type = (Type)(bytes[index+1] * 256 + bytes[index+2]);
        Class @class = (Class)(bytes[index+3] * 256 + bytes[index+4]);
        uint timeToLive =  (uint)(bytes[index+5] * 256 * 256 * 256 
                                  + bytes[index+6] * 256 * 256
                                  + bytes[index+7] * 256
                                  + bytes[index+8]);
        ushort length = (ushort)(bytes[index+9] * 256 + bytes[index+10]);
        IPAddress address = new([bytes[index + 11], bytes[index + 12], bytes[index + 13], bytes[index + 14]]);

        return new Answer()
        {
            Name = name,
            Type = type,
            TimeToLive = timeToLive,
            Address = address,
            Class = @class,
            Length = length
        };
    }
}