using Carter;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.DeleteCustomer
{
    public class DeleteCustomerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/Customers/{id}", async (Guid id,
                    ICommandHandler<DeleteCustomerCommand, DeleteCustomerResponse> handler) =>
            {
                var result = await handler.Handle(new DeleteCustomerCommand(id));

                var response = result.Adapt<DeleteCustomerResponse>();

                return Results.Ok(response);
            })
            .WithName("DeleteCustomer")
            .Produces<DeleteCustomerResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Customer")
            .WithDescription("Delete Customer");
        }
    }
}