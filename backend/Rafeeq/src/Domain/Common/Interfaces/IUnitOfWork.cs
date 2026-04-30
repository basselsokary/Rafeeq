using Domain.Repositories;

namespace Domain.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    /// Repository access
    ISiteRepository Sites { get; }
    IAttractionRepository Attractions { get; }
    ICityRepository Cities { get; }
    IContentReportRepository ContentReports { get; }
    IReviewRepository Reviews { get; }
    ISponsorRepository Sponsors { get; }
    ITouristRepository Tourists { get; }
    ITripRepository Trips { get; }

    /// Generic operations
    Task UpdateAsync<T>(T entity) where T : BaseEntity;
    Task DeleteAsync<T>(T entity) where T : BaseEntity;
    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity;

    /// Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
