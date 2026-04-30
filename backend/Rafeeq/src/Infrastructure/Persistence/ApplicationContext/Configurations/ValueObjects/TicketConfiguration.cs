using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class TicketConfiguration
{
    public static void Configure<T>(
        this OwnedNavigationBuilder<T, Ticket> ownerBuilder) where T : class
    {
        ownerBuilder.Property<bool>("_exists")
            .HasColumnName("TicketExists")
            .IsRequired();

        ownerBuilder.OwnsOne(t => t.EgyptianPrice, money =>
        {
            money.Configure("EgyptianPrice");

        });

        ownerBuilder.OwnsOne(t => t.ForeignerPrice, money =>
        {
            money.Configure("ForeignerPrice");
        });
    }
}
