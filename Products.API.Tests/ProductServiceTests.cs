using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Services;
using Xunit;

namespace Products.API.Tests;

public class ProductServiceTests
{
    private readonly DbContextOptions<ProductDbContext> _dbOptions;

    public ProductServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: "in-memory")
            .Options;
    }

    [Fact]
    public async Task Get_All_Products_Is_Not_Null()
    {
        //Arrange
        var productContext = new ProductTestContext(_dbOptions);
        var productService = new ProductService(productContext);

        //Act
        var response = productService.GetAllProducts();

        //Assert
        response.Should().NotBeNull();
    }
}