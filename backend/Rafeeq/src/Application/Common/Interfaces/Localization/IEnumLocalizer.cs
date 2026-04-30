using Application.DTOs.Common;

namespace Application.Common.Interfaces.Localization;

/// <summary>
/// Translates enum values by looking up keys of the form "{EnumTypeName}_{MemberName}"
/// in the shared .resx file.
///
/// Convention:
///   OrderStatus.Pending  →  key "OrderStatus_Pending"
///   PaymentMethod.Cash   →  key "PaymentMethod_Cash"
///
/// Falls back to the raw member name if no translation is found.
/// </summary>
public interface IEnumLocalizer
{
    /// <summary>
    /// Returns the localized display name for a single enum value.
    /// </summary>
    string Localize<TEnum>(TEnum value) where TEnum : struct, Enum;
    
    // LocalizedEnumValue LocalizeEnum<TEnum>(TEnum value) where TEnum : struct, Enum;

    /// <summary>
    /// Returns all values of an enum with their localized display names —
    /// useful for populating dropdowns on the client.
    /// </summary>
    IReadOnlyList<LocalizedEnumValue> LocalizeAll<TEnum>() where TEnum : struct, Enum;
}