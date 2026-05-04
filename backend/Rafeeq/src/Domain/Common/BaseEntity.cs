namespace Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } 
    
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
    
    protected BaseEntity(Guid id)
    {
        Id = id;
    }

    private readonly List<BaseEvent> _domainEvents = [];
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void RaiseDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
