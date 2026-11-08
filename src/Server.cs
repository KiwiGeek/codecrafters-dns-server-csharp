using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using CodeCrafters.DnsServer.Classes;
using Codecrafters.DnsServer.Enums;
using CodeCrafters.DnsServer.Extensions;
namespace Codecrafters.DnsServer;

internal abstract class Program
{
    private static string _resolverServer = string.Empty;
    
    static void Main(string[] args)
    {
        if (args.Count() == 2)
        {
            Debug.Assert(args[0] == "--resolver");
            _resolverServer = args[1];
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
            
            //Console.WriteLine(receivedData.ToHexString());
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
                ResponseCode = incomingHeader.Opcode == OpCode.Query
                    ? ResponseCode.NoError
                    : ResponseCode.NotImplemented,
                AuthorityCount = 0,
                AdditionalCount = 0
            };
            List<byte> response = [.. responseHeader.ToBytes()];
            List<byte> questions = [];
            List<byte> answers = [];
            foreach (Question incomingQuestion in incomingQuestions)
            {
                Answer answerBytes;
                if (_resolverServer == string.Empty)
                {
                    answerBytes = new Answer()
                    {
                        Name = incomingQuestion.Name,
                        Address = IPAddress.Parse("127.0.0.1"),
                        Class = incomingQuestion.Class,
                        Length = 4,
                        TimeToLive = 40,
                        Type = incomingQuestion.Type
                    };
                }
                else
                {
                    // we send the question on to the next server
                    answerBytes = GetResponseFromOtherServer(incomingQuestion);
                }
                questions.AddRange(incomingQuestion.ToBytes());
                answers.AddRange(answerBytes.ToBytes());
            }

            response.AddRange(questions);
            response.AddRange(answers);
            udpClient.Send(response.ToArray(), response.ToArray().Length, sourceEndPoint);
        }
        // ReSharper disable once FunctionNeverReturns
    }

    static Answer GetResponseFromOtherServer(Question incomingQuestion)
    {
        byte[] rewrittenQuery = RewriteQuestionForAnotherResolver(incomingQuestion);
        IPEndPoint ipEndPoint = IPEndPoint.Parse(_resolverServer);
        UdpClient udpClient = new (6969);
        udpClient.Send(rewrittenQuery, rewrittenQuery.Length, ipEndPoint);
        byte[] response = udpClient.Receive(ref ipEndPoint);
        int index = 12;
        _ = Question.FromBytes(response, ref index);
        Answer relayAnswer = Answer.FromBytes(response, ref index);
        udpClient.Close();
        return relayAnswer;
    }

    static byte[] RewriteQuestionForAnotherResolver(Question incomingQuestion)
    {
        Header queryHeader = new()
        {
            PacketIdentifier = 26985,
            QueryResponseIndicator = QueryResponseIndicator.Question,
            QuestionCount = 0x01,
            AnswerCount = 0x00,
            Opcode = OpCode.Query,
            AuthoritativeAnswer = false,
            Truncated = false,
            RecursionDesired = false,
            RecursionAvailable = false,
            ResponseCode = ResponseCode.NoError,
            AuthorityCount = 0,
            AdditionalCount = 0
        };
        Question question = new()
        {
            Class = incomingQuestion.Class,
            Name = incomingQuestion.Name,
            Type = incomingQuestion.Type
        };
        return [..queryHeader.ToBytes(), ..question.ToBytes()];
    }
}