// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Data;

public class AppDbContext : DbContext{
    public DbSet<Product> Products => Set<Product>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}