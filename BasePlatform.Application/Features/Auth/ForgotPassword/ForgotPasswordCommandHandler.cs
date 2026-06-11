using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Domain.Entities;
using BasePlatform.Shared;
using Microsoft.AspNetCore.Identity;

namespace BasePlatform.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;

    public ForgotPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result> HandleAsync(
        ForgotPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);

        // عمداً موفق برمی‌گردیم تا email enumeration جلوگیری شود
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            return Result.Success();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _emailService.SendAsync(
            user.Email!,
            "Reset your password",
            $"Your password reset token: {token}",
            cancellationToken: cancellationToken);

        return Result.Success();
    }
}