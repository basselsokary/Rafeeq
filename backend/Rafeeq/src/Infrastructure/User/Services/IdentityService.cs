using Application.Common.Interfaces.Authentication;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Models;
using Domain.Enums;
using Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Domain.Errors;
using Domain.ValueObjects;
using Application.Mappers;
using Application.Features.Users.Commands.Register;

namespace Infrastructure.User.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<UserDto?> GetUserDtoByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user == null ? null
            : await ToUserDto(user);
    }

    public async Task<UserDto?> GetUserDtoByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user == null ? null
            : await ToUserDto(user);
    }

    public async Task<string?> GetRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
    }

    private async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        => await _userManager.GetRolesAsync(user);

    public async Task<Result> SignInAsync(string email, string password, bool rememberMe = false, bool lockoutOnFailure = false)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Result.Failure(UserErrors.NotFound);

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure);
        return result.ToApplicationResult();
    }

    public async Task<Result> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return Result.Success();
    }
    
    public async Task<(Result Result, string CustomerId)> CreateCustomerAsync(RegisterCommand registerCommand)
    {
        var customer = Customer.Create(
            registerCommand.UserName,
            registerCommand.Email,
            registerCommand.FullName,
            registerCommand.PhoneNumber,
            registerCommand.Address.Map(),
            registerCommand.Age);

        var result = await _userManager.CreateAsync(customer, registerCommand.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        result = await _userManager.AddToRoleAsync(customer, nameof(UserRole.Customer));

        return (result.ToApplicationResult(), customer.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<Result> CheckUserExistsAsync(string userName, string email)
    {
        if (await _userManager.Users.AnyAsync(u => u.Email == email))
            return Result.Failure(UserErrors.EmailAlreadyExists);

        if (await _userManager.Users.AnyAsync(u => u.UserName == userName))
            return Result.Failure(UserErrors.UserNameAlreadyExists);

        return Result.Success();
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return false;

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<bool> CheckPasswordAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<bool> AddRoleToUserAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var identityResult = await _userManager.AddToRoleAsync(user, role);
        return identityResult.Succeeded;
    }

    private async Task<UserDto> ToUserDto(ApplicationUser user)
    {
        return new(
            user.Id,
            user.Email!,
            user.UserName!,
            await GetRolesAsync(user));
    }

    public Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        return _userManager.Users
            .OfType<Customer>()
            .AsNoTracking()
            .Select(c => new CustomerDto(
                c.Id,
                c.UserName!,
                c.FullName,
                c.Email!,
                c.Age,
                new(c.Address.Street, c.Address.District, c.Address.City, c.Address.ZipCode),
                c.PhoneNumber!,
                c.DateJoined))
            .ToListAsync();
    }

    public Task<CustomerDto?> GetCustomerDtoByIdAsync(string customerId)
    {
        return _userManager.Users
            .OfType<Customer>()
            .AsNoTracking()
            .Where(c => c.Id == customerId)
            .Select(c => new CustomerDto(
                c.Id,
                c.UserName!,
                c.FullName,
                c.Email!,
                c.Age,
                new(c.Address.Street, c.Address.District, c.Address.City, c.Address.ZipCode),
                c.PhoneNumber!,
                c.DateJoined))
            .FirstOrDefaultAsync();
    }

    public async Task<Result> UpdateCustomerAsync(
        string customerId,
        string fullName,
        Address address,
        string phoneNumber,
        int? age,
        CancellationToken cancellationToken)
    {
        var customer = await _userManager.Users.OfType<Customer>()
            .AsTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

        if (customer == null)
            return Result.Failure(CustomerErrors.NotFound);

        customer.Update(fullName, address, phoneNumber, age);
        return (await _userManager.UpdateAsync(customer)).ToApplicationResult();
    }
}
