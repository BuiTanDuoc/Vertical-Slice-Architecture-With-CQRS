namespace CqrsDemo.Application.Common.Interfaces;

// This is the only thing controllers depend on.
// It plays the role MediatR's ISender/IMediator would normally play,
// but it's ~30 lines of our own code instead of a NuGet dependency.
public interface IDispatcher
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
    Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
