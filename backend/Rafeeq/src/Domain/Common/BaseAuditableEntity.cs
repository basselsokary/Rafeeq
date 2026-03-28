namespace Domain.Common;

public class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    // public string? CreatedBy { get; protected set; }

    public DateTime LastModifiedAt { get; protected set; } = DateTime.UtcNow;
    // public string? LastModifiedBy { get; protected set; }

    protected BaseAuditableEntity() : base()
    {   
    }

    protected BaseAuditableEntity(Guid id) : base(id)
    {
    }

    protected void MarkAsUpdated()
    {
        LastModifiedAt = DateTime.UtcNow;
    }
}
