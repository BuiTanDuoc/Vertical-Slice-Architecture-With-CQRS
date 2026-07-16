using Carter;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions;

namespace VerticalSliceArchitectureWithCQRS.Products
{
    public record GetProductByIdResponse(Product Product);
    public class GetProductByIdQuery(Guid id) : IQuery<GetProductByIdResponse>
    {
        public Guid Id { get; } = id;
    }

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Products/{id}", async (Guid id,
                IQueryHandler<GetProductByIdQuery, GetProductByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetProductByIdQuery(id), cancellationToken);

                var response = result.Adapt<GetProductByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetProductById")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Product By Id")
            .WithDescription("Get Product By Id");
        }
    }

    internal class GetProductByIdQueryHandler
        (AppDbContext context)
        : IQueryHandler<GetProductByIdQuery, GetProductByIdResponse>
    {
        public Task<GetProductByIdResponse> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var product = context.Products.FirstOrDefault(x => x.Id == query.Id);
            if (product is null)
            {
                throw new NotFoundException(query.Id.ToString());
            }
            return Task.FromResult(new GetProductByIdResponse(product));
        }
    }
}
