using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Infrastructure;

public interface IAppDbContext
{
    // DbSet<Cart> Carts { get; }
    // DbSet<CartItem> CartItems { get; }
    // DbSet<Order> Orders { get; }
    // DbSet<OrderItem> OrderItems { get; }
    // DbSet<Product> Products { get; }
    // DbSet<Category> Categories { get;  }
    // DbSet<CategoryProduct> CategoryProducts { get; set; }
    // DbSet<Payment> Payments { get; }
    // DbSet<Review> Reviews { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
