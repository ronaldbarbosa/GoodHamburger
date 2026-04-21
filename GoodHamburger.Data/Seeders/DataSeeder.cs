using GoodHamburger.Core.Entities;
using GoodHamburger.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Seeders;

public static class DataSeeder
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, Name = "Sanduíches", IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0)},
            new ProductCategory { Id = 2, Name = "Acompanhamentos", IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "X-Burger", Price = new Money(15.00m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new Product { Id = 2, Name = "X-Egg", Price = new Money(17.00m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new Product { Id = 3, Name = "X-Bacon", Price = new Money(19.00m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new Product { Id = 4, Name = "Batata Frita", Price = new Money(10.00m), CategoryId = 2, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new Product { Id = 5, Name = "Refrigerante", Price = new Money(5.00m), CategoryId = 2, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) }
        );
    }
}