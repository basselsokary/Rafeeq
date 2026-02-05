using Domain.Errors;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Models;

namespace Infrastructure.User.Services;

public static class ResultsExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
        => result.Succeeded ?
            Result.Success() :
            Result.Failure(result.Errors.Select(e => e.Description));

    public static Result ToApplicationResult(this SignInResult result)
        => result.IsLockedOut ?
                Result.Failure(["User is locked out."]) :
            result.IsNotAllowed ?
                Result.Failure(["User is not allowed to sign in."]) :
            result.RequiresTwoFactor ?
                Result.Failure(["Two-factor authentication is required."]) :
            result.Succeeded ?
                Result.Success() : Result.Failure(UserErrors.InvalidCredentials);
}