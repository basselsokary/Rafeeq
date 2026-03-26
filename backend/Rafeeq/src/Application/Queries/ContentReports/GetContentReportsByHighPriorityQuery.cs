using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Queries.ContentReports;

public record GetContentReportsByHighPriorityQuery(
    PagingParameters Paging,
    int? Priority,
    ReportStatus? Status = null,
    ReportReason? Reason = null) : IQuery<PagedResult<ContentReportListDto>>;

internal class GetContentReportsByHighPriorityQueryHandler(IContentReportQueryService queryService) 
    : IQueryHandler<GetContentReportsByHighPriorityQuery, PagedResult<ContentReportListDto>>
{
    public async Task<Result<PagedResult<ContentReportListDto>>> HandleAsync(
        GetContentReportsByHighPriorityQuery query,
        CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetAsync(
            query.Paging,
            query.Priority,
            query.Reason,
            query.Status,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}