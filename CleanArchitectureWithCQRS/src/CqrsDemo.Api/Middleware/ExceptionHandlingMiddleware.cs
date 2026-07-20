using System.Net;
using System.Text.Json;
using CqrsDemo.Api.Contracts;
using CqrsDemo.Application.Common.Exceptions;
using ValidationException = CqrsDemo.Application.Common.Exceptions.ValidationException;

namespace CqrsDemo.Api.Middleware;

// Central place to translate Application-layer exceptions into HTTP responses,
// so command/query handlers can stay free of HTTP concerns. Response bodies here
// match the ApiErrorResponse / ApiValidationErrorResponse contracts declared via
// [ProducesResponseType] on the controllers, so Swagger's documented error schemas
// are actually what callers get back.
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(context, HttpStatusCode.BadRequest,
                new ApiValidationErrorResponse("Validation failed", (int)HttpStatusCode.BadRequest, ex.Errors));
        }
        catch (NotFoundException ex)
        {
            await WriteAsync(context, HttpStatusCode.NotFound,
                new ApiErrorResponse(ex.Message, (int)HttpStatusCode.NotFound));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, HttpStatusCode.InternalServerError,
                new ApiErrorResponse("An unexpected error occurred", (int)HttpStatusCode.InternalServerError));
        }
    }

    private static Task WriteAsync(HttpContext context, HttpStatusCode statusCode, object payload)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }
}
