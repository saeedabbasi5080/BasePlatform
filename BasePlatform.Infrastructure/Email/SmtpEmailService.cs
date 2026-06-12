using BasePlatform.Application.Common.Abstractions;

namespace BasePlatform.Infrastructure.Email;

public sealed class SmtpEmailService : IEmailService
{
    public Task SendAsync(
        string to,
        string subject,
        string body,
        bool isHtml = true,
        CancellationToken cancellationToken = default)
    {
        // Full implementation in Phase 13/14
        return Task.CompletedTask;
    }
}