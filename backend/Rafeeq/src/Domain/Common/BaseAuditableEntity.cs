namespace Domain.Common;

public class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    public string? LastModifiedBy { get; set; }
}
