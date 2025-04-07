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