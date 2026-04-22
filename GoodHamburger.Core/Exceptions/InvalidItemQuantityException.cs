namespace GoodHamburger.Core.Exceptions;

public class InvalidItemQuantityException() : Exception("A quantidade de itens de um produto deve ser maior que zero")
{
    public string EntityType { get; } = "quantity";
}