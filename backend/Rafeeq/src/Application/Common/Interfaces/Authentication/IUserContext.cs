namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    string Id { get; }
    bool IsAuthenticated { get; }
}
