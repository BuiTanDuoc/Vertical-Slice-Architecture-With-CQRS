using Carter;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.GetCustomerById
{
    public class GetCustomerByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/Customers/{id}", async (Guid id,
                IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetCustomerByIdQuery(id), cancellationToken);

                var response = result.Adapt<GetCustomerByIdResponse>();

                return Results.Ok(response);
            })
            .WithName("GetCustomerById")
            .Produces<GetCustomerByIdResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Customer By Id")
            .WithDescription("Get Customer By Id");
        }
    }
}
