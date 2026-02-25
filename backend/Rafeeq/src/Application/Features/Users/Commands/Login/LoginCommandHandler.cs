using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Features.Users.Commands.Login;

public class LoginCommandHandler(
    IIdentityService identityService,
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork)
    : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        bool passwordCorrect = await _identityService.CheckPasswordAsync(request.Email, request.Password);
        if (!passwordCorrect)
        {
            return UserErrors.InvalidCredentials();
        }

        string accessToken = _tokenProvider.GenerateAccessToken(user.Id, user.Email, user.Roles);
        string refreshToken = _tokenProvider.GenerateRefreshToken();

        await _unitOfWork.RefreshTokens.AddRefreshTokenAsync(user.Id, refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new LoginResponse());
    }
}
