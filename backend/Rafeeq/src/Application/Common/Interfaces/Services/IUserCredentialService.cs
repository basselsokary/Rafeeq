namespace Application.Common.Interfaces.Services;

public interface IUserCredentialService
{
    Task<string> GenerateUniqueUsernameAsync(string firstName, string lastName, CancellationToken cancellationToken);
    string GenerateTemporaryPassword();
}

