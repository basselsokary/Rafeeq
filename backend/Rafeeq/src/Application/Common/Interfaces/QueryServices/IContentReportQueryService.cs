using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IContentReportQueryService
{
    Task<ContentReportDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<PagedResult<ContentReportListDto>> GetFilteredByPriorityAsync(
        int priority,
        ReportReason? reason = null,
        ReportStatus? status = null,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
}
