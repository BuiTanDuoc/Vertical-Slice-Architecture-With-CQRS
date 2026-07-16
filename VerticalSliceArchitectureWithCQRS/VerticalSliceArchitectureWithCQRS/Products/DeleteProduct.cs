using Carter;
using FluentValidation;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions;

namespace VerticalSliceArchitectureWithCQRS.Products
{
    public record DeleteProductResponse(bool IsSuccess);
    public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResponse>;

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required");
        }
    }

    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/Products/{id}", async (Guid id,
                    ICommandHandler<DeleteProductCommand, DeleteProductResponse> handler) =>
            {
                var result = await handler.Handle(new DeleteProductCommand(id));

                var response = result.Adapt<DeleteProductResponse>();

                return Results.Ok(response);
            })
            .WithName("DeleteProduct")
            .WithTags("Products")
            .Produces<DeleteProductResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
        }
    }

    internal class DeleteProductCommandHandler(AppDbContext context)
        : ICommandHandler<DeleteProductCommand, DeleteProductResponse>
    {
        public async Task<DeleteProductResponse> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await context.Products.FindAsync(new object[] { command.Id }, cancellationToken);

            if (product is null)
            {
                throw new NotFoundException(command.Id.ToString());
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync(cancellationToken);

            return new DeleteProductResponse(true);
        }
    }
}
