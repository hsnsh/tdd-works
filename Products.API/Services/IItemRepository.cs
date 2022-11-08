using Products.API.Models;

namespace Products.API.Services;

public interface IItemRepository
{
    Task<IEnumerable<Item>> GetItemsAsync();
    Task<Item?> GetItemAsync(Guid id);

    Task<Item?> CreateItemAsync(Item input);
    
    Task UpdateItemAsync(Item input);
    
    Task DeleteItemAsync(Item input);
}