using Products.API.Models;

namespace Products.API.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAllProducts();
        IEnumerable<Product> GetPageProducts(int skipCount = 0, int maxResultCount = 10);
        long GetCount();

        Product GetProduct(int id);
        Product Add(Product product);
        Product Edit(Product product);
        void Delete(int id);
    }
}