using CqrsDemo.Application.Common.Interfaces;
using CqrsDemo.Application.Interfaces;
using CqrsDemo.Application.Products.Dtos;

namespace CqrsDemo.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<ProductDto>> HandleAsync(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);

        return products
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, p.CreatedAtUtc))
            .ToList();
    }
}
