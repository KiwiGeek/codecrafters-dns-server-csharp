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
       /* if (args[0] == "local")
        {
            Header emptyHeader = new()
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
            Header incomingHeader = Header.FromBytes(query[..12]);
            int index = 12;
            Question incomingQuestion = Question.FromBytes(query, ref index);
            Console.WriteLine(JsonSerializer.Serialize(incomingHeader));
            Console.WriteLine(JsonSerializer.Serialize(incomingQuestion));

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
            byte[] query = [0x02, 0x8d, 0x01, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x61, 0x62, 
                            0x63, 0x11, 0x6c, 0x6f, 0x6e, 0x67, 0x61, 0x73, 0x73, 0x64, 0x6f, 0x6d, 0x61, 0x69, 0x6e, 
                            0x6e, 0x61, 0x6d, 0x65, 0x03, 0x63, 0x6f, 0x6d, 0x00, 0x00, 0x01, 0x00, 0x01, 0x03, 0x64, 
                            0x65, 0x66, 0xc0, 0x10, 0x00, 0x01, 0x00, 0x01];
            Header incomingHeader = Header.FromBytes(query[..12]);
            int index = 12;
            List<Question> incomingQuestions = [];
            Console.WriteLine(JsonSerializer.Serialize(incomingHeader));
            while (index < query.Length)
            {
                Question incomingQuestion = Question.FromBytes(query, ref index);
                incomingQuestions.Add(incomingQuestion);
            }
            Console.WriteLine(JsonSerializer.Serialize(incomingQuestions));
            return;
        }
        */
        
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
            Header incomingHeader = Header.FromBytes(receivedData[..12]);
            int index = 12;
            List<Question> incomingQuestions = [];
            while (index < receivedData.Length)
            {
                Question incomingQuestion = Question.FromBytes(receivedData, ref index);
                incomingQuestions.Add(incomingQuestion);
            }
            
            // Create an empty response
            Header responseHeader = new()
            {
                PacketIdentifier = incomingHeader.PacketIdentifier,
                QueryResponseIndicator = QueryResponseIndicator.Reply,
                QuestionCount = (ushort)incomingQuestions.Count,
                AnswerCount = (ushort)incomingQuestions.Count,
                Opcode = incomingHeader.Opcode,
                AuthoritativeAnswer = false,
                Truncated = false,
                RecursionDesired = incomingHeader.RecursionDesired,
                RecursionAvailable = false, 
                ResponseCode = incomingHeader.Opcode == OpCode.Query ? ResponseCode.NoError : ResponseCode.NotImplemented,
                AuthorityCount = 0,
                AdditionalCount = 0
            };
            List<byte> response = [.. responseHeader.ToBytes()];
            List<byte> questions = [];
            List<byte> answers = [];
            foreach (Question incomingQuestion in incomingQuestions)
            {
                Answer answerBytes = new Answer()
                {
                    Name = incomingQuestion.Name,
                    Address = IPAddress.Parse("127.0.0.1"),
                    Class = incomingQuestion.Class,
                    Length = 4,
                    TimeToLive = 40,
                    Type = incomingQuestion.Type
                };
                questions.AddRange(incomingQuestion.ToBytes());
                answers.AddRange(answerBytes.ToBytes());
            }
            response.AddRange(questions);
            response.AddRange(answers);
            udpClient.Send(response.ToArray(), response.ToArray().Length, sourceEndPoint);
        }

    }
}