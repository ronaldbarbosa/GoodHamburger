using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class DeleteOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        int id)
    {
        try
        {
            var existingOrderItem = await orderItemService.GetByIdAsync(id);
            
            if (existingOrderItem is null)
            {
                return Results.NotFound(new { message = "Item de pedido não encontrado" });
            }
            
            await orderItemService.DeleteAsync(id);
            
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}