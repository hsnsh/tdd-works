using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Products.API.Controllers;
using Products.API.Models;
using Products.API.Services;
using Xunit;

namespace Products.API.Tests.UnitTests;

public class SampleControllerTests
{
    private readonly Mock<IItemRepository> _mockItemRepository = new();
    private readonly Mock<ILogger<SampleController>> _mockLogger = new();
    private readonly Random _rand = new();

    /// <summary>
    /// Test Function Implementation : UnitOfWork_StateUnderTest_ExpectedBehavior
    /// </summary>
    [Fact]
    public async Task GetItemsAsync_WithExistingItems_ReturnsAllItems()
    {
        // Arrange
        var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        _mockItemRepository.Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(expectedItems);

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var actualItems = await controller.GetItemsAsync();

        // Assert
        actualItems.Result.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)actualItems.Result!)?.Value;
        value.Should().BeEquivalentTo(expectedItems, opt => opt.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.Empty);

        // Assert
        // Other implementation => Assert.IsType<BadRequestResult>(result.Result)
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task GetItemAsync_WithUnExistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item)null!);

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
    {
        // Arrange
        var expectedItem = CreateRandomItem();
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.NewGuid());

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();

        var value = ((OkObjectResult)result.Result!)?.Value;
        value.Should().BeEquivalentTo(expectedItem, opt => opt.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task CreateItemAsync_WithNullItem_ReturnsBadRequest()
    {
        // Arrange
        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.CreateItemAsync(null);

        // Assert
        result.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
    {
        // Arrange
        var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new decimal(_rand.Next(1000)));

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.CreateItemAsync(itemToCreate);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();

        var createdItem = (result.Result as CreatedAtActionResult)?.Value as ItemDto;
        itemToCreate.Should().BeEquivalentTo(createdItem, opt => opt.ComparingByMembers<ItemDto>().ExcludingMissingMembers());
        createdItem?.Id.Should().NotBeEmpty();
        createdItem?.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(0, 0, 1));
    }

    [Theory]
    [MemberData(nameof(GetInvalidInputForUpdate))]
    public async Task UpdateItemAsync_WithInvalidParameters_ReturnsBadRequest(Guid itemId, UpdateItemDto itemToUpdate)
    {
        // Arrange
        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task UpdateItemAsync_WithUnExistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item)null!);

        var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), new decimal(_rand.Next(1000)));
        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.UpdateItemAsync(Guid.NewGuid(), itemToUpdate);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
    {
        // Arrange
        var existingItem = CreateRandomItem();
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var itemId = existingItem.Id;
        var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.DeleteItemAsync(Guid.Empty);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithUnExistingItem_ReturnsNotFound()
    {
        // Arrange
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item)null!);

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.DeleteItemAsync(Guid.NewGuid());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
    {
        // Arrange
        var existingItem = CreateRandomItem();
        _mockItemRepository.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var itemId = existingItem.Id;

        var controller = new SampleController(_mockItemRepository.Object, _mockLogger.Object);

        // Act
        var result = await controller.DeleteItemAsync(itemId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateRandomItem()
    {
        return new Item
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Description = Guid.NewGuid().ToString(),
            Price = new decimal(_rand.Next(1000)),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }

    public static IEnumerable<object?[]> GetInvalidInputForUpdate()
    {
        yield return new object?[] { Guid.Empty, null };
        yield return new object?[] { Guid.Empty, new UpdateItemDto("", "", 0) };
        yield return new object?[] { Guid.NewGuid(), null };
    }
}