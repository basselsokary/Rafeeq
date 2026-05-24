namespace Domain.Common.Constants;

public class UserRoles
{
    public static readonly string[] AllRoles = [SuperAdmin, Admin, Moderator, Tourist];
    
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string Tourist = "Tourist";
    // public const string Guide = "Guide";
}
