using Microsoft.AspNetCore.Mvc;
using Products.API.Models;
using Products.API.Services;

namespace Products.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly IItemRepository _repository;
    private readonly ILogger<SampleController> _logger;

    public SampleController(IItemRepository repository, ILogger<SampleController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // GET /items
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
    {
        var items = (await _repository.GetItemsAsync()).Select(item => item.AsDto());

        _logger.LogInformation("{S}: Retrieved {Count} items", DateTime.UtcNow.ToString("HH:mm:ss"), items.Count());

        return Ok(items);
    }

    // GET /items/{id}
    [HttpGet("{id:guid}")]
    public virtual async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) return BadRequest();

        var item = await _repository.GetItemAsync(id);
        if (item != null) return Ok(item.AsDto());

        _logger.LogInformation("{S} not found", id.ToString());
        return NotFound();
    }

    // POST /items
    [HttpPost]
    public virtual async Task<ActionResult<ItemDto>> CreateItemAsync([FromBody] CreateItemDto? input)
    {
        if (input == null) return BadRequest();

        var item = new Item() { Id = Guid.NewGuid(), Name = input.Name,Description = input.Description, Price = input.Price, CreatedDate = DateTimeOffset.UtcNow };

        await _repository.CreateItemAsync(item);

        return CreatedAtAction("GetItem", new { id = item.Id }, item.AsDto());
    }

    // PUT /items/{id}
    [HttpPut("{id:guid}")]
    public virtual async Task<IActionResult> UpdateItemAsync(Guid id, [FromBody] UpdateItemDto? input)
    {
        if (id.Equals(Guid.Empty)) return BadRequest();
        if (input == null) return BadRequest();

        var existingItem = await _repository.GetItemAsync(id);
        if (existingItem == null) return NotFound();

        existingItem.Name = input.Name;
        existingItem.Price = input.Price;

        await _repository.UpdateItemAsync(existingItem);

        return NoContent();
    }

    // DELETE /items/{id}
    [HttpDelete("{id:guid}")]
    public virtual async Task<IActionResult> DeleteItemAsync(Guid id)
    {
        if (id.Equals(Guid.Empty)) return BadRequest();

        var existingItem = await _repository.GetItemAsync(id);
        if (existingItem == null) return NotFound();

        await _repository.DeleteItemAsync(existingItem);

        return NoContent();
    }
}