using Microsoft.EntityFrameworkCore;
using VerticalSliceArchitectureWithCQRS.Models;

namespace VerticalSliceArchitectureWithCQRS.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = Guid.Parse("e455f48f-35d1-4fa4-aaf1-4f7fcf5da22a"),
                    CustomerCode = "CUST-001",
                    CustomerName = "John Doe"
                },
                new Customer
                {
                    Id = Guid.Parse("0c30022b-8617-47ec-8e2d-6f327f507084"),
                    CustomerCode = "CUST-002",
                    CustomerName = "Jane Smith"
                }
            );
        }
    }
}
