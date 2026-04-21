using GoodHamburger.Core.Entities;
using GoodHamburger.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Data.Mappings;

public class OrderMap : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(OrderStatus.Pending);
        
        builder.Property(o => o.Subtotal)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);
        
        builder.Property(o => o.Discount)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);
        
        builder.Property(o => o.Total)
            .HasConversion<MoneyConverter>()
            .HasPrecision(18, 2);
        
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
    }
}