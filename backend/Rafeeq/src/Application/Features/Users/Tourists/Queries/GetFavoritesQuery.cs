using FluentValidation;

namespace Application.Features.Sites.Queires;

public record GetFavoritesQuery(
) : IQuery<GetFavoritesResponse>;

internal class GetFavoritesQueryHandler : IQueryHandler<GetFavoritesQuery, GetFavoritesResponse>
{
    public Task<Result<GetFavoritesResponse>> HandleAsync(GetFavoritesQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetFavoritesQueryValidator : AbstractValidator<GetFavoritesQuery>
{
    public GetFavoritesQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetFavoritesResponse();