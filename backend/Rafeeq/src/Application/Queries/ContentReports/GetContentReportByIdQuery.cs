using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.ContentReports;
using Domain.Entities.ContentReportAggregate;

namespace Application.Queries.ContentReports;

public sealed record GetContentReportByIdQuery(Guid Id) : IQuery<ContentReportDetailDto>;

internal sealed class GetContentReportByIdQueryHandler(
    IContentReportQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetContentReportByIdQuery, ContentReportDetailDto>
{
    public async Task<Result<ContentReportDetailDto>> HandleAsync(GetContentReportByIdQuery query, CancellationToken cancellationToken)
    {
        var reportDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (reportDetailDto == null || reportDetailDto.ReportedBy != userContext.Id)
            return ContentReportErrors.NotFound(query.Id);

        return Result.Success(reportDetailDto with
        {
            ReasonDisplay = enumLocalizer.Localize(reportDetailDto.Reason),
            StatusDisplay = enumLocalizer.Localize(reportDetailDto.Status),
            ActionTakenDisplay = reportDetailDto.ActionTaken.HasValue ? enumLocalizer.Localize(reportDetailDto.ActionTaken.Value) : null
        });
    }
}
