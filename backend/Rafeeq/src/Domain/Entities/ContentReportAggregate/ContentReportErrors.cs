using Shared;
using static Domain.Common.Constants.DomainConstants.ContentReport;

namespace Domain.Entities.ContentReportAggregate;

public class ContentReportErrors
{
    public static Error IdRequired =>
        Error.Validation("REPORT_ID_REQUIRED", "Report ID is required.");
    
    public static Error ReportedEntityIdRequired =>
        Error.Validation("REPORTED_ENTITY_ID_REQUIRED", "Reported entity ID is required.");

    public static Error NotFound(Guid id) =>
        Error.NotFound("REPORT_NOT_FOUND", $"The content report with ID {id} does not exist.");

    // public static Error InvalidContent =>
    //     Error.Validation(
    //         "REPORT_INVALID_CONTENT",
    //         "The content to report is invalid or missing.");

    public static Error ReasonRequired => 
        Error.Validation("REPORT_REASON_REQUIRED", "Report reason cannot be empty.");
    
    public static Error ExceededReasonLength => 
        Error.Validation("REPORT_REASON_LENGTH_EXCEEDED", $"Report reason length cannot exceed {MaxReasonLength}");

    public static Error DescriptionRequired => 
        Error.Validation("REPORT_DESCRIPTION_REQUIRED", "Report description cannot be empty.");
    public static Error ExceededDescriptionLength => 
        Error.Validation("REPORT_DESCRIPTION_LENGTH_EXCEEDED", $"Report description length cannot exceed {MaxDescriptionLength}");
   
    // public static Error CannotBeResolved =>
    //     Error.Validation("REPORT_CANNOT_BE_RESOLVED", "Either reason or action must be provided, but not both.");
    
    public static Error CannotBeSolved =>
        Error.Validation("REPORT_CANNOT_BE_REVIEWED", "Only pending or under-review reports can be reviewed.");
    
    public static Error CannotBeMarkedUnderReview =>
        Error.Validation("REPORT_CANNOT_BE_MARKED_UNDER_REVIEW", "Only pending reports can be marked as under review.");
    
    public static Error CannotBeDismissed =>
        Error.Validation("REPORT_CANNOT_BE_DISMISSED", "Dismissal reason is required.");
}
