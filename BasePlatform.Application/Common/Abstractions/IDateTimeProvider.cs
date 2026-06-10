namespace BasePlatform.Application.Common.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
