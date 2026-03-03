using FluentValidation;

namespace Application.Features.ContentReports.Queries;

public record GetContentReportsQuery() : IQuery<GetContentReportsResponse>;

internal class GetContentReportsQueryHandler : IQueryHandler<GetContentReportsQuery, GetContentReportsResponse>
{
    public Task<Result<GetContentReportsResponse>> HandleAsync(GetContentReportsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetContentReportsQueryValidator : AbstractValidator<GetContentReportsQuery>
{
    public GetContentReportsQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetContentReportsResponse();