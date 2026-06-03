using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IContentReportQueryService
{
    Task<ContentReportDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ContentReportAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<PagedResult<ContentReportListDto>> GetAsync(
        PagingParameters paging,
        int? priority = null,
        ReportReason? reason = null,
        ReportStatus? status = null,
        CancellationToken cancellationToken = default);
}
