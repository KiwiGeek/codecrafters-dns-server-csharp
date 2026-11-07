using System.Text;
using CodeCrafters.DnsServer.Extensions;

namespace CodeCrafters.DnsServer.Classes;

public class Question
{
    
    public required string Name { get; init; }
    public required Type Type { get; init; }
    public required Class Class { get; init; }
    
    public byte[] ToBytes()
    {
        // the length of the label is: [length of string + 1 + number of dots]
        // count the dots in Name
        int numberOfDots = Name.Count(f => f == '.');
        int lengthOfLabel = Name.Length + numberOfDots + 1;
        byte[] responseBytes = new byte[lengthOfLabel+4];
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
        return responseBytes;
    }

    public static Question FromBytes(byte[] bytes, ref int index)
    {
        List<byte> result = [];
        bool done = false;
        StringBuilder builder = new();
        while (!done)
        {
            // the next digit is a length of string
            int strLength = bytes[index];
            builder.Append(Encoding.UTF8.GetString(bytes, index+1, strLength));
            index += strLength + 1;
            // peek the next char. If it's null we're done.
            done = bytes[index] == 0;
            if (!done) builder.Append('.');
        }

        index++;        // skip null bit.
        Type type = (Type)bytes.GetBits((index*8), 16);
        Class @class = (Class)bytes.GetBits((index*8) + 16, 16);
        index += 4;
        return new Question()
        {
            Name = builder.ToString(),
            Type = type,
            Class = @class
        };
    }
}