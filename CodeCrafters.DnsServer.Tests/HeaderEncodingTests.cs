using CodeCrafters.DnsServer.Classes;
using Codecrafters.DnsServer.Enums;

namespace CodeCrafters.DnsServer.Tests;

[TestClass]
public class HeaderEncodingTests
{
    [TestMethod]
    public void EmptyHeader_AllBytesZero()
    {
        Header header = new();

        AssertHex("000000000000000000000000", header);
    }

    [TestMethod]
    public void StandardQuery_RecursionDesired()
    {
        Header header = new()
        {
            PacketIdentifier = 0x1234,
            QueryResponseIndicator = QueryResponseIndicator.Question,
            Opcode = OpCode.Query,
            RecursionDesired = true,
            QuestionCount = 1
        };

        AssertHex("123401000001000000000000", header);
    }

    [TestMethod]
    public void StandardResponse_RecursionAvailable()
    {
        Header header = new()
        {
            PacketIdentifier = 0x1234,
            QueryResponseIndicator = QueryResponseIndicator.Reply,
            Opcode = OpCode.Query,
            RecursionDesired = true,
            RecursionAvailable = true,
            QuestionCount = 1,
            AnswerCount = 1
        };

        AssertHex("123481800001000100000000", header);
    }

    [TestMethod]
    public void AuthoritativeResponse()
    {
        Header header = new()
        {
            QueryResponseIndicator = QueryResponseIndicator.Reply,
            AuthoritativeAnswer = true,
            QuestionCount = 1,
            AnswerCount = 2
        };

        AssertHex("000084000001000200000000", header);
    }

    [TestMethod]
    public void TruncatedResponse()
    {
        Header header = new()
        {
            QueryResponseIndicator = QueryResponseIndicator.Reply,
            Truncated = true
        };

        AssertHex("000082000000000000000000", header);
    }

    [TestMethod]
    public void AllCommonFlagsEnabled()
    {
        Header header = new()
        {
            QueryResponseIndicator = QueryResponseIndicator.Reply,
            AuthoritativeAnswer = true,
            Truncated = true,
            RecursionDesired = true,
            RecursionAvailable = true
        };

        AssertHex("000087800000000000000000", header);
    }

    [TestMethod]
    public void ResponseCode_YxRrSet()
    {
        Header header = new()
        {
            QueryResponseIndicator = QueryResponseIndicator.Reply,
            ResponseCode = ResponseCode.YxRrSet
        };

        AssertHex("000080070000000000000000", header);
    }

    [TestMethod]
    public void Counts_AreBigEndian()
    {
        Header header = new()
        {
            PacketIdentifier = 0xABCD,
            QuestionCount = 0x1234,
            AnswerCount = 0x5678,
            AuthorityCount = 0x9ABC,
            AdditionalCount = 0xDEF0
        };

        AssertHex("ABCD0000123456789ABCDEF0", header);
    }

    [TestMethod]
    public void Counts_AtMaximumValues()
    {
        Header header = new()
        {
            PacketIdentifier = ushort.MaxValue,
            QuestionCount = ushort.MaxValue,
            AnswerCount = ushort.MaxValue,
            AuthorityCount = ushort.MaxValue,
            AdditionalCount = ushort.MaxValue
        };

        AssertHex("FFFF0000FFFFFFFFFFFFFFFF", header);
    }

    [TestMethod]
    public void MixedFlagsAndCounts()
    {
        Header header = new()
        {
            PacketIdentifier = 1234,
            QueryResponseIndicator = QueryResponseIndicator.Question,
            QuestionCount = 3,
            AnswerCount = 1,
            AuthorityCount = 10,
            AdditionalCount = 16,
            Opcode = OpCode.Query,
            AuthoritativeAnswer = true,
            Truncated = true,
            RecursionDesired = true,
            RecursionAvailable = true,
            ResponseCode = ResponseCode.YxRrSet
        };

        AssertHex("04D2078700030001000A0010", header);
    }

    private static void AssertHex(string expected, Header header)
    {
        byte[] actual = header.ToBytes();

        Assert.HasCount(12, actual);
        Assert.AreEqual(expected, Convert.ToHexString(actual));
    }
}