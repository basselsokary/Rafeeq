using Domain.Enums;
using Shared;

namespace Domain.Entities.ReviewAggregate;

public static class ReviewErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("REVIEW_NOT_FOUND", $"Review with ID {id} was not found.");

    public static Error InvalidStatus(ReviewStatus status) =>
        Error.Validation("REVIEW_INVALID_STATUS", $"Review status {status} is not valid.");
    
    public static Error Unauthorized =>
        Error.Validation("REVIEW_UNAUTHORIZED", $"You are not authorized to perform this action.");

    public static Error IdRequired =>
        Error.Validation("REVIEW_ID_REQUIRED", "Review ID is required.");
    
    public static Error SiteIdRequired =>
        Error.Validation("REVIEW_SITE_ID_REQUIRED", "Site ID is required.");
    
    public static Error TouristIdRequired =>
        Error.Validation("REVIEW_USER_ID_REQUIRED", "User ID is required.");

    public static Error TitleRequired =>
        Error.Validation("REVIEW_TITLE_REQUIRED", "Review title cannot be empty.");
    
    public static Error ExeededTitleLength =>
        Error.Validation("REVIEW_TITLE_EXCEEDED_LENGTH", "Review title exceeds the maximum allowed length.");

    public static Error ContentRequired => 
        Error.Validation("REVIEW_CONTENT_REQUIRED", "Review content cannot be empty.");
    
    public static Error ExeededContentLength =>
        Error.Validation("REVIEW_CONTENT_EXCEEDED_LENGTH", "Review content exceeds the maximum allowed length.");
    
    public static Error ImageUrlRequired => 
        Error.Validation("REVIEW_IMAGE_URL_REQUIRED", "Review image URL is required.");
    
    public static Error ImageNotFound =>
        Error.NotFound("REVIEW_IMAGE_NOT_FOUND", "Review image does not exist.");
    
    public static Error RejectionReasonRequired => 
        Error.Validation("REVIEW_REJECTION_REASON_REQUIRED", "Rejection reason is required.");
    
    public static Error UserAlreadyReviewdSite => 
        Error.Validation("REVIEW_USER_ALREADY_REVIEWED_SITE", "User has already reviewed this site.");

}
