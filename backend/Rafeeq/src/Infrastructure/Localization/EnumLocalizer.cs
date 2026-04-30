using Microsoft.Extensions.Localization;
using Infrastructure.Localization.Resources;
using Application.DTOs.Common;
using Application.Common.Interfaces.Localization;

namespace Infrastructure.Localization;

internal sealed class EnumLocalizer(
    IStringLocalizer<EnumResource> localizer) : IEnumLocalizer
{
    public string Localize<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var resourceKey = BuildKey(value);
        var localized = localizer[resourceKey];

        // ResourceNotFound means no translation exists → fall back to the member name
        return localized.ResourceNotFound ? value.ToString() : localized.Value;
    }

    public IReadOnlyList<LocalizedEnumValue> LocalizeAll<TEnum>() where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>()
            .Select(v => new LocalizedEnumValue(
                Value: Convert.ToInt32(v),
                Key: v.ToString(),
                DisplayName: Localize(v)))
            .ToList();

    private static string BuildKey<TEnum>(TEnum value) where TEnum : struct, Enum =>
        $"{typeof(TEnum).Name}_{value}";
}
