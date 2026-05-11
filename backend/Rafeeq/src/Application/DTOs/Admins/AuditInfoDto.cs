namespace Application.DTOs.Admins;

public record AuditInfoDto(
    DateTime CreatedAt,
    Guid CreatedBy,
    string CreatedByName,
    DateTime? LastModifiedAt,
    Guid? LastModifiedBy,
    string? LastModifiedByName);
