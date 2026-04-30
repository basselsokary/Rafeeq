namespace Application.Common.Interfaces.Authentication;

public interface IPasswordGenerator
{
    string GenerateTemporaryPassword();
}