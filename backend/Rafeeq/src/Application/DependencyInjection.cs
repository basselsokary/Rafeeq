using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;
using Application.DTOs.Attractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.DTOs.Sponsors;
using Application.DTOs.Sites;
using Application.DTOs.Users;
using Application.DTOs.Cities;
using Application.DTOs.ContentReports;
using Application.Common.Interfaces.Authentication;
using Application.Commands.Users;
using Domain.Common.Interfaces;
using Domain.Repositories;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddServicesByScrutor();

        services.AddBehaviors();

        services.AddDecrotors();

        return services;
    }

    private static void AddDecrotors(this IServiceCollection services)
    {
        
        // services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        // services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));
        // services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));

        // services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        // services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));
        // services.Decorate(typeof(IQueryHandler<,>), typeof(ValidationDecorator.QueryHandler<,>));
    }

    private static void AddBehaviors(this IServiceCollection services)
    {
        services.AddScoped<ISiteQueryService, SiteQueryService>();
        services.AddScoped<IReviewQueryService, ReviewQueryService>();
        services.AddScoped<IAttractionQueryService, AttractionQueryService>();
        services.AddScoped<ISponsorQueryService, SponsorQueryService>();
        services.AddScoped<IContentReportQueryService, ContentReportQueryService>();
        services.AddScoped<ICityQueryService, CityQueryService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

    }

    private static void AddServicesByScrutor(this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}

internal class UnitOfWork : IUnitOfWork
{
    public ISiteRepository Sites => throw new NotImplementedException();

    public IAttractionRepository Attractions => throw new NotImplementedException();

    public ICityRepository Cities => throw new NotImplementedException();

    public IContentReportRepository ContentReports => throw new NotImplementedException();

    public IReviewRepository Reviews => throw new NotImplementedException();

    public ISponsorRepository Sponsors => throw new NotImplementedException();

    public IUserRepository Tourists => throw new NotImplementedException();

    public ITripRepository Trips => throw new NotImplementedException();

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IBaseRepository<T> Repository<T>() where T : IAggregateRoot
    {
        throw new NotImplementedException();
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

internal class IdentityService : IIdentityService
{
    public Task<bool> AddRoleToUserAsync(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckPasswordAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result> CheckUserExistsAsync(string userName, string email)
    {
        throw new NotImplementedException();
    }

    public Task<(Result Result, string CustomerId)> CreateCustomerAsync(RegisterCommand registerCommand)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetRolesAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInRoleAsync(string userId, string role)
    {
        throw new NotImplementedException();
    }

    public Task<Result<LoginResponse>> SignInAsync(string email, string password, bool rememberMe = false, bool lockoutOnFailure = false)
    {
        throw new NotImplementedException();
    }

    public Task<Result<RefreshResponse>> SignInAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SignOutAsync()
    {
        throw new NotImplementedException();
    }
}

internal class UserContext : IUserContext
{
    public Guid Id => throw new NotImplementedException();

    public LanguageCode Language => throw new NotImplementedException();

    public bool IsAuthenticated => throw new NotImplementedException();
}

internal class ContentReportQueryService : IContentReportQueryService
{
    public Task<ContentReportDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<ContentReportListDto>> GetFilteredByPriorityAsync(int priority, ReportReason? reason = null, ReportStatus? status = null, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

internal class CityQueryService : ICityQueryService
{
    public Task<List<CitySummaryDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<CityListDto>> GetAsync(PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CityDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

internal class UserQueryService : IUserQueryService
{
    public Task<PagedResult<UserListDto>> GetAllAsync(string? searchTerm, UserRole? role = null, UserStatus status = UserStatus.Active, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(Guid userId, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasFavoritedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
internal class SiteQueryService : ISiteQueryService
{
    public Task<PagedResult<SiteListDto>> GetAsync(SiteFilters? filters = null, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SiteDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<SiteListDto>> GetFeaturedAsync(int count = 10, Guid? city = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<DTOs.Sites.LocalizedContentDto>> GetLocalizedContentsAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<SiteListDto>> GetNearbyAsync(double latitude, double longitude, int radiusKm = 5, SiteFilters? filters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<SiteListDto>> GetSimilarAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(BoundingBox bounds, SiteFilters? filters = null, int count = 20, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<SiteListDto>> SearchAsync(string searchTerm, SiteFilters? filters = null, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
internal class SponsorQueryService : ISponsorQueryService
{
    public Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(SponsorFilters filters, bool activeOnly = true, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<SponsorListDto>> GetAsync(SponsorFilters filters, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SponsorDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<NearbySponsorDto>> GetNearbyAsync(double latitude, double longitude, double radiusKm = 3, SponsorFilters? filters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<SponsorOfferDto?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<SponsorOfferDto>> GetOffersAsync(Guid sponsorId, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<SponsorListDto>> SearchAsync(string searchTerm, SponsorFilters filters, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
internal class AttractionQueryService : IAttractionQueryService
{
    public Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<AttractionListDto>> GetByTypeAsync(Guid siteId, AttractionType type, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
internal class ReviewQueryService : IReviewQueryService
{
    public Task<PagedResult<ReviewListDto>> GetApprovedBySiteIdAsync(Guid siteId, string? sortBy = "Helpful", PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ReviewDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<ReviewListDto>> GetBySiteAndRatingAsync(Guid siteId, int rating, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<ReviewListDto>> GetBySiteIdAsync(Guid siteId, string? sortBy = "Helpful", PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<ReviewListDto>> GetByStatusAsync(ReviewStatus status = ReviewStatus.Pending, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<UserReviewDto>> GetByUserIdAsync(Guid userId, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReviewSummaryDto>> GetRecentBySiteIdAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserReviewDto>> GetRecentByUserIdAsync(Guid userId, int count = 5, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ReviewSummaryDto>> GetTopHelpfulBySiteIdAsync(Guid siteId, int count = 5, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

