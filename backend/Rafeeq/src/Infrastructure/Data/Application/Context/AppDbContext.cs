using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.Reflection;
using Infrastructure.Data.Application.Configurations;
using Domain.Entities.ReviewAggregate;
using Domain.Entities.TouristAggregate;
using Domain.Entities.PlaceAggregate;
using Domain.Entities.CityAggregate;

namespace Infrastructure.Data.Application.Context;

internal class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Place> Places { get; set; }
    public DbSet<PlaceCategory> PlaceCategories { get; set; }
    public DbSet<PlaceImage> PlaceImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favourite> FavouritePlaces { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            x => x.Namespace == typeof(ReviewConfiguration).Namespace);

        base.OnModelCreating(modelBuilder);
    }
}
