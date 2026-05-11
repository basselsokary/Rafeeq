using Domain.Enums;

namespace Application.DTOs.Sites;

public record SiteLocalizedContentDto(
    LanguageCode Language,
    string Name,
    string Description);
