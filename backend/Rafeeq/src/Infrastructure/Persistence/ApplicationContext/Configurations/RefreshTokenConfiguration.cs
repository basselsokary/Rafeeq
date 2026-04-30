using Domain.Common.Constants;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .HasMaxLength(DomainConstants.RefreshToken.MaxRefreshTokenLength)
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.HasIndex(rt => rt.UserId);

        builder.HasIndex(rt => rt.ExpiresAt);
    }
}