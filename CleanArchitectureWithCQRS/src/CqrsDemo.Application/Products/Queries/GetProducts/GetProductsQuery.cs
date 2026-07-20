using CqrsDemo.Application.Common.Interfaces;
using CqrsDemo.Application.Products.Dtos;

namespace CqrsDemo.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IQuery<List<ProductDto>>;
