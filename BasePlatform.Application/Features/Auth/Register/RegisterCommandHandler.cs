using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler
    : ICommandHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<RegisterResponse>> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser is not null)
            return Result<RegisterResponse>.Failure(
                Error.Conflict("Auth.EmailTaken", "This email address is already registered."));

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            DisplayName = $"{command.FirstName} {command.LastName}",
            Email = command.Email,
            UserName = command.Email,
            IsActive = true,
            CreatedAt = _dateTimeProvider.UtcNow,
            UpdatedAt = _dateTimeProvider.UtcNow
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<RegisterResponse>.Failure(
                Error.Validation("Auth.RegistrationFailed", errorMessage));
        }

        await _userManager.AddToRoleAsync(user, "User");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        await _emailService.SendAsync(
            user.Email!,
            "Confirm your email",
            $"Your confirmation token: {token}",
            cancellationToken: cancellationToken);

        return Result<RegisterResponse>.Success(
            new RegisterResponse(user.Id, user.Email!, "Registration successful. Please confirm your email."));
    }
}