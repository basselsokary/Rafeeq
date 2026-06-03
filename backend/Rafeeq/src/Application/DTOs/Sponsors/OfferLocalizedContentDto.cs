using Domain.Enums;

namespace Application.DTOs.Sponsors;

public sealed record OfferLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Title,
    string Description);
