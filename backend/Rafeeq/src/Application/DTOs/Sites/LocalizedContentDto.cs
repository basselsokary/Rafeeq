namespace Application.DTOs.Sites;

public record LocalizedContentDto(
    string Language,
    string Name,
    string Description);
