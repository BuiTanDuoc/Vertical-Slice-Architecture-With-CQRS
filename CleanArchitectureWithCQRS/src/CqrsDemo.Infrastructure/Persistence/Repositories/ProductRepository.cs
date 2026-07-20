using CqrsDemo.Application.Interfaces;
using CqrsDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken) =>
        _context.Products.AsNoTracking().OrderBy(p => p.CreatedAtUtc).ToListAsync(cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken) =>
        _context.Products.AddAsync(product, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _context.SaveChangesAsync(cancellationToken);
}
