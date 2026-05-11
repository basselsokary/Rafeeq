using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.ContentReportAggregate;

public class ContentReportResolvedEvent(Guid ReportId, ModerationAction Action) : BaseEvent
{
    public Guid ReportId { get; } = ReportId;
    public ModerationAction Action { get; } = Action;
}

public class ContentReportedEvent(Guid ReportId, Guid EntityId) : BaseEvent
{
    public Guid ReportId { get; } = ReportId;
    public Guid EntityId { get; } = EntityId;
}
