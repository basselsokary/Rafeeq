using System.Data;

namespace Domain.Common.Constants;

public static class DomainConstants
{
    public static class Site
    {
        public const int MaxNameLength = 128;
        public const int MaxDescriptionLength = 4096;
        public const int MaxWebsiteUrlLength = 256;
        public const int MaxNearestTransportationStationLength = 128;
        public const int MaxNearestBusStopLength = 128;
        public const int MaxContactPhoneLength = 32;
    }

    public class Sponsor
    {
        public const int MaxTitleLength = 128;
        public const int MaxDescriptionLength = 4096;
        public const int MaxTermsLength = 2048;
        public const int MaxPromoCodeLength = 64;
        public const int MaxNearestTransportationStationLength = 128;
        public const int MaxNearestBusStopLength = 128;
        public const int MaxContactPhoneLength = 32;
    }

    public static class Attraction
    {
        public const int MaxNameLength = 128;
        public const int MaxDescriptionLength = 4096;
        public const int MaxLocationDescriptionLength = 1024;
    }

    public static class Artifact
    {
        public const int MaxNameLength = 128;
        public const int MaxDescriptionLength = 4096;
    }

    public static class City
    {
        public const int MaxNameLength = 128;
        public const int MaxDescriptionLength = 4096;
    }

    public static class Trip
    {
        public const int MaxNameLength = 256;
        public const int MaxDescriptionLength = 4096;
        public const int MaxNoteLength = 2048;
    }

    public static class Review
    {
        public const int MaxTitleLength = 256;
        public const int MaxContentLength = 4096;
        public const int MaxRejectionReasonLength = 512;
        public const int MaxRatingValue = 5;
        public const int MinRatingValue = 1;

    }

    public static class ContentReport
    {
        public const int HighPriority = 5;
        public const int MaxDescriptionLength = 4096;
        public const int MaxReviewNotesLength = 2048;
        public const int MaxNotesLength = 2048;
        public const int MaxReasonLength = 512;
    }

    public static class Money
    {
        public const int Precision = 18;
        public const int Scale = 2;
        public const int MaxCurrencyLength = 3;
    }

    public static class Ticket
    {
        public const int MaxNotesLength = 2048;
    }

    public static class GeoLocation
    {
        public const int Precision = 9;
        public const int Scale = 6;
    }

    public static class Address
    {
        public const int MaxAddressLength = 512;
        public const int MaxStreetLength = 256;
        public const int MaxCityLength = 128;
        public const int MaxRegionLength = 128;
        public const int MaxPostalCodeLength = 16;
    }

    public static class Tourist
    {
        public const int MaxUserNameLength = 128;
        public const int MaxFirstNameLength = 128;
        public const int MaxLastNameLength = 128;
        public const int MaxFullNameLength = 512;
        public const int MaxNationalityLength = 128;
        public const int MaxNotesLength = 2048;
    }

    public static class User
    {
        public const int MaxUserNameLength = 128;
        public const int MaxFirstNameLength = 128;
        public const int MaxLastNameLength = 128;
        public const int MaxFullNameLength = 512;
        public const int MaxPasswordLength = 64;
        public const int MinPasswordLength = 8;
        public const int MaxPasswordHashLength = 512;
    }

    public static class RefreshToken
    {
        public const int MaxRefreshTokenLength = 128;
    }

    public static class Image
    {
        public const int MaxImagesPerRequest = 5;
        public const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        public const int MaxStorageKeyLength = 256;
        public const int MaxImageUrlLength = 512;
        public const int MaxCaptionLength = 256;
    }
}