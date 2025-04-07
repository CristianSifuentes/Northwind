using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Data;

namespace Northwind.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository {
    private readonly AppDbContext _context;

    public InMemoryProductRepository(AppDbContext context)
    {
        _context = context;
    }
    public Task<List<Product>> GetAllAsync() => _context.Products.ToListAsync();

    // public Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    // {
    //     return _context.Products.ToListAsync(cancellationToken);
    // }

    // Other methods (Add, Update, Delete) can be implemented similarly
}
