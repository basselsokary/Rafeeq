using Domain.Repositories;

namespace Domain.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface for transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository access
    IAttractionRepository Places { get; }
    ITouristRepository Tourists { get; }
    ITripRepository Trips { get; }
    IReviewRepository Reviews { get; }
    ISponsorRepository Sponsors { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// Generic repository access
    // IEntityBaseRepository<T> Repository<T>() where T : BaseEntity;
}
