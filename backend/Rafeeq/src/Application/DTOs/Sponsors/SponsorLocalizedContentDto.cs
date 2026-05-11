using Domain.Enums;

namespace Application.DTOs.Sponsors;

public sealed record SponsorLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Title,
    string Description);
