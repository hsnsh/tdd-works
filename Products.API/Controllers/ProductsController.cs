using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        this._productService = productService;
    }

    [HttpGet]
    public IActionResult GetProducts()
    {
        var products = _productService.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        return Ok(_productService.GetProduct(id));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, Product product)
    {
        var existingProduct = _productService.GetProduct(id);
        if (existingProduct == null)
        {
            return BadRequest($"{id} id'li eleman yok.");
        }

        existingProduct.Id = id;
        return Ok(_productService.Edit(existingProduct));
    }

    [HttpPost]
    public IActionResult Add(Product product)
    {
        var newProduct = _productService.Add(product);
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, null);
    }
}