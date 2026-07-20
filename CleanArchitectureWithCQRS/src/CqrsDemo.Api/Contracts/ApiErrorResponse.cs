namespace CqrsDemo.Api.Contracts;

/// <summary>
/// Standard error payload returned for 404, 500 and other non-validation failures.
/// </summary>
/// <param name="Title">Short, human-readable description of the error.</param>
/// <param name="Status">HTTP status code.</param>
public record ApiErrorResponse(string Title, int Status);

/// <summary>
/// Error payload returned when request validation fails (HTTP 400).
/// </summary>
/// <param name="Title">Short, human-readable description of the error.</param>
/// <param name="Status">HTTP status code.</param>
/// <param name="Errors">Validation errors grouped by field name.</param>
public record ApiValidationErrorResponse(string Title, int Status, IDictionary<string, string[]> Errors);
