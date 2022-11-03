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
        var products = _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("page-list")]
    public IActionResult GetPageProducts(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = int.MaxValue;

        int skipCount = (pageNumber - 1) * pageSize;
        var products = _productService.GetPageProducts(skipCount, pageSize);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        return Ok(_productService.GetProduct(id));
    }

    [HttpPost]
    public IActionResult Add(Product product)
    {
        var newProduct = _productService.Add(product);
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, null);
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

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        _productService.Delete(id);

        return NoContent();
    }
}