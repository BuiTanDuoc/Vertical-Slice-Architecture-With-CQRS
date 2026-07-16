using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions;

namespace VerticalSliceArchitectureWithCQRS.Customers.GetCustomerById
{
    internal class GetCustomerByIdQueryHandler
        (AppDbContext context)
        : IQueryHandler<GetCustomerByIdQuery, GetCustomerByIdResponse>
    {
        public Task<GetCustomerByIdResponse> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
        {
            var customer = context.Customers.FirstOrDefault(x => x.Id == query.Id);
            if (customer is null)
            {
                throw new NotFoundException(query.Id.ToString());
            }
            return Task.FromResult(new GetCustomerByIdResponse(customer));
        }
    }
}
