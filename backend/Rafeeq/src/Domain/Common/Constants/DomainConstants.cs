namespace Domain.Common.Constants;

public static class DomainConstants
{
    public static class Attraction
    {
        public const int MaxNameLength = 128;
        public const int MaxDescriptionLength = 2048;
        public const int MaxWebsiteUrlLength = 256;
        public const int MaxNearestMetroStationLength = 128;
        public const int MaxNearestBusStopLength = 128;
        public const int MaxPhoneLength = 32;
        public const int MinRatingValue = 1;
        public const int MaxRatingValue = 5;
    }

    public static class Category
    {
        public const int MaxNameLength = 512;
        public const int MaxDescriptionLength = 2048;
    }

    public static class Review
    {
        public const int MaxTextLength = 2048;
        public const int MaxRatingValue = 5;
        public const int MinRatingValue = 1;

    }

    public static class ContentReport
    {
        public const int HighPriority = 4;
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
        public const int MaxEmailLength = 254; // // Maximum length of emails is 254
        public const int MaxPasswordLength = 64;
        public const int MinPasswordLength = 8;
        public const int MaxPasswordHashLength = 512;
    }

    public static class RefreshToken
    {
        public const int MaxRefreshTokenLength = 64;
    }
}