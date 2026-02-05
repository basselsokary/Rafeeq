using Infrastructure.Data.Application.Configurations.Owened;
using Infrastructure.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Domain.Constants.DomainConstants.Customer;

namespace Infrastructure.Data.Identity.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.FullName)
            .IsRequired()
            .HasMaxLength(FullNameMaxLength);

        builder.Property(c => c.Age)
            .IsRequired(false);

        builder.OwnsOne(
            c => c.Address,
            b => AddressConfiguration.Configure(b));
    }
}
