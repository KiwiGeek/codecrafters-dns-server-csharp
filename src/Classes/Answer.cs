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
}