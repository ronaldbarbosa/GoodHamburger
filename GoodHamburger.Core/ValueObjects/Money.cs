namespace GoodHamburger.Core.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; init; }

    public Money(decimal amount)
    {
        Amount = Math.Round(amount, 2);
    }

    public static Money Zero => new(0m);

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money a, decimal factor) => new(a.Amount * factor);
    public static Money operator *(decimal factor, Money a) => new(a.Amount * factor);

    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => a.Amount < b.Amount;
    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => a.Amount <= b.Amount;

    public override string ToString() => $"R$ {Amount:N2}";
}