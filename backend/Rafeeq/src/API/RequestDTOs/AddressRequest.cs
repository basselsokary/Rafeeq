namespace API.RequestDTOs;

public record AddressRequest(
    string Street,
    string City,
    string? Region,
    string? PostalCode);
