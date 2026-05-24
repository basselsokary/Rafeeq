namespace Infrastructure.Authorization;

public static class Policies
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string AdminOnly = "AdminOnly";
    public const string ModeratorOrAdmin = "ModeratorOrAdmin";
    public const string TouristOnly = "TouristOnly";

    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageCities = "CanManageCities";
    public const string CanManageSites = "CanManageSites";
    public const string CanManageArtifacts = "CanManageArtifacts";
    public const string CanManageAttractions = "CanManageAttractions";
    public const string CanManageSponsors = "CanManageSponsors";
    public const string CanModerateContent = "CanModerateContent";
    public const string CanViewAnalytics = "CanViewAnalytics";
    public const string CanManageOwnTrips = "CanManageOwnTrips";
    public const string CanWriteReviews = "CanWriteReviews";
    public const string CanReportContent = "CanReportContent";
    public const string CanImportData = "CanImportData";

    /// <summary>Used for resource-based checks: is this the owner?</summary>
    public const string ResourceOwner = "ResourceOwner";
}
