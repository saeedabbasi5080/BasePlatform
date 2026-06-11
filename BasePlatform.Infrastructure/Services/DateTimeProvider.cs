using BasePlatform.Application.Common.Abstractions;

namespace BasePlatform.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}