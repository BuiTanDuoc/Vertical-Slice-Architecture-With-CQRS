using System.Reflection;
using CqrsDemo.Application.Common;
using CqrsDemo.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsDemo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddScoped<IDispatcher, Dispatcher>();

        // Scan the assembly and register every ICommandHandler<,> / IQueryHandler<,>
        // implementation automatically - this is the piece MediatR normally does for you.
        RegisterOpenGenericImplementations(services, assembly, typeof(ICommandHandler<,>));
        RegisterOpenGenericImplementations(services, assembly, typeof(IQueryHandler<,>));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    private static void RegisterOpenGenericImplementations(IServiceCollection services, Assembly assembly, Type openGenericInterface)
    {
        var matches =
            from type in assembly.GetTypes()
            where !type.IsAbstract && !type.IsInterface
            from @interface in type.GetInterfaces()
            where @interface.IsGenericType && @interface.GetGenericTypeDefinition() == openGenericInterface
            select new { Interface = @interface, Implementation = type };

        foreach (var match in matches)
        {
            services.AddScoped(match.Interface, match.Implementation);
        }
    }
}
