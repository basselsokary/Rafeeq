namespace Application.DTOs.Sites;

public record RatingDistributionDto(
    int FiveStars,
    int FourStars,
    int ThreeStars,
    int TwoStars,
    int OneStar);
