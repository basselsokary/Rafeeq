using Domain.Enums;

namespace Application.DTOs.ContentReports;

public record ContentReportAdminDetailDto(
    Guid Id,
    Guid ReportedBy,
    Guid ReportedEntityId,
    ReportReason Reason,
    string Description,
    ReportStatus Status,
    int Priority,
    DateTime ReportedAt,
    Guid? ReviewedBy,
    DateTime? ReviewedAt,
    string? ReviewNotes,
    ModerationAction? ActionTaken,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
