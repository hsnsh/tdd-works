using Microsoft.EntityFrameworkCore;
using Products.API.Data;
using Products.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace Products.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _dbContext;

        public ProductService(ProductDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public Product Add(Product product)
        {
            var entity = _dbContext.Products.Add(product).Entity;
            _dbContext.SaveChanges();
            return entity;
        }

        public Product Delete(int id)
        {
            var product = _dbContext.Products.Find(id);
            var deletedProduct = _dbContext.Products.Remove(product).Entity;
            return deletedProduct;
        }

        public Product Edit(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return product;
        }

        public Product GetProduct(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public IEnumerable<Product> GetProducts()
        {
            return _dbContext.Products.ToList();
        }
    }
}