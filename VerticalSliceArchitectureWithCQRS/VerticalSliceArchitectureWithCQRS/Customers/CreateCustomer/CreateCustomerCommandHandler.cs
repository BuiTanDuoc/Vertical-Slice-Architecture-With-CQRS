using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.CreateCustomer
{
    internal class CreateCustomerCommandHandler(AppDbContext context) : ICommandHandler<CreateCustomerCommand, CreateCustomerResponse>
    {
        public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
        {
            //todo: implement mapster
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                CustomerCode = command.CustomerCode,
                CustomerName = command.CustomerName
            };

            context.Customers.Add(customer);
            await context.SaveChangesAsync(cancellationToken);

            return new CreateCustomerResponse(customer.Id);
        }
    }
}
