using Domain.Common;
using Shared.Models;

namespace Domain.Entities.AttractionAggregate;

public class AttractionErrors
{
    public static Error NotFound() =>
        Error.NotFound(
            "ATTRACTION_NOT_FOUND",
            "Attraction does not exist.");

    public static Error NameRequired => 
        Error.Validation(
            "ATTRACTION_NAME_REQUIRED",
            "Attraction name cannot be empty.");

    public static Error DescriptionRequired => 
        Error.Validation(
            "ATTRACTION_DESCRIPTION_REQUIRED",
            "Attraction description cannot be empty.");
    
    public static Error ImageUrlRequired => 
        Error.Validation(
            "ATTRACTION_IMAGEURL_REQUIRED",
            "Attraction image url cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound(
            "ATTRACTION_IMAGE_NOT_FOUND",
            "Attraction Image does not exist.");
    
    public static Error NegativeDisplayOrder => 
        Error.Validation(
            "ATTRACTION_NEGATIVE_DISPLAY_ORDER",
            "Attraction display order cannot be negative.");

    public static Error LocalizedContentNotFound =>
        Error.NotFound(
            "ATTRACTION_LOCALIZED_CONTENT_NOT_FOUND",
            "Attraction localized content does not exist.");
}