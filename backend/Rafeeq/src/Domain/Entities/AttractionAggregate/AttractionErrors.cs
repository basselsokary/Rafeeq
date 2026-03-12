using Shared.Models;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Domain.Entities.AttractionAggregate;

public class AttractionErrors
{
    public static Error IdRequired =>
        Error.Validation("ATTRACTION_EMPTY_ID", "Attraction ID cannot be empty.");
    
    public static Error SiteIdRequired =>
        Error.Validation("ATTRACTION_EMPTY_SITE_ID", "Site ID cannot be empty.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("ATTRACTION_NOT_FOUND", $"Attraction with ID {id} does not exist.");

    public static Error NameRequired => 
        Error.Validation("ATTRACTION_NAME_REQUIRED", "Attraction name cannot be empty.");
    public static Error ExceededNameLength => 
        Error.Validation("ATTRACTION_NAME_LENGTH_EXCEEDED", $"Attraction name length cannot exceed {MaxNameLength}");

    public static Error DescriptionRequired => 
        Error.Validation("ATTRACTION_DESCRIPTION_REQUIRED", "Attraction description cannot be empty.");
    public static Error ExceededDescriptionLength => 
        Error.Validation("ATTRACTION_DESCRIPTION_LENGTH_EXCEEDED", $"Attraction description length cannot exceed {MaxDescriptionLength}");
    
    public static Error ImageUrlRequired => 
        Error.Validation("ATTRACTION_IMAGEURL_REQUIRED", "Attraction image url cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound("ATTRACTION_IMAGE_NOT_FOUND", "Attraction Image does not exist.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation("ATTRACTION_NEGATIVE_DISPLAY_ORDER", "Attraction display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound("ATTRACTION_LOCALIZED_CONTENT_NOT_FOUND", "Attraction localized content does not exist.");
}