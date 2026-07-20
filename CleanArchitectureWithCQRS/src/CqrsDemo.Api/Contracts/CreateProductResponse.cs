namespace CqrsDemo.Api.Contracts;

/// <summary>
/// Response returned after a product is successfully created.
/// </summary>
/// <param name="Id">Identifier of the newly created product.</param>
public record CreateProductResponse(Guid Id);
