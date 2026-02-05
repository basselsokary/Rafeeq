using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Application.Common.Interfaces.Infrustructure;
using System.Reflection;
using Infrastructure.Data.Application.Configurations;

namespace Infrastructure.Data.Application.Context;

internal class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryProduct> CategoryProducts { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            x => x.Namespace == typeof(OrderConfiguration).Namespace);

        base.OnModelCreating(modelBuilder);
    }
}
