using Domain.Enums;
using Shared.Models;

namespace Domain.Entities.ReviewAggregate;

public static class ReviewErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("REVIEW_NOT_FOUND", $"Review with ID {id} was not found.");

    public static Error InvalidStatus(ReviewStatus status) =>
        Error.Validation("REVIEW_INVALID_STATUS", $"Review status {status} is not valid.");

    public static Error IdRequired =>
        Error.Validation("REVIEW_EMPTY_ID", "Review ID cannot be empty.");
    
    public static Error SiteIdRequired =>
        Error.Validation("REVIEW_EMPTY_SITE_ID", "Site ID cannot be empty.");
    
    public static Error TouristIdRequired =>
        Error.Validation("REVIEW_EMPTY_USER_ID", "User ID cannot be empty.");

    public static Error TitleRequired =>
        Error.Validation("REVIEW_TITLE_REQUIRED", "Review title cannot be empty.");

    public static Error ContentRequired => 
        Error.Validation("REVIEW_CONTENT_REQUIRED", "Review content cannot be empty.");
    
    public static Error ImageUrlRequired => 
        Error.Validation("REVIEW_IMAGEURL_REQUIRED", "Review image url cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound("REVIEW_IMAGE_NOT_FOUND", "Review Image does not exist.");
    
    public static Error RejectionReasonRequired => 
        Error.Validation("REVIEW_EMPTY_REJECTION_REASON", "Rejection reason cannot be empty.");
    
    public static Error UserAlreadyReviewdSite => 
        Error.Validation("USER_ALREADY_REVIEWD_SITE", "User has already reviewd site before.");

}
