using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.ApplicationContext.Configurations.ValueObjects;

internal static class ValueObjectConversions
{
    public static PropertyBuilder<Rating> HasRatingConversion(
        this PropertyBuilder<Rating> builder,
        string columnName = "Rating")
    {
        builder.HasConversion(
                v => v.Value,
                v => Rating.Create(v).Value)
            .HasColumnName(columnName);

        return builder;
    }

    public static PropertyBuilder<Email?> HasEmailConversion(
        this PropertyBuilder<Email?> builder,
        string columnName,
        int maxLength = Email.MaxEmailLength)
    {
        builder.HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : Email.Create(v).Value)
            .HasColumnName(columnName)
            .HasMaxLength(maxLength)
            .IsRequired(false);

        return builder;
    }

    public static PropertyBuilder<PhoneNumber?> HasPhoneNumberConversion(
        this PropertyBuilder<PhoneNumber?> builder,
        string columnName,
        int maxLength = PhoneNumber.MaxPhoneNumberLength)
    {
        builder.HasConversion(
                v => v == null ? null : v.Value,
                v => v == null ? null : PhoneNumber.Create(v).Value)
            .HasColumnName(columnName)
            .HasMaxLength(maxLength)
            .IsRequired(false);

        return builder;
    }

    public static PropertyBuilder<FileHash> HasFileHashConversion(
        this PropertyBuilder<FileHash> builder,
        string columnName,
        int maxLength = 64)
    {
        builder.HasConversion(
                v => v.Value,
                v => FileHash.From(v))
            .HasColumnName(columnName)
            .HasMaxLength(maxLength);

        return builder;
    }

    public static PropertyBuilder<StorageKey> HasStorageKeyConversion(
        this PropertyBuilder<StorageKey> builder,
        string columnName,
        int maxLength)
    {
        builder.HasConversion(
                v => v.Value,
                v => (StorageKey)v)
            .HasColumnName(columnName)
            .HasMaxLength(maxLength);

        return builder;
    }
}
