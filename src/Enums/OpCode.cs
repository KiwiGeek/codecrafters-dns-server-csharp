namespace Codecrafters.DnsServer.Enums;

public enum OpCode
{
    Query = 0,        // 000
    InverseQuery = 1, // 001
    Status = 2,       // 010
    Notify = 4,       // 100
    Update = 5        // 101 
}