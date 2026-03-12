using Domain.Enums;

namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    LanguageCode Language { get; }
    bool IsAuthenticated { get; }
}
