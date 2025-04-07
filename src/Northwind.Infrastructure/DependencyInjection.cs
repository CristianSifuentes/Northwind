using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Northwind.Infrastructure.Data;
using Northwind.Infrastructure.Repositories;
using Northwind.Application.Interfaces;

namespace Northwind.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("NorthwindDb"));
        services.AddScoped<IProductRepository, InMemoryProductRepository>();
        return services;
    }
}