# Clean Architecture Example in .NET Core using CLI

## Table of Contents

1. [Overview](#overview)
2. [Project Structure](#project-structure)
3. [Tools and Technologies](#tools-and-technologies)
4. [CLI Setup](#cli-setup)
5. [Layer Responsibilities](#layer-responsibilities)
   - [1. Enterprise Business Rules (Domain)](#1-enterprise-business-rules-domain)
   - [2. Application Business Rules (Application)](#2-application-business-rules-application)
   - [3. Interface Adapters (Infrastructure)](#3-interface-adapters-infrastructure)
   - [4. Frameworks & Drivers (Web API)](#4-frameworks--drivers-web-api)
6. [Dependency Rule Explanation](#dependency-rule-explanation)
7. [Testing the API](#testing-the-api)
8. [Conclusion](#conclusion)

---

## Overview

This document presents a professional Clean Architecture implementation in .NET Core using the `dotnet CLI`. The example demonstrates how to separate an application into layers and ensure dependencies only point inward. It uses EF Core with an in-memory provider.

---

## Project Structure

```text
/CleanArchitectureDemo
│
├── src/
│   ├── Northwind.Domain                 # Enterprise Business Rules
│   ├── Northwind.Application            # Application Business Rules
│   ├── Northwind.Infrastructure         # Interface Adapters
│   └── Northwind.WebApi                 # Frameworks & Drivers (Web API)
```

---

## Tools and Technologies

- .NET SDK 8+
- Entity Framework Core In-Memory
- dotnet CLI
- ASP.NET Core Web API

---

## CLI Setup

```bash
# Create the solution and projects
mkdir CleanArchitectureDemo && cd CleanArchitectureDemo

dotnet new sln -n CleanArchitectureDemo

dotnet new classlib -n Northwind.Domain -o src/Northwind.Domain

dotnet new classlib -n Northwind.Application -o src/Northwind.Application

dotnet new classlib -n Northwind.Infrastructure -o src/Northwind.Infrastructure

dotnet new webapi -n Northwind.WebApi -o src/Northwind.WebApi

# Add to solution
cd src
for proj in Northwind.*; do dotnet sln ../CleanArchitectureDemo.sln add $proj/$proj.csproj; done

# Windows
# if you are in 'src'
Get-ChildItem -Directory Northwind.* | ForEach-Object {
    dotnet sln ../CleanArchitectureDemo.sln add "$($_.Name)/$($_.Name).csproj"
}


# Set project references
cd Northwind.Application
 dotnet add reference ../Northwind.Domain/Northwind.Domain.csproj
cd ../Northwind.Infrastructure
 dotnet add reference ../Northwind.Application/Northwind.Application.csproj
cd ../Northwind.WebApi
 dotnet add reference ../Northwind.Infrastructure/Northwind.Infrastructure.csproj

# Add EF Core InMemory
cd ../Northwind.Infrastructure
 dotnet add package Microsoft.EntityFrameworkCore.InMemory
 dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions

dotnet add src/Northwind.WebApi package Swashbuckle.AspNetCore
```

---

## Layer Responsibilities

### 1. Enterprise Business Rules (Domain)
```csharp
// Entities/Product.cs
namespace Northwind.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
```

### 2. Application Business Rules (Application)
```csharp
// Interfaces/IProductRepository.cs
using Northwind.Domain.Entities;

namespace Northwind.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
}
```

```csharp
// Services/ProductService.cs
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repository;
    public ProductService(IProductRepository repository) => _repository = repository;
    public Task<List<Product>> GetProductsAsync() => _repository.GetAllAsync();
}
```

### 3. Interface Adapters (Infrastructure)
```csharp
// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Northwind.Domain.Entities;

namespace Northwind.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
}
```

```csharp
// Repositories/InMemoryProductRepository.cs
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Data;

namespace Northwind.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public InMemoryProductRepository(AppDbContext context) => _context = context;

    public Task<List<Product>> GetAllAsync() => _context.Products.ToListAsync();
}
```

```csharp
// DependencyInjection.cs
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
```

### 4. Frameworks & Drivers (Web API)
```csharp
// Program.cs
using Northwind.Application.Services;
using Northwind.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddScoped<ProductService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
```

```csharp
// Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Services;

namespace Northwind.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _service;
    public ProductController(ProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var products = await _service.GetProductsAsync();
        return Ok(products);
    }
}
```

---

## Dependency Rule Explanation

- Domain has **no dependencies**.
- Application depends **only** on Domain.
- Infrastructure depends on Application and Domain.
- Web API depends on Infrastructure (never the other way).

This follows **Clean Architecture's core principle**: Dependencies always point inward.

---

## Testing the API

1. Set `Northwind.WebApi` as the startup project:
   ```bash
   dotnet run --project src/Northwind.WebApi
   ```
2. Open Swagger at `https://localhost:{port}/swagger`
3. Test the `GET /api/product` endpoint

---

## Conclusion

This project provides a minimal yet complete implementation of Clean Architecture in .NET. It strictly enforces the Dependency Rule, separates responsibilities per layer, and uses modern techniques like Dependency Injection and EF Core InMemory.

You can now scale this with more use cases, richer entities, validation, and test projects.

Happy coding!

