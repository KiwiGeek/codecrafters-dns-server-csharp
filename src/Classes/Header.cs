using Codecrafters.DnsServer.Enums;
using CodeCrafters.DnsServer.Extensions;

namespace CodeCrafters.DnsServer.Classes;

public readonly struct Header
{
    public ushort PacketIdentifier { get; init; }
    public QueryResponseIndicator QueryResponseIndicator { get; init; }
    public OpCode Opcode { get; init; }
    public bool AuthoritativeAnswer { get; init; }
    public bool Truncated { get; init; }
    public bool RecursionDesired { get; init; }
    public bool RecursionAvailable { get; init; }
    public ResponseCode ResponseCode { get; init; }
    public ushort QuestionCount { get; init; }
    public ushort AnswerCount { get; init; }
    public ushort AuthorityCount { get; init; }
    public ushort AdditionalCount { get; init; }

    public byte[] ToBytes()
    {
        byte[] byteResult = new byte[12];
        byteResult.SetLtoRBits(0,16, PacketIdentifier);
        byteResult.SetLtoRBits(16,1, (ushort)QueryResponseIndicator);
        byteResult.SetLtoRBits(17,4, (ushort)Opcode);
        byteResult.SetLtoRBits(21, 1, AuthoritativeAnswer);
        byteResult.SetLtoRBits(22, 1, Truncated);
        byteResult.SetLtoRBits(23, 1, RecursionDesired);
        byteResult.SetLtoRBits(24, 1, RecursionAvailable);
        byteResult.SetLtoRBits(25, 3, 0);           // reserved
        byteResult.SetLtoRBits(28, 4, (ushort)ResponseCode);
        byteResult.SetLtoRBits(32, 16, QuestionCount);
        byteResult.SetLtoRBits(48, 16,AnswerCount);
        byteResult.SetLtoRBits(64, 16,AuthorityCount);
        byteResult.SetLtoRBits(80, 16, AdditionalCount);
        return byteResult;
    }
//https://gizza.ai/tools/dns-message-parser/
    public static Header FromBytes(byte[] bytes)
    {
        return new Header()
        {
            PacketIdentifier = (ushort)bytes.GetBits(0, 16),
            QueryResponseIndicator = (QueryResponseIndicator)bytes.GetBits(16, 1),
            Opcode = (OpCode)bytes.GetBits(17, 4),
            AuthoritativeAnswer =  Convert.ToBoolean(bytes.GetBits(21, 1)),
            Truncated =  Convert.ToBoolean(bytes.GetBits(22, 1)),
            RecursionDesired = Convert.ToBoolean(bytes.GetBits(23, 1)),
            RecursionAvailable =  Convert.ToBoolean(bytes.GetBits(24, 1)),
            ResponseCode = (ResponseCode)bytes.GetBits(28, 4),
            QuestionCount = (ushort)bytes.GetBits(32, 16),
            AnswerCount = (ushort)bytes.GetBits(48, 16),
            AuthorityCount = (ushort)bytes.GetBits(64, 16),
            AdditionalCount = (ushort)bytes.GetBits(80, 16),
        };
    }
    
    

}