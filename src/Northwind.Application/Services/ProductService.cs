// Service/ProductService.cs

using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services;

public class ProductService{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }
    // public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    // {
    //     return await _repository.GetAllAsync(cancellationToken);
    // }
    public Task<List<Product>> GetProductsAsync() => _repository.GetAllAsync();

}