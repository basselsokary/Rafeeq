using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.ContentReports;
using Domain.Entities.ContentReportAggregate;

namespace Application.Queries.ContentReports;

public record GetContentReportByIdQuery(Guid Id) : IQuery<ContentReportDetailDto>;

internal class GetContentReportByIdQueryHandler(
    IContentReportQueryService queryService) : IQueryHandler<GetContentReportByIdQuery, ContentReportDetailDto>
{
    public async Task<Result<ContentReportDetailDto>> HandleAsync(GetContentReportByIdQuery query, CancellationToken cancellationToken)
    {
        var reportDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (reportDetailDto == null)
            return ContentReportErrors.NotFound(query.Id);

        return Result.Success(reportDetailDto);
    }
}
