using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.CreateCustomer
{
    public record CreateCustomerRequest(string CustomerCode, string CustomerName);
    public record CreateCustomerResponse(Guid Id);
    public record CreateCustomerCommand(string CustomerCode, string CustomerName) : ICommand<CreateCustomerResponse>;
}