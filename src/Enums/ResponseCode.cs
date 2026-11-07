namespace Codecrafters.DnsServer.Enums;

public enum ResponseCode
{
    NoError = 0,
    FormatError = 1,
    ServerFailure = 2,
    NonExistentDomain = 3,
    NotImplemented = 4,
    QueryRefused = 5,
    YxDomain = 6,
    YxRrSet = 7,
    NxRrSet = 8,
    NotAuthorized = 9,
    OutsideZone = 10,
    BadKey = 17,
    BadTime = 18,
    BadMode = 19,
    BadName = 20,
    BalAlgorithm = 21,
    BadTruncation = 22,
    BadCookie = 23
}