using CqrsDemo.Application.Common.Interfaces;
using CqrsDemo.Application.Products.Dtos;

namespace CqrsDemo.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<ProductDto>;
