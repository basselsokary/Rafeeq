using Shared;

namespace Domain.Entities.ArtifactAggregate;

public static class ArtifactErrors
{
    public static Error NameRequired
        => Error.Validation("ARTIFACT_NAME_REQUIRED", "Name is required.");
    public static Error DescriptionRequired
        => Error.Validation("ARTIFACT_DESCRIPTION_REQUIRED", "Description is required.");

    public static Error NegativeDisplayOrder
        => Error.Validation("ARTIFACT_NEGATIVE_DISPLAY_ORDER", "Display order cannot be negative.");

    public static Error LocalizedContentNotFound
        => Error.NotFound("ARTIFACT_LOCALIZED_CONTENT_NOT_FOUND", "Localized content not found for the specified language.");
    public static Error ImageNotFound
        => Error.NotFound("ARTIFACT_IMAGE_NOT_FOUND", "Image not found.");
}