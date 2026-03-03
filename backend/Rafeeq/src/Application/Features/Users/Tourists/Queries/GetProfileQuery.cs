using FluentValidation;

namespace Application.Features.Sites.Queires;

public record GetProfileQuery(
) : IQuery<GetProfileResponse>;

internal class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, GetProfileResponse>
{
    public Task<Result<GetProfileResponse>> HandleAsync(GetProfileQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetProfileResponse();