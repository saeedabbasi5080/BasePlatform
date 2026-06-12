using BasePlatform.Application.Common.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BasePlatform.Infrastructure.Dispatcher;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TResult>(
        ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync")!;
        var task = (Task<TResult>)method.Invoke(handler, [command, cancellationToken])!;

        return await task;
    }

    public async Task SendAsync(
        ICommand command,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(BasePlatform.Shared.Result));

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync")!;
        var task = (Task<BasePlatform.Shared.Result>)method.Invoke(handler, [command, cancellationToken])!;

        await task;
    }

    public async Task<TResult> QueryAsync<TResult>(
        IQuery<TResult> query,
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));

        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod("HandleAsync")!;
        var task = (Task<TResult>)method.Invoke(handler, [query, cancellationToken])!;

        return await task;
    }
}