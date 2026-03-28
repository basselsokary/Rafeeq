namespace Application.DTOs.Common;

public record AddressDto(
    string Street,
    string City,
    string? Region,
    string? PostalCode,
    string FullAddress);
