namespace BasePlatform.Application.Common.Abstractions
{
    public interface IDispatcher
    {
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default );
        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default );
    }
}
