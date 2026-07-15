using VerticalSliceArchitectureWithCQRS.Models;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Pagination;

namespace VerticalSliceArchitectureWithCQRS.Customers.GetCustomers
{
    public class GetCustomersQuery(PaginationRequest request) : IQuery<GetCustomersResponse>
    {
        public PaginationRequest Request { get; } = request;
    }
    public record GetCustomersResponse(PaginatedResult<CustomerDto> Customers);
}