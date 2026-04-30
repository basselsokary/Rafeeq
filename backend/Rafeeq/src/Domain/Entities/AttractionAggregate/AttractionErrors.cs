using Shared;

namespace Domain.Entities.AttractionAggregate;

public class AttractionErrors
{
    public static Error IdRequired =>
        Error.Validation("ATTRACTION_ID_REQUIRED", "Attraction ID is required.");
    
    public static Error SiteIdRequired =>
        Error.Validation("ATTRACTION_SITE_ID_REQUIRED", "Site ID is required.");
    
    public static Error ContentIdRequired =>
        Error.Validation("ATTRACTION_CONTENT_ID_REQUIRED", "Content ID is required.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("ATTRACTION_NOT_FOUND", $"Attraction with ID {id} does not exist.");

    public static Error NameRequired => 
        Error.Validation("ATTRACTION_NAME_REQUIRED", "Attraction name cannot be empty.");
    public static Error ExceededNameLength => 
        Error.Validation("ATTRACTION_NAME_LENGTH_EXCEEDED", "Attraction name length cannot exceed the defined maximum.");

    public static Error ExceededLocationDescriptionLength => 
        Error.Validation("ATTRACTION_LOCATION_DESCRIPTION_LENGTH_EXCEEDED", "Attraction location description length cannot exceed the defined maximum.");
    public static Error DescriptionRequired => 
        Error.Validation("ATTRACTION_DESCRIPTION_REQUIRED", "Attraction description cannot be empty.");
    public static Error ExceededDescriptionLength => 
        Error.Validation("ATTRACTION_DESCRIPTION_LENGTH_EXCEEDED", "Attraction description length cannot exceed the defined maximum.");
    public static Error ExceededImageCaptionLength => 
        Error.Validation("ATTRACTION_IMAGE_CAPTION_LENGTH_EXCEEDED", "Attraction image caption length cannot exceed the defined maximum.");
    
    public static Error ImageUrlRequired => 
        Error.Validation("ATTRACTION_IMAGE_URL_REQUIRED", "Attraction image URL is required.");
    
    public static Error ImageNotFound =>
        Error.NotFound("ATTRACTION_IMAGE_NOT_FOUND", "Attraction image does not exist.");
    
    public static Error ImageCaptionRequired =>
        Error.Validation("ATTRACTION_IMAGE_CAPTION_REQUIRED", "Attraction image caption is required.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("ATTRACTION_NEGATIVE_DISPLAY_ORDER", "Attraction display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("ATTRACTION_LOCALIZED_CONTENT_NOT_FOUND", "Attraction localized content does not exist.");

    public static Error HistoricalPeriodAlreadyExists =>
        Error.Validation("ATTRACTION_HISTORICAL_PERIOD_ALREADY_EXISTS", "Attraction already has the specified historical period.");

    public static Error HistoricalPeriodRequired =>
        Error.Validation("ATTRACTION_HISTORICAL_PERIOD_REQUIRED", "Attraction historical period is required.");
}