using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Products.API.Data;
using Products.API.Models;

namespace Products.API.Tests.Base
{
    public class ProductTestContext : ProductDbContext
    {
        public ProductTestContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData<Product>(modelBuilder, "../../../base/data/products.json");
        }

        private static void SeedData<T>(ModelBuilder modelBuilder, string file) where T : class
        {
            using var reader = new StreamReader(file);
            var json = reader.ReadToEnd();
            var data = JsonConvert.DeserializeObject<T[]>(json);
            modelBuilder.Entity<T>().HasData(data);
        }
    }
}