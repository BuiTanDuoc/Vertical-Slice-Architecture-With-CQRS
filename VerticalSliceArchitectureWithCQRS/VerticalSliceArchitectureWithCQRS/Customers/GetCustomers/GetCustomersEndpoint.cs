using Carter;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Pagination;

namespace VerticalSliceArchitectureWithCQRS.Customers.GetCustomers
{
    public class GetCustomersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Customers", async ([AsParameters] PaginationRequest request,
                    IQueryHandler<GetCustomersQuery, GetCustomersResponse> handler, CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetCustomersQuery(request), cancellationToken);

                var response = result.Adapt<GetCustomersResponse>();

                return Results.Ok(response);
            })
            .WithName("GetCustomers")
            .WithTags("Customers")
            .Produces<GetCustomersResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Customers")
            .WithDescription("Get Customers");
        }
    }
}
