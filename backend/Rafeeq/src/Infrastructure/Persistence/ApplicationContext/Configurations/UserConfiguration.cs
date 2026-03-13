using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RafeeqApp.Domain.Entities.UserAggregate;

namespace RafeeqApp.Infrastructure.Persistence.ApplicationDbContext.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.PreferredLanguage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Bio)
            .HasMaxLength(1000);

        builder.Property(u => u.Nationality)
            .HasMaxLength(100);

        builder.Property(u => u.DateOfBirth);

        builder.Property(u => u.LastLoginAt)
            .IsRequired();

        builder.Property(u => u.EmailVerified)
            .HasDefaultValue(false);

        builder.Property(u => u.PhoneVerified)
            .HasDefaultValue(false);

        builder.Property(u => u.TotalTrips)
            .HasDefaultValue(0);

        builder.Property(u => u.TotalReviews)
            .HasDefaultValue(0);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        // Value Objects - Email
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasMaxLength(256)
                .IsRequired();

            email.WithOwner();
        });

        // Value Objects - PhoneNumber
        builder.OwnsOne(u => u.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20);

            phone.WithOwner();
        });

        // Relationships - Preferences (one-to-many)
        builder.HasMany(u => u.Preferences)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - FavoriteAttractions (one-to-many)
        builder.HasMany(u => u.FavoriteAttractions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships - VisitedAttractions (one-to-many)
        builder.HasMany(u => u.VisitedAttractions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.Role)
            .HasDatabaseName("IX_Users_Role");

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);

        // Navigation properties metadata
        builder.Metadata
            .FindNavigation(nameof(User.Preferences))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(User.FavoriteAttractions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(User.VisitedAttractions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
