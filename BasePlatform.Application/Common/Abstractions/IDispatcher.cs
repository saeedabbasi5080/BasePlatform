namespace BasePlatform.Application.Common.Abstractions
{
    public interface IDispatcher
    {
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default );
    }
}
