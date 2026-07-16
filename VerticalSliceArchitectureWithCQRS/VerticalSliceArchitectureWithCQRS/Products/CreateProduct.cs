using Carter;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Products
{
    public record CreateProductRequest(string ProductCode, string ProductName, string ProductDescription);
    public record CreateProductResponse(Guid Id);
    public record CreateProductCommand(string ProductCode, string ProductName, string ProductDescription) : ICommand<CreateProductResponse>;

    internal class CreateProductCommandHandler(AppDbContext context) : ICommandHandler<CreateProductCommand, CreateProductResponse>
    {
        public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            //todo: implement mapster
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ProductCode = command.ProductCode,
                ProductName = command.ProductName,
                ProductDescription = command.ProductDescription,
                CraetedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            context.Products.Add(product);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateProductResponse(product.Id);
        }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
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

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Products",
                    async (
                        CreateProductRequest request,
                        [FromServices] IDispatcher dispatcher,
                        CancellationToken cancellationToken) =>
                    {
                        var command = request.Adapt<CreateProductCommand>();
                        var result = await dispatcher.Send<CreateProductCommand, CreateProductResponse>(command, cancellationToken);

                        return Results.Created($"/Products/{result.Id}", result);
                    })
                .WithName("CreateProduct")
                .WithTags("Products")
                .Produces<CreateProductResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Create Product")
                .WithDescription("Create Product");
        }
    }
}
