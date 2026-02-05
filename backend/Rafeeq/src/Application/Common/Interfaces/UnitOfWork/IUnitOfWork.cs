using Application.Common.Interfaces.Repositories;
using Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    /// Repository properties
    // IUserRepository Users { get; }
    // IBasketRepository Baskets { get; }
    // IEventRepository Events { get; }
    // IRefreshTokenRepository RefreshTokens { get; }
    // ITicketRepository Tickets { get; }

    /// Transaction methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// Generic repository access
    IEntityBaseRepository<T> Repository<T>() where T : BaseEntity;
}