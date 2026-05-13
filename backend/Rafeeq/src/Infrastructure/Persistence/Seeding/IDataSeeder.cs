namespace Infrastructure.Persistence.Seeding;

internal interface IDataSeeder
{
    /// <summary>
    /// Execution order — lower values run first.
    /// Use gaps (10, 20, 30…) so new seeders can be inserted between existing ones.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Perform the seed operation.
    /// Implementations must be idempotent: running twice must not create duplicates.
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
