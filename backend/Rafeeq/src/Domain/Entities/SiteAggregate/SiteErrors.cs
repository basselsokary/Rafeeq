using Shared.Models;

namespace Domain.Entities.SiteAggregate;

public static class SiteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("SITE_NOT_FOUND", $"Site with ID {id} does not exist.");
    
    public static Error IdRequired =>
        Error.Validation("SITE_EMPTY_ID", "Site ID cannot be empty.");
    
    public static Error CityIdRequired =>
        Error.Validation("SITE_EMPTY_CITY_ID", "City ID cannot be empty.");

    public static Error NameRequired =>
        Error.Validation("SITE_NAME_REQUIRED", "Site name cannot be empty.");

    public static Error DescriptionRequired => 
        Error.Validation("SITE_DESCRIPTION_REQUIRED", "Site description cannot be empty.");
    
    public static Error PhoneRequired => 
        Error.Validation("SITE_CONTACT_PHONE_REQUIRED", "Site contact phone cannot be empty.");
    
    public static Error ImageIdRequired => 
        Error.Validation("SITE_EMPTY_IMAGE_ID", "Site image ID cannot be empty.");
    
    public static Error ImageUrlRequired => 
        Error.Validation("SITE_IMAGEURL_REQUIRED", "Site image url cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound("SITE_IMAGE_NOT_FOUND", "Site Image does not exist.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("SITE_NEGATIVE_DISPLAY_ORDER", "Site display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("SITE_LOCALIZED_CONTENT_NOT_FOUND", "Site localized content does not exist.");

    public static Error FacilityIdRequired => 
        Error.Validation("SITE_EMPTY_FACILITY_ID", "Site facility ID cannot be empty.");
    
    public static Error FacilityNotFound =>
        Error.NotFound("SITE_FACILITY_NOT_FOUND", "Site facility does not exist.");
}