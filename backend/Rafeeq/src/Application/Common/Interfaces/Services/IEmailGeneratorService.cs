namespace Application.Common.Interfaces.Services;

public interface IEmailGeneratorService
{
    Task<string> GenerateModeratorEmailAsync(string firstName, string lastName, CancellationToken cancellationToken);
    Task<string> GenerateUniqueUsernameAsync(string firstName, string lastName, CancellationToken cancellationToken);
}

