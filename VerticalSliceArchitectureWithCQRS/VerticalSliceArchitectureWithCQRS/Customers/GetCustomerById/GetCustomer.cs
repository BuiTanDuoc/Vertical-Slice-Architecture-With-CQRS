using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;

namespace VerticalSliceArchitectureWithCQRS.Customers.GetCustomerById
{
    public record GetCustomerByIdResponse(Customer Customer);
    public class GetCustomerByIdQuery(Guid id) : IQuery<GetCustomerByIdResponse>
    {
        public Guid Id { get; } = id;
    }
}
