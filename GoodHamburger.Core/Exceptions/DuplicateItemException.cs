namespace GoodHamburger.Core.Exceptions;

public class DuplicateItemException(string categoryName)
    : Exception($"Já existe um item da categoria '{categoryName}' neste pedido.")
{
    public string CategoryName { get; } = categoryName;
}