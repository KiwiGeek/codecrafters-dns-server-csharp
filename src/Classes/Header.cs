using System.Buffers.Binary;
using CodeCrafters.DnsServer.Extensions;
using CodeCrafters.DnsServer.Extensions.Enums;

namespace CodeCrafters.DnsServer.Classes;

public struct Header
{
    public ushort PacketIdentifier { get; set; }
    public QueryResponseIndicator QueryResponseIndicator { get; set; }

    public byte[] ToBytes()
    {
        byte[] byteResult = new byte[96];
        BinaryPrimitives.WriteUInt16BigEndian(byteResult.AsSpan(0, 2), PacketIdentifier);
        byteResult.SetBitMsbFirst(16, QueryResponseIndicator == QueryResponseIndicator.Reply);
        return byteResult;
    }

}