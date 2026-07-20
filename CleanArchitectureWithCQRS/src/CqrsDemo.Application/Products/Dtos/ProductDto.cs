namespace CqrsDemo.Application.Products.Dtos;

/// <summary>
/// Product data returned to API clients.
/// </summary>
/// <param name="Id">Product identifier.</param>
/// <param name="Name">Product display name.</param>
/// <param name="Price">Unit price.</param>
/// <param name="Stock">Current stock quantity.</param>
/// <param name="CreatedAtUtc">When the product was created (UTC).</param>
public record ProductDto(Guid Id, string Name, decimal Price, int Stock, DateTime CreatedAtUtc);
