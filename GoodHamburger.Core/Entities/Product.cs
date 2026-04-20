using GoodHamburger.Core.Entities.Shared;
using GoodHamburger.Core.ValueObjects;

namespace GoodHamburger.Core.Entities;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;
    public Money Price { get; set; }
    public int CategoryId { get; set; }
    public ProductCategory? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}