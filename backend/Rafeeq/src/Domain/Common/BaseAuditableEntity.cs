namespace Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public Guid CreatedBy { get; protected set; }
    public string CreatedByName { get; protected set; } = null!;

    public DateTime? LastModifiedAt { get; protected set; }
    public Guid? LastModifiedBy { get; protected set; }
    public string? LastModifiedByName { get; protected set; }

    protected BaseAuditableEntity() : base() { }
    // protected BaseAuditableEntity(Guid id) : base(id) { }

    public void SetCreated(DateTime createdAt, Guid createdById, string createdByName)
    {
        CreatedAt = createdAt;
        
        CreatedBy = createdById;
        CreatedByName = createdByName.Trim();
    }
    
    public void SetModified(DateTime lastModifiedAt, Guid lastModifiedById = default, string? lastModifiedByName = null)
    {
        LastModifiedAt = lastModifiedAt;
        
        LastModifiedBy = lastModifiedById;
        LastModifiedByName = lastModifiedByName?.Trim();
    }
}
