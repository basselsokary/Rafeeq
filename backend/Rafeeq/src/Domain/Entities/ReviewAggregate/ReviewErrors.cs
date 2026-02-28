using Shared.Models;

namespace Domain.Entities.ReviewAggregate;

public static class ReviewErrors
{
    public static Error NotFound =>
        Error.NotFound(
            "REVIEW_NOT_FOUND",
            "Review was not found.");
    
    public static Error EntityIdRequired(string entityName) =>
        Error.Validation(
            $"REVIEW_EMPTY_{entityName.ToUpper()}_ID",
            $"{entityName} ID cannot be empty.");

    public static Error TitleRequired =>
        Error.Validation(
            "REVIEW_TITLE_REQUIRED",
            "Review title cannot be empty.");

    public static Error ContentRequired => 
        Error.Validation(
            "REVIEW_CONTENT_REQUIRED",
            "Review content cannot be empty.");
    
    public static Error ImageUrlRequired => 
        Error.Validation(
            "REVIEW_IMAGEURL_REQUIRED",
            "Review image url cannot be empty.");
    
    public static Error ImageNotFound =>
        Error.NotFound(
            "REVIEW_IMAGE_NOT_FOUND",
            "Review Image does not exist.");
    
    public static Error RejectionReasonReqiured => 
        Error.Validation(
            "REVIEW_EMPTY_REJECTION_REASON",
            "Rejection reason cannot be empty.");
}
