using Shared.Models;

namespace Domain.Entities.ContentReportAggregate;

public class ContentReportErrors
{
    public static Error NotFound =>
        Error.NotFound(
            "REPORT_NOT_FOUND",
            "The content report does not exist.");

    // public static Error InvalidContent =>
    //     Error.Validation(
    //         "REPORT_INVALID_CONTENT",
    //         "The content to report is invalid or missing.");

    public static Error DescriptionRequired => 
        Error.Validation(
            "REPORT_DESCRIPTION_REQUIRED",
            "Report description cannot be empty.");

    public static Error CantBeSolved =>
        Error.Failure(
            "REPORT_CANT_BE_REVIEWD",
            "Only pending or under review reports can be reviewed.");
    
    public static Error CantBeReviewed =>
        Error.Failure(
            "REPORT_CANT_BE_REVIEWED",
            "Only pending reports can be marked as under review.");
    
    public static Error CantBeDismissed =>
        Error.Failure(
            "REPORT_CANT_BE_Dismissed",
            "Dismissal reason cannot be empty.");
}
