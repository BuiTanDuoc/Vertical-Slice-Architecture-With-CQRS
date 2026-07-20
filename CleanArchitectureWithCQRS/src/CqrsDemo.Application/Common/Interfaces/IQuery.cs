namespace CqrsDemo.Application.Common.Interfaces;

// A Query represents a read that returns TResult and must not change state.
public interface IQuery<TResult> { }

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
