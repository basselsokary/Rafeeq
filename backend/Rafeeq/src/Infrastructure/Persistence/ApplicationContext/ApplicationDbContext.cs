using Microsoft.EntityFrameworkCore;
using Domain.Entities.SiteAggregate;
using Domain.Entities.UserAggregate;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.SponsorAggregate;
using Domain.Entities.CityAggregate;
using Domain.Entities.ContentReportAggregate;
using Infrastructure.Persistence.Interceptors;
using Domain.Entities.AttractionAggregate;

namespace Infrastructure.Persistence.ApplicationContext;

internal class ApplicationDbContext : DbContext
{
    private readonly DomainEventDispatcherInterceptor _domainEventDispatcher;
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        DomainEventDispatcherInterceptor domainEventDispatcher,
        AuditableEntityInterceptor auditableEntityInterceptor) 
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    // DbSets for Aggregate Roots
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<Attraction> Attractions => Set<Attraction>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<ContentReport> ContentReports => Set<ContentReport>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(
            _domainEventDispatcher,
            _auditableEntityInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure default schema
        modelBuilder.HasDefaultSchema("rafeeq");
    }
}
