using Microsoft.EntityFrameworkCore;
using Domain.Entities.SiteAggregate;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.SponsorAggregate;
using Domain.Entities.CityAggregate;
using Domain.Entities.ContentReportAggregate;
using Domain.Entities.AttractionAggregate;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Domain.Entities.TouristAggregate;

namespace Infrastructure.Persistence.ApplicationContext;

internal class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // DbSets for Aggregate Roots
    public DbSet<Site> Sites { get; set; }
    public DbSet<Attraction> Attractions { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ContentReport> ContentReports { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Sponsor> Sponsors { get; set; }
    public DbSet<Tourist> Tourists { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Customize table names
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");

        // Apply all entity configurations from this assembly
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure default schema
        builder.HasDefaultSchema("rafeeq");
    }
}
