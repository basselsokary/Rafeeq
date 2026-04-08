using Domain.Enums;

namespace Application.DTOs.ContentReports;

public record ContentReportSummaryDto(
    Guid Id,
    Guid ReportedBy,
    Guid ReportedEntityId,
    ReportReason Reason,
    string Description,
    ReportStatus Status,
    DateTime ReportedAt);
