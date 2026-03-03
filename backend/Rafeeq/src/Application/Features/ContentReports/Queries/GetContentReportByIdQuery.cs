using FluentValidation;

namespace Application.Features.ContentReports.Queries;

public record GetContentReportByIdQuery() : IQuery<GetContentReportByIdResponse>;

internal class GetContentReportByIdQueryHandler : IQueryHandler<GetContentReportByIdQuery, GetContentReportByIdResponse>
{
    public Task<Result<GetContentReportByIdResponse>> HandleAsync(GetContentReportByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetContentReportByIdQueryValidator : AbstractValidator<GetContentReportByIdQuery>
{
    public GetContentReportByIdQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetContentReportByIdResponse();