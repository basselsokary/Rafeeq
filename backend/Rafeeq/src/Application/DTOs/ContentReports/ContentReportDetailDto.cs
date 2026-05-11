using Domain.Enums;

namespace Application.DTOs.ContentReports;

public record ContentReportDetailDto(
    Guid Id,
    Guid ReportedBy,
    Guid ReportedEntityId,
    ReportReason Reason,
    string Description,
    ReportStatus Status,
    DateTime ReportedAt,
    DateTime? ReviewedAt,
    string? ReviewNotes,
    ModerationAction? ActionTaken,
    string ReasonDisplay = "",
    string StatusDisplay = "",
    string? ActionTakenDisplay = "");
