using CqrsDemo.Application.Common.Interfaces;

namespace CqrsDemo.Application.Products.Commands.CreateProduct;

/// <summary>
/// Request to create a new product. Returns the id of the created product.
/// </summary>
/// <param name="Name">Product display name (required, max 200 characters).</param>
/// <param name="Price">Unit price. Must be greater than 0.</param>
/// <param name="Stock">Initial stock quantity. Must be 0 or greater.</param>
public record CreateProductCommand(string Name, decimal Price, int Stock) : ICommand<Guid>;
