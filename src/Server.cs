using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeCrafters.DnsServer.Classes;
using Codecrafters.DnsServer.Enums;
using CodeCrafters.DnsServer.Extensions;
using Type = CodeCrafters.DnsServer.Classes.Type;

namespace Codecrafters.DnsServer;

internal abstract class Program
{
    static void Main(string[] args)
    {
        if (args[0] == "local")
        {
            /*Header emptyHeader = new()
            {
                PacketIdentifier = 1234,
                QueryResponseIndicator = QueryResponseIndicator.Question,
                QuestionCount = 3,
                AnswerCount = 1,
                AuthorityCount = 10,
                AdditionalCount = 16,
                Opcode = OpCode.Query,
                AuthoritativeAnswer= true,
                Truncated = true,
                RecursionDesired = true,
                RecursionAvailable = true,
                ResponseCode = ResponseCode.YxRrSet,
            };
            byte[] response = emptyHeader.ToBytes();
            Console.WriteLine(response.ToHexString());
            return;*/
/*
            Question question = new()
            {
                Name = "google.com",
                Class = Class.Internet,
                Type = Type.HostAddress
            };
            byte[] response = question.ToBytes();
            Console.WriteLine(response.ToHexString());
            return;
            byte[] query = [0x81,0x62,0x01,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x0c,
                            0x63,0x6f,0x64,0x65,0x63,0x72,0x61,0x66,0x74,0x65,0x72,0x73,0x02,
                            0x69,0x6f,0x00,0x00,0x01,0x00,0x01];
            Header incomingHeader = Header.FromBytes(query[..12]);
            int index = 12;
            Question incomingQuestion = Question.FromBytes(query, ref index);
            Console.WriteLine(JsonSerializer.Serialize(incomingHeader));
            Console.WriteLine(JsonSerializer.Serialize(incomingQuestion));*/

            Answer answer = new Answer()
            {
                Address = IPAddress.Parse("127.1.2.3"),
                Class = Class.Internet,
                Length = 4,
                Name = "codecrafters.io",
                TimeToLive = 4,
                Type = Type.HostAddress
            };
            Console.WriteLine(answer.ToBytes().ToHexString());

            return;
        }
        
        
        // Resolve UDP address
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 2053;
        IPEndPoint udpEndPoint = new IPEndPoint(ipAddress, port);

        // Create UDP socket
        UdpClient udpClient = new UdpClient(udpEndPoint);

        while (true)
        {
            // Receive data
            IPEndPoint sourceEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref sourceEndPoint);
            string receivedString = Encoding.ASCII.GetString(receivedData);

            
            Console.WriteLine($"Received {receivedData.Length} bytes from {sourceEndPoint}: {receivedString}");
    
            Header incomingHeader = Header.FromBytes(receivedData[..12]);
            int index = 12;
            Question incomingQuestion = Question.FromBytes(receivedData, ref index);
            
            // Create an empty response
            Header emptyHeader = new()
            {
                PacketIdentifier = 1234,
                QueryResponseIndicator = QueryResponseIndicator.Reply,
                QuestionCount = 1,
                AnswerCount = 1
            };

            Answer answerBytes = new Answer()
            {
                Name = incomingQuestion.Name,
                Address = IPAddress.Parse("127.0.0.1"),
                Class = incomingQuestion.Class,
                Length = 4,
                TimeToLive = 40,
                Type = incomingQuestion.Type
            };
            
            byte[] response = [..emptyHeader.ToBytes(),.. incomingQuestion.ToBytes(), .. answerBytes.ToBytes()];
            
            // Send response
            udpClient.Send(response, response.Length, sourceEndPoint);
        }

    }
}