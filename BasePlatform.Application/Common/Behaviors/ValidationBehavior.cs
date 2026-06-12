using BasePlatform.Application.Common.Abstractions;
using BasePlatform.Shared;
using FluentValidation;

namespace BasePlatform.Application.Common.Behaviors;

public sealed class ValidationBehavior<TCommand, TResult>
    : ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : class
{
    private readonly ICommandHandler<TCommand, TResult> _inner;
    private readonly IEnumerable<IValidator<TCommand>> _validators;

    public ValidationBehavior(
        ICommandHandler<TCommand, TResult> inner,
        IEnumerable<IValidator<TCommand>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<TResult> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
            return await _inner.HandleAsync(command, cancellationToken);

        var context = new ValidationContext<TCommand>(command);

        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await _inner.HandleAsync(command, cancellationToken);

        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // TResult باید Result یا Result<T> باشد
        // از طریق reflection به صورت ایمن برمی‌گردانیم
        var resultType = typeof(TResult);

        if (resultType == typeof(Result))
        {
            return (Result.Failure(Error.Validation("Validation.Failed", errorMessage)) as TResult)!;
        }

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failureMethod = resultType.GetMethod("Failure", [typeof(Error)])!;
            return (TResult)failureMethod.Invoke(null, [Error.Validation("Validation.Failed", errorMessage)])!;
        }

        return await _inner.HandleAsync(command, cancellationToken);
    }
}