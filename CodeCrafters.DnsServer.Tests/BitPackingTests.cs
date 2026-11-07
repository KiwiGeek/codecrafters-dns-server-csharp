using CodeCrafters.DnsServer.Extensions;

namespace CodeCrafters.DnsServer.Tests;

[TestClass]
public class BitPackingTests
{
    // -------------------------------------------------------
    // Single-bit operations
    // -------------------------------------------------------

    [TestMethod]
    public void SetBits_FirstBit_SetsMostSignificantBit()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(0, 1, 1);

        CollectionAssert.AreEqual(
            new byte[] { 0x80 }, buffer);
    }

    [TestMethod]
    public void SetBits_LastBit_SetsLeastSignificantBit()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(7, 1, 1);

        CollectionAssert.AreEqual(
            new byte[] { 0x01 }, buffer);
    }

    [TestMethod]
    public void SetBits_FirstBitOfSecondByte()
    {
        byte[] buffer = "\0\0"u8.ToArray();

        buffer.SetLtoRBits(8, 1, 1);

        CollectionAssert.AreEqual(
            new byte[] { 0x00, 0x80 }, buffer);
    }

    // -------------------------------------------------------
    // Multiple bits within one byte
    // -------------------------------------------------------

    [TestMethod]
    public void SetBits_ThreeBits_AtBeginning()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(0, 3, 0b101);

        CollectionAssert.AreEqual(
            new byte[] { 0b1010_0000 }, buffer);
    }

    [TestMethod]
    public void SetBits_ThreeBits_InMiddle()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(2, 3, 0b110);

        CollectionAssert.AreEqual(
            new byte[] { 0b0011_0000 }, buffer);
    }

    [TestMethod]
    public void SetBits_FourBits_AtEnd()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(4, 4, 0b1011);

        CollectionAssert.AreEqual(
            new byte[] { 0x0B }, buffer);
    }

    [TestMethod]
    public void SetBits_EntireByte()
    {
        byte[] buffer = [0x00];

        buffer.SetLtoRBits(0, 8, 0xA5);

        CollectionAssert.AreEqual(
            new byte[] { 0xA5 }, buffer);
    }

    // -------------------------------------------------------
    // Crossing byte boundaries
    // -------------------------------------------------------

    [TestMethod]
    public void SetBits_CrossesByteBoundary()
    {
        byte[] buffer = "\0\0"u8.ToArray();

        buffer.SetLtoRBits(6, 4, 0b1011);

        CollectionAssert.AreEqual(
            new byte[] { 0x02, 0xC0 }, buffer);
    }

    [TestMethod]
    public void SetBits_CrossesMultipleBytes()
    {
        byte[] buffer = "\0\0\0"u8.ToArray();

        buffer.SetLtoRBits(4, 16, 0xABCD);

        CollectionAssert.AreEqual(
            new byte[] { 0x0A, 0xBC, 0xD0 }, buffer);
    }

    [TestMethod]
    public void SetBits_EntireSixteenBitValue()
    {
        byte[] buffer = "\0\0"u8.ToArray();

        buffer.SetLtoRBits(0, 16, 0x1234);

        CollectionAssert.AreEqual(
            new byte[] { 0x12, 0x34 }, buffer);
    }

    [TestMethod]
    public void SetBits_EntireThirtyTwoBitValue()
    {
        byte[] buffer = new byte[4];

        buffer.SetLtoRBits(0, 32, 0xDEADBEEF);

        CollectionAssert.AreEqual(
            new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
            buffer);
    }

    // -------------------------------------------------------
    // Preserving existing bits
    // -------------------------------------------------------

    [TestMethod]
    public void SetBits_PreservesSurroundingBits()
    {
        byte[] buffer = [0xFF];

        buffer.SetLtoRBits(2, 3, 0);

        CollectionAssert.AreEqual(
            new byte[] { 0b1100_0111 }, buffer);
    }

    [TestMethod]
    public void SetBits_ReplacesExistingValue()
    {
        byte[] buffer = [0xFF];

        buffer.SetLtoRBits(2, 3, 0b010);

        CollectionAssert.AreEqual(
            new byte[] { 0b1101_0111 }, buffer);
    }

    [TestMethod]
    public void SetBits_DoesNotModifyAdjacentBytes()
    {
        byte[] buffer = [0xAA, 0x00, 0x55];

        buffer.SetLtoRBits(8, 8, 0xFF);

        CollectionAssert.AreEqual(
            new byte[] { 0xAA, 0xFF, 0x55 },
            buffer);
    }

    [TestMethod]
    public void SetBits_SequentialWrites_DoNotInterfere()
    {
        byte[] buffer = "\0\0"u8.ToArray();

        buffer.SetLtoRBits(0, 1, 1);
        buffer.SetLtoRBits(1, 4, 0b0010);
        buffer.SetLtoRBits(5, 1, 1);
        buffer.SetLtoRBits(6, 1, 0);
        buffer.SetLtoRBits(7, 1, 1);

        CollectionAssert.AreEqual(
            new byte[] { 0b1001_0101, 0x00 },
            buffer);
    }

    // -------------------------------------------------------
    // Reading bits
    // -------------------------------------------------------

    [TestMethod]
    public void GetBits_ReadsFirstBit()
    {
        byte[] buffer = [0x80];

        Assert.AreEqual(1u, buffer.GetBits(0, 1));
    }

    [TestMethod]
    public void GetBits_ReadsLastBit()
    {
        byte[] buffer = [0x01];

        Assert.AreEqual(1u, buffer.GetBits(7, 1));
    }

    [TestMethod]
    public void GetBits_ReadsMiddleField()
    {
        byte[] buffer = [0b1011_0101];

        Assert.AreEqual(
            0b110u,
            buffer.GetBits(2, 3));
    }

    [TestMethod]
    public void GetBits_ReadsAcrossByteBoundary()
    {
        byte[] buffer = [0x02, 0xC0];

        Assert.AreEqual(
            0b1011u,
            buffer.GetBits(6, 4));
    }

    [TestMethod]
    public void GetBits_ReadsSixteenBitValue()
    {
        byte[] buffer = [0x12, 0x34];

        Assert.AreEqual(
            0x1234u,
            buffer.GetBits(0, 16));
    }

    [TestMethod]
    public void GetBits_ReadsThirtyTwoBitValue()
    {
        byte[] buffer = [0xDE, 0xAD, 0xBE, 0xEF];

        Assert.AreEqual(
            0xDEADBEEFu,
            buffer.GetBits(0, 32));
    }

    // -------------------------------------------------------
    // Round-trip tests
    // -------------------------------------------------------

    [TestMethod]
    public void RoundTrip_ThreeBitValue()
    {
        byte[] buffer = new byte[2];

        buffer.SetLtoRBits(5, 3, 0b101);

        Assert.AreEqual(
            0b101u,
            buffer.GetBits(5, 3));
    }

    [TestMethod]
    public void RoundTrip_ArbitraryFieldCrossingBytes()
    {
        byte[] buffer = new byte[4];

        buffer.SetLtoRBits(5, 17, 0x1ABCD);

        Assert.AreEqual(
            0x1ABCDu,
            buffer.GetBits(5, 17));
    }

    [TestMethod]
    public void RoundTrip_AllPossibleFourBitValues()
    {
        for (uint value = 0; value < 16; value++)
        {
            byte[] buffer = new byte[2];

            buffer.SetLtoRBits(6, 4, value);

            Assert.AreEqual(
                value,
                buffer.GetBits(6, 4));
        }
    }

    // -------------------------------------------------------
    // DNS-specific tests
    // -------------------------------------------------------

    [TestMethod]
    public void Dns_OpcodeTwo_EncodesCorrectly()
    {
        byte[] buffer = new byte[12];

        buffer.SetLtoRBits(17, 4, 2);

        Assert.AreEqual((byte)0x10, buffer[2]);
        Assert.AreEqual((byte)0x00, buffer[3]);
    }

    [TestMethod]
    public void Dns_StandardRecursiveQuery_EncodesCorrectly()
    {
        byte[] buffer = new byte[12];

        buffer.SetLtoRBits(16, 1, 0); // QR
        buffer.SetLtoRBits(17, 4, 0); // Opcode
        buffer.SetLtoRBits(23, 1, 1); // RD

        Assert.AreEqual((byte)0x01, buffer[2]);
        Assert.AreEqual((byte)0x00, buffer[3]);
    }

    [TestMethod]
    public void Dns_ResponseCode_EncodesCorrectly()
    {
        byte[] buffer = new byte[12];

        buffer.SetLtoRBits(28, 4, 3);

        Assert.AreEqual((byte)0x03, buffer[3]);
    }

    [TestMethod]
    public void Dns_OpcodeAndResponseCode_DoNotInterfere()
    {
        byte[] buffer = new byte[12];

        buffer.SetLtoRBits(17, 4, 2);
        buffer.SetLtoRBits(28, 4, 3);

        Assert.AreEqual(2u, buffer.GetBits(17, 4));
        Assert.AreEqual(3u, buffer.GetBits(28, 4));

        Assert.AreEqual((byte)0x10, buffer[2]);
        Assert.AreEqual((byte)0x03, buffer[3]);
    }

    // -------------------------------------------------------
    // Invalid arguments and boundary conditions
    // -------------------------------------------------------

    [TestMethod]
    public void SetBits_RejectsNegativeStartPosition()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.SetLtoRBits(-1, 3, 1));
    }

    [TestMethod]
    public void SetBits_RejectsZeroBitCount()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.SetLtoRBits(0, 0, 0));
    }

    [TestMethod]
    public void SetBits_RejectsMoreThanThirtyTwoBits()
    {
        byte[] buffer = new byte[8];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.SetLtoRBits(0, 33, 1));
    }

    [TestMethod]
    public void SetBits_RejectsWritingPastBufferEnd()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.SetLtoRBits(14, 4, 3));
    }

    [TestMethod]
    public void SetBits_RejectsValueThatDoesNotFit()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.SetLtoRBits(0, 3, 8));
    }

    [TestMethod]
    public void SetBits_AllowsMaximumValueForField()
    {
        byte[] buffer = new byte[2];

        buffer.SetLtoRBits(0, 3, 7);

        Assert.AreEqual((byte)0xE0, buffer[0]);
    }

    [TestMethod]
    public void SetBits_AllowsMaximumThirtyTwoBitValue()
    {
        byte[] buffer = new byte[4];

        buffer.SetLtoRBits(0, 32, uint.MaxValue);

        CollectionAssert.AreEqual(
            new byte[] { 0xFF, 0xFF, 0xFF, 0xFF },
            buffer);
    }

    [TestMethod]
    public void GetBits_RejectsReadingPastBufferEnd()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.GetBits(15, 2));
    }

    [TestMethod]
    public void GetBits_RejectsZeroBitCount()
    {
        byte[] buffer = new byte[2];

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => buffer.GetBits(0, 0));
    }

    [TestMethod]
    public void SetBits_NullBuffer_Throws()
    {
        byte[] buffer = null!;

        Assert.ThrowsExactly<ArgumentNullException>(
            () => buffer.SetLtoRBits(0, 1, 1));
    }

    [TestMethod]
    public void GetBits_NullBuffer_Throws()
    {
        byte[] buffer = null!;

        Assert.ThrowsExactly<ArgumentNullException>(
            () => buffer.GetBits(0, 1));
    }
}