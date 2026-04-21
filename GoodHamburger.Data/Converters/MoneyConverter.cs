using GoodHamburger.Core.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GoodHamburger.Data.Converters;

public sealed class MoneyConverter : ValueConverter<Money, decimal>
{
    public MoneyConverter() 
        : base(
            v => v.Amount,
            v => new Money(v)
        )
    {
    }
}