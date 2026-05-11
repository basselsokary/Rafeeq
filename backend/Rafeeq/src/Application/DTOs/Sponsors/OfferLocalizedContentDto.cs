using Application.DTOs.Admins;
using Domain.Enums;

namespace Application.DTOs.Sponsors;

public sealed record OfferLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Title,
    string Description);

public sealed record AdminOfferLocalizedContentDto(
    Guid Id,
    LanguageCode Language,
    string Title,
    string Description,
    string? TermsAndConditions,
    AuditInfoDto AuditInfo);
