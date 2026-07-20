using CqrsDemo.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsDemo.Application.Common;

// Reflection-based dispatcher: resolves the matching ICommandHandler<,>/IQueryHandler<,>
// from DI at runtime and invokes it. Also runs FluentValidation validators (if any are
// registered for the command type) before calling the handler - this replaces MediatR's
// IPipelineBehavior<,> validation-behavior pattern with a couple of explicit lines here.
public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();

        await ValidateAsync(command, commandType, cancellationToken);

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        return await handler.HandleAsync((dynamic)command, cancellationToken);
    }

    public async Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        return await handler.HandleAsync((dynamic)query, cancellationToken);
    }

    private async Task ValidateAsync(object request, Type requestType, CancellationToken cancellationToken)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var validator = _serviceProvider.GetService(validatorType) as IValidator;

        if (validator is null) return;

        var context = new ValidationContext<object>(request);
        var result = await validator.ValidateAsync(context, cancellationToken);

        if (!result.IsValid)
            throw new Exceptions.ValidationException(result.Errors);
    }
}
