namespace VerticalSliceArchitectureWithCQRS.Models;

public class CustomerDto
{

    public Guid Id { get; set; }
    public string CustomerCode { get; set; } = default!;
    public string CustomerName { get; set; } = default!;

}

