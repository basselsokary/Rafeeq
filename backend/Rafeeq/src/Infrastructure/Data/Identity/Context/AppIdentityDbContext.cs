using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Infrastructure.User;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Infrastructure.Data.Identity.Configurations;
using Infrastructure.Authentication;

namespace Infrastructure.Data.Identity.Context;

internal class AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            x => x.Namespace == typeof(CustomerConfiguration).Namespace);

        base.OnModelCreating(builder);
    }
}