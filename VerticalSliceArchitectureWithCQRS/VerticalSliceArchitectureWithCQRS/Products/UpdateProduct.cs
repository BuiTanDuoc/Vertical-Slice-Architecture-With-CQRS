using Carter;
using FluentValidation;
using Mapster;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions;

namespace VerticalSliceArchitectureWithCQRS.Products
{
    public record UpdateProductRequest(Guid Id, string ProductCode, string ProductName, string ProductDescription);
    public record UpdateProductResponse(bool IsSuccess, string? ErrorMessage = null, Product? UpdatedProduct = null);
    public record UpdateProductCommand(Guid Id, string ProductCode, string ProductName, string ProductDescription)
        : ICommand<UpdateProductResponse>;

    internal class UpdateProductCommandHandler(AppDbContext context)
        : ICommandHandler<UpdateProductCommand, UpdateProductResponse>
    {
        public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await context.Products.FindAsync([command.Id], cancellationToken);

            if (product is null)
            {
                throw new NotFoundException(command.Id.ToString());
            }

            product.ProductCode = command.ProductCode;
            product.ProductName = command.ProductName;
            product.ProductDescription = command.ProductDescription;

            context.Products.Update(product);
            await context.SaveChangesAsync(cancellationToken);

            return new UpdateProductResponse(true, null, product);
        }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");

            RuleFor(x => x.ProductCode)
                .NotEmpty()
                .WithMessage("Product code is required.")
                .Matches(@"^PROD-\d{3}$")
                .WithMessage("Product code must follow the format 'CUST-xxx', where 'xxx' are three digits (e.g., CUST-002).");

            RuleFor(x => x.ProductName)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        }
    }

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/Products",
                async (UpdateProductRequest request,
                    ICommandHandler<UpdateProductCommand, UpdateProductResponse> handler) =>
                {
                    var command = request.Adapt<UpdateProductCommand>();

                    var result = await handler.Handle(command);

                    var response = result.Adapt<UpdateProductResponse>();

                    return Results.Ok(response);
                })
                .WithName("UpdateProduct")
                .WithTags("Products")
                .Produces<UpdateProductResponse>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithSummary("Update Product")
                .WithDescription("Update Product");
        }
    }
}
