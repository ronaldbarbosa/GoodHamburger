using GoodHamburger.Core.Entities;
using GoodHamburger.Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Data.Seeders;

public static class DataSeeder
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>().HasData(
            new ProductCategory { Id = 1, Name = "Sanduíches", IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new ProductCategory { Id = 2, Name = "Acompanhamentos", IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) },
            new ProductCategory { Id = 3, Name = "Bebidas", IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0) }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "X-Burger", Price = new Money(5.00m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0), ImageUrl = "https://img77.uenicdn.com/image/upload/v1616473574/business/74412a32-557c-424b-9a43-2dfd3c8a01ab.jpg"},
            new Product { Id = 2, Name = "X-Egg", Price = new Money(4.50m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0), ImageUrl = "https://paulinlanches.pedidoturbo.com.br/_core/_uploads/129/2023/01/1648240123i0iafc9kji.jpeg" },
            new Product { Id = 3, Name = "X-Bacon", Price = new Money(7.00m), CategoryId = 1, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0), ImageUrl = "https://imagens.jotaja.com/produtos/2607/8845691EA121444DA9B3A15705E0F7CEC7DDCC8D53ABC58CFC044C87711E87EF.jpeg" },
            new Product { Id = 4, Name = "Batata Frita", Price = new Money(2.00m), CategoryId = 2, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0), ImageUrl = "https://latourangelle.com/cdn/shop/articles/hikynvl8pjkjqhvpnok6_1200x.jpg?v=1619198610" },
            new Product { Id = 5, Name = "Refrigerante", Price = new Money(2.50m), CategoryId = 3, IsActive = true, CreatedAt = new DateTime(2026, 4, 21, 17, 0, 0), ImageUrl = "https://hortifrutibr.vtexassets.com/arquivos/ids/173841/Refrigerante-Coca-Cola-Lata-350ml-gelada.jpg.jpg?v=638931057551370000" }
        );
    }
}