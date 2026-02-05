namespace Domain.Constants;

public static class DomainConstants
{
    public static class Notification
    {
        public const int MaxTitleLength = 64;
        public const int MaxMessageLength = 512;
        public const int MaxIdLength = 64;

    }

    public static class Event
    {
        public const int MaxTitleLength = 128;
        public const int MaxDescriptionLength = 2048;
        public const int MaxVenueLength = 256;
        public const int MaxLocationLength = 256;
        public const int MaxEventAttachmentsPerEvent = 10;
        
    }

    public static class EventAttachment
    {
        public const int MaxTextLength = 256;
        public const int MaxFileUrlLength = 128;
    }

    public static class Money
    {
        public const int Precision = 18;
        public const int Scale = 2;
        public const int MaxCurrencyLength = 3;
    }

    public static class User
    {
        public const int MaxUserNameLength = 32;
        public const int MaxFullNameLength = 128;
        public const int MaxEmailLength = 128;
        public const int MaxPasswordLength = 64;
        public const int MinPasswordLength = 8;
        public const int MaxPasswordHashLength = 512;
    }

    public static class RefreshToken
    {
        public const int MaxRefreshTokenLength = 64;
    }
}