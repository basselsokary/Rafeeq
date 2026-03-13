namespace Application.DTOs.Common;

/// <summary>
/// Represents a physical address
/// </summary>
public record AddressDto(
    string Street,
    string City,
    string? Region,
    string? PostalCode,
    string FullAddress);
