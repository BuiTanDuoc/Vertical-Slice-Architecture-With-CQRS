using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.DeleteCustomer
{
    public record DeleteCustomerResponse(bool IsSuccess);
    public record DeleteCustomerCommand(Guid Id) : ICommand<DeleteCustomerResponse>;
}