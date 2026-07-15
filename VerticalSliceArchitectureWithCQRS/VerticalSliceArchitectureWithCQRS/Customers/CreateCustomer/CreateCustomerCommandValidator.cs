using FluentValidation;

namespace VerticalSliceArchitectureWithCQRS.Customers.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerCode)
                .NotEmpty()
                .WithMessage("Customer code is required.")
                .Matches(@"^CUST-\d{3}$")
                .WithMessage("Customer code must follow the format 'CUST-xxx', where 'xxx' are three digits (e.g., CUST-002).");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Name is required")
                .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");
        }
    }
}