using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.ContentReportAggregate;

namespace Application.Queries.ContentReports;

public sealed record GetContentReportByIdForAdminQuery(Guid Id) : IQuery<ContentReportAdminDetailDto>;

internal sealed class GetContentReportByIdForAdminQueryHandler(
    IContentReportQueryService queryService) : IQueryHandler<GetContentReportByIdForAdminQuery, ContentReportAdminDetailDto>
{
    public async Task<Result<ContentReportAdminDetailDto>> HandleAsync(GetContentReportByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var reportDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (reportDetailDto == null)
            return ContentReportErrors.NotFound(query.Id);

        return Result.Success(reportDetailDto);
    }
}
