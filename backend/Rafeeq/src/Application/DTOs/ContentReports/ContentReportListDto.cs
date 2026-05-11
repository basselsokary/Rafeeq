using Domain.Enums;

namespace Application.DTOs.ContentReports;

public record ContentReportListDto(
    Guid Id,
    Guid ReportedBy,
    Guid ReportedEntityId,
    ReportReason Reason,
    string Description,
    ReportStatus Status,
    DateTime ReportedAt,
    string ReasonDisplay = "",
    string StatusDisplay = "");
