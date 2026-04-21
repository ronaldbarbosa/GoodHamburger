using GoodHamburger.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Data.Mappings;

public class ProductCategoryMap : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        
        builder.HasKey(pc => pc.Id);
        
        builder.Property(pc => pc.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(pc => pc.Name)
            .IsUnique();
    }
}