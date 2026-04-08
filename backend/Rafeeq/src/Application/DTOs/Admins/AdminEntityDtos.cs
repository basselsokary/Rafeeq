using Application.DTOs.Common;

namespace Application.DTOs.Admins;
#region Trip DTOs
// ============================================================================
// TRIP DTOs
// ============================================================================

/// <summary>
/// Admin trip list DTO
/// </summary>
public class AdminTripListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string Status { get; set; }
    public DateRangeDto DateRange { get; set; }
    public int TotalAttractions { get; set; }
    public int CompletionPercentage { get; set; }
    public bool IsPublic { get; set; }
    public int ShareCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Admin trip statistics
/// </summary>
public class AdminTripStatisticsDto
{
    public int TotalTrips { get; set; }
    public int DraftTrips { get; set; }
    public int PlannedTrips { get; set; }
    public int InProgressTrips { get; set; }
    public int CompletedTrips { get; set; }
    public int CancelledTrips { get; set; }

    public Dictionary<string, int> CountByStatus { get; set; } = new();
    public Dictionary<string, int> CountByTransportation { get; set; } = new();

    public int PublicTrips { get; set; }
    public int PrivateTrips { get; set; }

    public int TripsCreatedToday { get; set; }
    public int TripsCreatedThisWeek { get; set; }
    public int TripsCreatedThisMonth { get; set; }

    public double AverageAttractionsPerTrip { get; set; }
    public double AverageTripDurationDays { get; set; }

    public List<PopularAttractionInTripsDto> MostAddedAttractions { get; set; } = new();
}

/// <summary>
/// Popular attraction in trips DTO
/// </summary>
public class PopularAttractionInTripsDto
{
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; }
    public int TimesAdded { get; set; }
}
#endregion

#region Review DTOs
// ============================================================================
// REVIEW DTOs
// ============================================================================

/// <summary>
/// Admin review list DTO
/// </summary>
public class AdminReviewListDto
{
    public Guid Id { get; set; }
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Status { get; set; }
    public DateTime VisitDate { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public bool IsVerifiedVisit { get; set; }
    public int TotalImages { get; set; }
    public int TotalResponses { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Admin review detail DTO
/// </summary>
public class AdminReviewDetailDto
{
    public Guid Id { get; set; }
    public Guid AttractionId { get; set; }
    public string AttractionName { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Status { get; set; }
    public DateTime VisitDate { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public bool IsVerifiedVisit { get; set; }
    public string? RejectionReason { get; set; }

    // Images
    public List<ImageDto> Images { get; set; } = new();

    // Responses
    public List<ReviewResponseDto> Responses { get; set; } = new();

    // Moderation
    public int FlagCount { get; set; }
    public List<ReviewFlagDto> Flags { get; set; } = new();
    public string? ModeratorNotes { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Review response DTO
/// </summary>
public class ReviewResponseDto
{
    public Guid Id { get; set; }
    public Guid ResponderId { get; set; }
    public string ResponderName { get; set; }
    public string Content { get; set; }
    public bool IsOfficialResponse { get; set; }
    public DateTime ResponseDate { get; set; }
}

/// <summary>
/// Review flag DTO
/// </summary>
public class ReviewFlagDto
{
    public Guid Id { get; set; }
    public Guid FlaggedBy { get; set; }
    public string FlaggedByName { get; set; }
    public string Reason { get; set; }
    public string? Description { get; set; }
    public DateTime FlaggedAt { get; set; }
}

/// <summary>
/// Admin review statistics
/// </summary>
public class AdminReviewStatisticsDto
{
    public int TotalReviews { get; set; }
    public int PendingReviews { get; set; }
    public int ApprovedReviews { get; set; }
    public int RejectedReviews { get; set; }
    public int FlaggedReviews { get; set; }

    public Dictionary<string, int> CountByStatus { get; set; } = new();
    public Dictionary<int, int> CountByRating { get; set; } = new();

    public int VerifiedVisitReviews { get; set; }
    public int ReviewsWithImages { get; set; }
    public int ReviewsWithResponses { get; set; }

    public int ReviewsCreatedToday { get; set; }
    public int ReviewsCreatedThisWeek { get; set; }
    public int ReviewsCreatedThisMonth { get; set; }

    public double AverageRating { get; set; }
    public double AverageHelpfulnessScore { get; set; }

    public List<TopReviewerDto> TopReviewers { get; set; } = new();
}

/// <summary>
/// Top reviewer DTO
/// </summary>
public class TopReviewerDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
}
#endregion

#region Sponsor DTOs
// ============================================================================
// SPONSOR DTOs
// ============================================================================

/// <summary>
/// Admin sponsor detail DTO
/// </summary>
public class AdminSponsorDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Tier { get; set; }

    // Location
    public LocationDto Location { get; set; }
    public AddressDto Address { get; set; }

    // Contact
    public string ContactPhone { get; set; }
    public string ContactEmail { get; set; }
    public string? Website { get; set; }

    // Contract
    public DateTime ContractStartDate { get; set; }
    public DateTime ContractEndDate { get; set; }
    public bool IsContractValid { get; set; }
    public MoneyDto? ContractValue { get; set; }

    // Status
    public bool IsActive { get; set; }

    // Statistics
    public int TotalClicks { get; set; }
    public int TotalRedemptions { get; set; }
    public double ConversionRate { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }

    // Images
    public List<ImageDto> Images { get; set; } = new();

    // Offers
    public List<SponsorOfferDto> Offers { get; set; } = new();

    // Internal
    public string? InternalNotes { get; set; }
    public List<string> Tags { get; set; } = new();

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public string? LastModifiedByName { get; set; }
}

/// <summary>
/// Sponsor offer DTO with admin fields
/// </summary>
public class SponsorOfferDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public MoneyDto? DiscountAmount { get; set; }
    public int? DiscountPercentage { get; set; }
    public DateRangeDto ValidityPeriod { get; set; }
    public string? TermsAndConditions { get; set; }
    public string? PromoCode { get; set; }
    public bool IsActive { get; set; }
    public int RedemptionCount { get; set; }
    public int? MaxRedemptions { get; set; }
    public bool CanBeRedeemed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Admin sponsor statistics
/// </summary>
public class AdminSponsorStatisticsDto
{
    public int TotalSponsors { get; set; }
    public int ActiveSponsors { get; set; }
    public int InactiveSponsors { get; set; }
    public int ExpiredContractSponsors { get; set; }

    public Dictionary<string, int> CountByType { get; set; } = new();
    public Dictionary<string, int> CountByTier { get; set; } = new();

    public int TotalActiveOffers { get; set; }
    public int TotalClicks { get; set; }
    public int TotalRedemptions { get; set; }
    public double OverallConversionRate { get; set; }

    public decimal TotalContractValue { get; set; }
    public string ContractValueCurrency { get; set; }

    public List<TopSponsorDto> TopSponsorsByClicks { get; set; } = new();
    public List<TopSponsorDto> TopSponsorsByRedemptions { get; set; } = new();
    public List<TopOfferDto> TopOffers { get; set; } = new();
}

/// <summary>
/// Top sponsor DTO
/// </summary>
public class TopSponsorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Value { get; set; } // Clicks or redemptions
    public double ConversionRate { get; set; }
}

/// <summary>
/// Top offer DTO
/// </summary>
public class TopOfferDto
{
    public Guid Id { get; set; }
    public Guid SponsorId { get; set; }
    public string SponsorName { get; set; }
    public string OfferTitle { get; set; }
    public int RedemptionCount { get; set; }
}
#endregion