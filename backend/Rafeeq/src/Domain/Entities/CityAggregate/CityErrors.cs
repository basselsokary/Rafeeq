using Shared.Models;

namespace Domain.Entities.CityAggregate;

public static class CityErrors
{
    public static Error NotFound =>
        Error.NotFound(
            "CITY_NOT_FOUND",
            "City does not exist.");

    public static Error NameRequired => 
        Error.Validation(
            "CITY_NAME_REQUIRED",
            "City name cannot be empty.");

    public static Error DescriptionRequired => 
        Error.Validation(
            "CITY_DESCRIPTION_REQUIRED",
            "City description cannot be empty.");
    
    public static Error ImageUrlRequired => 
        Error.Validation(
            "CITY_IMAGEURL_REQUIRED",
            "City image url cannot be empty.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation(
            "CITY_NEGATIVE_DISPLAY_ORDER",
            "City display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound(
            "CITY_NEGATIVE_DISPLAY_ORDER",
            "City display order cannot be negative.");
}