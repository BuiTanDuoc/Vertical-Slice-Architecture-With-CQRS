using CqrsDemo.Application.Common.Exceptions;
using CqrsDemo.Application.Common.Interfaces;
using CqrsDemo.Application.Interfaces;
using CqrsDemo.Application.Products.Dtos;

namespace CqrsDemo.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> HandleAsync(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), query.Id);

        return new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.CreatedAtUtc);
    }
}
