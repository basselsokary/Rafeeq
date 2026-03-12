using Domain.Enums;

namespace Application.DTOs.Sponsors;

public record SponsorFilters(
    SponsorType? Type,
    SponsorTier? Tier,
    Guid? City,
    bool? ActiveOnly);