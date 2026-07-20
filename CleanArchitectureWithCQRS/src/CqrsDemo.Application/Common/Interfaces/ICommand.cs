namespace CqrsDemo.Application.Common.Interfaces;

// A Command represents an intent to change state and returns TResult.
public interface ICommand<TResult> { }

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
