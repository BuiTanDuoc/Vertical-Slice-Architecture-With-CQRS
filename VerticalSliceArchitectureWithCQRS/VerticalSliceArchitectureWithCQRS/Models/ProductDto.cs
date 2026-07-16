namespace VerticalSliceArchitectureWithCQRS.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string ProductDescription { get; set; } = string.Empty;
        public DateOnly CraetedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
