using Shared.Models;
using static Domain.Common.Constants.DomainConstants.City;

namespace Domain.Entities.CityAggregate;

public static class CityErrors
{
    public static Error IdRequired =>
        Error.Validation("CITY_EMPTY_ID", "City ID cannot be empty.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("CITY_NOT_FOUND", $"City with ID {id} does not exist.");

    public static Error NameRequired => 
        Error.Validation("CITY_NAME_REQUIRED", "City name cannot be empty.");
    public static Error ExceededNameLength => 
        Error.Validation("CITY_NAME_LENGTH_EXCEEDED", $"City name length cannot exceed {MaxNameLength}");
    public static Error DescriptionRequired => 
        Error.Validation("CITY_DESCRIPTION_REQUIRED", "City description cannot be empty.");
    public static Error ExceededDescriptionLength => 
        Error.Validation("CITY_DESCRIPTION_LENGTH_EXCEEDED", $"City description length cannot exceed {MaxDescriptionLength}");
    
    public static Error ImageUrlRequired => 
        Error.Validation("CITY_IMAGEURL_REQUIRED", "City image url cannot be empty.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("CITY_NEGATIVE_DISPLAY_ORDER", "City display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("CITY_NEGATIVE_DISPLAY_ORDER", "City display order cannot be negative.");
}