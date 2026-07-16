using Carter;
using Mapster;
using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Pagination;

namespace VerticalSliceArchitectureWithCQRS.Products
{
    public class GetProductsQuery(PaginationRequest request) : IQuery<GetProductsResponse>
    {
        public PaginationRequest Request { get; } = request;
    }
    public record GetProductsResponse(PaginatedResult<ProductDto> Products);

    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Products", async ([AsParameters] PaginationRequest request,
                    IQueryHandler<GetProductsQuery, GetProductsResponse> handler, CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetProductsQuery(request), cancellationToken);

                var response = result.Adapt<GetProductsResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces<GetProductsResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Products")
            .WithDescription("Get Products");
        }
    }

    public class GetProductsQueryHandler
    (AppDbContext context)
        : IQueryHandler<GetProductsQuery, GetProductsResponse>
    {
        public async Task<GetProductsResponse> Handle(GetProductsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.Request.PageIndex;
            var pageSize = query.Request.PageSize;

            var totalCount = await context.Products.LongCountAsync(cancellationToken);

            var products = await context.Products
                .OrderBy(o => o.ProductName)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var productDtos = products.ToProductDtoList();


            return new GetProductsResponse(
                new PaginatedResult<ProductDto>(
                    pageIndex,
                    pageSize,
                    totalCount,
                    productDtos));
        }
    }
}
