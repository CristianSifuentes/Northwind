// Interface/IProductRepository.cs
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        // Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        // Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        // Task AddAsync(Product product, CancellationToken cancellationToken = default);
        // Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        // Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}