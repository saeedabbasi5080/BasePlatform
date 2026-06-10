namespace BasePlatform.Application.Common.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
    }
}
