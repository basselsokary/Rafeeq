using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.ContentReports;
using Domain.Enums;

namespace Application.Queries.ContentReports;

public sealed record GetContentReportsForTouristQuery(
    PagingParameters Paging,
    ReportStatus? Status = null) : IQuery<PagedResult<ContentReportListDto>>;

internal sealed class GetContentReportsForTouristQueryHandler(
    IContentReportQueryService queryService,
    IEnumLocalizer enumLocalizer) 
    : IQueryHandler<GetContentReportsForTouristQuery, PagedResult<ContentReportListDto>>
{
    public async Task<Result<PagedResult<ContentReportListDto>>> HandleAsync(
        GetContentReportsForTouristQuery query,
        CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetAsync(
            query.Paging,
            status: query.Status,
            cancellationToken: cancellationToken);

        var localizedData = pagedResult.Data.Select(cr => cr with
        {
            ReasonDisplay = enumLocalizer.Localize(cr.Reason),
            StatusDisplay = enumLocalizer.Localize(cr.Status)
        }).ToList();

        return Result.Success(pagedResult with { Data = localizedData });
    }
}