using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Models;

namespace Products.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _dbContext;
        private readonly DbSet<Product> _dbSet;

        public ProductService(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Product>();
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _dbSet.ToList();
        }

        public IEnumerable<Product> GetPageProducts(int skipCount = 0, int maxResultCount = 10)
        {
            return _dbSet.AsQueryable()
                .OrderBy(x => x.Id)
                .Skip(skipCount).Take(maxResultCount)
                .ToList();
        }

        public long GetCount()
        {
            return _dbSet.LongCount();
        }

        public Product GetProduct(int id)
        {
            return _dbSet.FirstOrDefault(x => x.Id.Equals(id));
        }

        public Product Add(Product product)
        {
            var savedEntity = _dbSet.Add(product).Entity;
            _dbContext.SaveChanges();
            return savedEntity;
        }

        public Product Edit(Product product)
        {
            _dbContext.Attach(product);

            var updatedEntity = _dbContext.Update(product).Entity;

            _dbContext.SaveChanges();

            return updatedEntity;
        }

        public void Delete(int id)
        {
            var product = _dbSet.FirstOrDefault(x => x.Id.Equals(id));
            if (product == null) return;

            if (_dbContext.Entry(product).State == EntityState.Detached)
            {
                _dbSet.Attach(product);
            }

            _dbSet.Remove(product);

            _dbContext.SaveChanges();
        }
    }
}