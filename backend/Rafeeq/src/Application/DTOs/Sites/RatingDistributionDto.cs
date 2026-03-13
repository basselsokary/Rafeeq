namespace Application.DTOs.Sites;

/// <summary>
/// Rating distribution DTO
/// </summary>
public record RatingDistributionDto(
    int FiveStars,
    int FourStars,
    int ThreeStars,
    int TwoStars,
    int OneStar);
