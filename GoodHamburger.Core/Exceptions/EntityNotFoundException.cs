namespace GoodHamburger.Core.Exceptions;

public class EntityNotFoundException(string entityType, int id) 
    : Exception($"{entityType} com ID {id} não encontrada.")
{
    public int EntityId { get; } = id;
    public string EntityType { get; } = entityType;
}