using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Identity.Entities;
using Domain.ValueObjects;
using static Domain.Common.Constants.DomainConstants.User;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.Email).HasMaxLength(Email.MaxEmailLength).IsRequired();
        
        builder.Property(user => user.CreatedAt).IsRequired();

        builder.HasDiscriminator<string>("UserRole")
            .HasValue<TouristUser>("Tourist")
            .HasValue<ModeratorUser>("Moderator")
            .HasValue<AdminUser>("Admin");
    }
}

internal sealed class StaffUserConfiguration : IEntityTypeConfiguration<StaffUser>
{
    public void Configure(EntityTypeBuilder<StaffUser> builder)
    {
        builder.Property(user => user.FirstName).HasMaxLength(MaxFirstNameLength).IsRequired();
        builder.Property(user => user.LastName).HasMaxLength(MaxLastNameLength).IsRequired();
        builder.Property(user => user.FullName).HasMaxLength(MaxFullNameLength).IsRequired();
    }
}