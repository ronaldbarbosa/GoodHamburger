using GoodHamburger.Core.Entities.Shared;

namespace GoodHamburger.Core.Entities;

public class ProductCategory : Entity
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}