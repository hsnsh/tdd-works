using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Products.API.Data;

public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ProductDbContext>()
            .UseSqlite("Data Source=../data/product.db;Cache=Shared", b =>
            {
                b.MigrationsHistoryTable("__EFMigrationsHistory");
            });

        return new ProductDbContext(builder.Options);
    }
}