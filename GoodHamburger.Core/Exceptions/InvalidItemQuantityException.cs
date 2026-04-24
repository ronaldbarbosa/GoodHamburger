namespace GoodHamburger.Core.Exceptions;

public class InvalidItemQuantityException() : Exception("Cada item do pedido deve ter quantidade igual a 1")
{
    public string EntityType { get; } = "quantity";
}