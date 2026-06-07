using Domain.Common;

namespace Domain.Entities.ArtifactAggregate;

public class ArtifactCreatedEvent(Guid artifactId) : BaseEvent
{
    public Guid ArtifactId { get; } = artifactId;
}

public class ArtifactUpdatedEvent(Guid artifactId) : BaseEvent
{
    public Guid ArtifactId { get; } = artifactId;
}

public class ArtifactDeletedEvent(Guid artifactId) : BaseEvent
{
    public Guid ArtifactId { get; } = artifactId;
}

public class ArtifactImageUpdatedEvent(Guid artifactId) : BaseEvent
{
    public Guid ArtifactId { get; } = artifactId;
}

public class ArtifactLocalizedContentUpdatedEvent(Guid artifactId) : BaseEvent
{
    public Guid ArtifactId { get; } = artifactId;
}

// public class ArtifactStatusChangedEvent(Guid ArtifactId, ArtifactStatus NewStatus) : BaseEvent
// {
//     public Guid ArtifactId { get; } = ArtifactId;
//     public ArtifactStatus NewStatus { get; } = NewStatus;
// }