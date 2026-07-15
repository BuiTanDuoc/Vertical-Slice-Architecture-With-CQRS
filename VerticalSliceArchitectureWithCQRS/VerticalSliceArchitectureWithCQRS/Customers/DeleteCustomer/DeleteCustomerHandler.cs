
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions;

namespace VerticalSliceArchitectureWithCQRS.Customers.DeleteCustomer
{
    internal class DeleteCustomerCommandHandler(CustomerContext context)
        : ICommandHandler<DeleteCustomerCommand, DeleteCustomerResponse>
    {
        public async Task<DeleteCustomerResponse> Handle(DeleteCustomerCommand command, CancellationToken cancellationToken)
        {
            var customer = await context.Customers.FindAsync(new object[] { command.Id }, cancellationToken);

            if (customer is null)
            {
                throw new NotFoundException(command.Id.ToString());
            }

            context.Customers.Remove(customer);
            await context.SaveChangesAsync(cancellationToken);

            return new DeleteCustomerResponse(true);
        }
    }
}