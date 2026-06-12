using BasePlatform.Application.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace BasePlatform.Application.Common.Behaviors;

public sealed class LoggingBehavior<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly ILogger<LoggingBehavior<TCommand, TResult>> _logger;

    public LoggingBehavior(
        ICommandHandler<TCommand, TResult> inner,
        ILogger<LoggingBehavior<TCommand, TResult>> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<TResult> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default)
    {
        var commandName = typeof(TCommand).Name;

        _logger.LogInformation("Executing command {CommandName}", commandName);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await _inner.HandleAsync(command, cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation(
                "Command {CommandName} executed in {ElapsedMs}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Command {CommandName} failed after {ElapsedMs}ms",
                commandName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}