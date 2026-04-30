using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Queries.ContentReports;

public sealed record GetContentReportsByHighPriorityQuery(
    PagingParameters Paging,
    int? Priority,
    ReportStatus? Status = null,
    ReportReason? Reason = null) : IQuery<PagedResult<ContentReportListDto>>;

internal sealed class GetContentReportsByHighPriorityQueryHandler(
    IContentReportQueryService queryService,
    IEnumLocalizer enumLocalizer) 
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

        var localizedData = pagedResult.Data.Select(cr => cr with
        {
            ReasonDisplay = enumLocalizer.Localize(cr.Reason),
            StatusDisplay = enumLocalizer.Localize(cr.Status)
        }).ToList();

        return Result.Success(pagedResult with { Data = localizedData });
    }
}