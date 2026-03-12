using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Queries.ContentReports;

public record GetContentReportsByHighPriorityQuery(
    int Priority,
    ReportStatus? Status = null,
    ReportReason? Reason = null,
    PagingParameters? Paging = null) : IQuery<PagedResult<ContentReportListDto>>;

internal class GetContentReportsByHighPriorityQueryHandler(IContentReportQueryService queryService) 
    : IQueryHandler<GetContentReportsByHighPriorityQuery, PagedResult<ContentReportListDto>>
{
    public async Task<Result<PagedResult<ContentReportListDto>>> HandleAsync(
        GetContentReportsByHighPriorityQuery query,
        CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetFilteredByPriorityAsync(
            query.Priority,
            query.Reason,
            query.Status,
            query.Paging,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}