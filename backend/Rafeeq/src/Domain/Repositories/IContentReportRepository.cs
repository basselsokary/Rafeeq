using Domain.Entities.ContentReportAggregate;
using Domain.Enums;

namespace Domain.Repositories;

public interface IContentReportRepository : IBaseRepository<ContentReport>
{
    Task<IEnumerable<ContentReport>> GetPendingReportsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentReport>> GetByStatusAsync(ReportStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentReport>> GetHighPriorityReportsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentReport>> GetByReporterAsync(Guid touristId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentReport>> GetByEntityAsync(Guid entityId, CancellationToken cancellationToken = default);
}
