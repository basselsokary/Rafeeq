namespace Application.DTOs.Reviews;

public record RatingDistributionDto(
    int FiveStars,
    int FourStars,
    int ThreeStars,
    int TwoStars,
    int OneStar)
{
    public int Total => FiveStars + FourStars + ThreeStars + TwoStars + OneStar;
    public double FiveStarsPercentage => Total > 0 ? (FiveStars * 100.0 / Total) : 0;
    public double FourStarsPercentage => Total > 0 ? (FourStars * 100.0 / Total) : 0;
    public double ThreeStarsPercentage => Total > 0 ? (ThreeStars * 100.0 / Total) : 0;
    public double TwoStarsPercentage => Total > 0 ? (TwoStars * 100.0 / Total) : 0;
    public double OneStarPercentage => Total > 0 ? (OneStar * 100.0 / Total) : 0;
}
