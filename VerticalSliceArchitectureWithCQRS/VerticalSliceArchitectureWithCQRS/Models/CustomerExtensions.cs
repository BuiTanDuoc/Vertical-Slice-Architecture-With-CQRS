namespace VerticalSliceArchitectureWithCQRS.Models;

public static class CustomerExtensions
{
    public static IEnumerable<CustomerDto> ToCustomerDtoList(this IEnumerable<Customer> customers)
    {
        return customers.Select(customer => new CustomerDto
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            CustomerName = customer.CustomerName
        });
    }
}
