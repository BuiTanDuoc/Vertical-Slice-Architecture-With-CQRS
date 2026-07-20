using CqrsDemo.Api.Contracts;
using CqrsDemo.Application.Common.Interfaces;
using CqrsDemo.Application.Products.Commands.CreateProduct;
using CqrsDemo.Application.Products.Dtos;
using CqrsDemo.Application.Products.Queries.GetProductById;
using CqrsDemo.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace CqrsDemo.Api.Controllers;

/// <summary>
/// Manages products: listing, retrieving by id, and creating new ones.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    // Controllers only ever depend on IDispatcher - never on handlers, repositories,
    // or EF Core directly. That's the whole point of the CQRS boundary here.
    private readonly IDispatcher _dispatcher;

    public ProductsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Get all products.
    /// </summary>
    /// <remarks>Ordered by creation date, oldest first.</remarks>
    /// <response code="200">List of products.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new GetProductsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get a single product by id.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The matching product.</response>
    /// <response code="404">No product exists with the given id.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <param name="command">Name, price and initial stock for the new product.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="201">Product created; the Location header points to the new resource.</response>
    /// <response code="400">Request failed validation (e.g. empty name, non-positive price).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await _dispatcher.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new CreateProductResponse(id));
    }
}
