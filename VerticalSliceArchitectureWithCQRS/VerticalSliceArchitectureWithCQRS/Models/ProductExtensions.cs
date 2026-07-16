namespace VerticalSliceArchitectureWithCQRS.Models
{
    public static class ProductExtensions
    {
        public static IEnumerable<ProductDto> ToProductDtoList(this IEnumerable<Product> products)
        {
            return products.Select(product => new ProductDto
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                CraetedDate = product.CraetedDate,
            });
        }
    }
}
