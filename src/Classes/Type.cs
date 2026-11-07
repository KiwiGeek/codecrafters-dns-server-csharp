namespace CodeCrafters.DnsServer.Classes;

public enum Type
{
    HostAddress = 1,
    AuthoritativeNameServer = 2,
    MailDestination = 3,
    MailForwarder = 4,
    CanonicalName = 5,
    StartOfZoneAuthority = 6,
    MailboxDomainName = 7,
    MailGroupMember = 8,
    MailRenameDomainName = 9,
    Null = 10,
    WellKnownService = 11,
    DomainNamePointer = 12,
    HostInformation = 13,
    MailboxInformation = 14,
    MailExchange = 15,
    Text = 16
}