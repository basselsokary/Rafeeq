using Domain.Repositories;

namespace Domain.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository access
    ISiteRepository Sites { get; }
    IAttractionRepository Attractions { get; }
    ICityRepository Cities { get; }
    IContentReportRepository ContentReports { get; }
    IReviewRepository Reviews { get; }
    ISponsorRepository Sponsors { get; }
    IUserRepository Tourists { get; }
    ITripRepository Trips { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// Generic repository access
    IBaseRepository<T> Repository<T>() where T : IAggregateRoot;
}
