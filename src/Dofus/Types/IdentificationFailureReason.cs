namespace Dofus.Types
{
    public enum IdentificationFailureReason
    {
        BadVersion = 1,
        WrongCredentials = 2,
        Banned = 3,
        Kicked = 4,
        InMaintenance = 5,
        TooManyOnIp = 6,
        TimeOut = 7,
        BadIpRange = 8,
        CredentialsReset = 9,
        EmailUnvalidated = 10,
        OtpTimeout = 11,
        Locked = 12,
        AnonymousIpForbidden = 13,
        ServiceUnavailable = 53,
        ExternalAccountLinkRefused = 61,
        ExternalAccountAlreadyLinked = 62,
        UnknownAuthError = 99,
        Spare = 100,
    }
}
