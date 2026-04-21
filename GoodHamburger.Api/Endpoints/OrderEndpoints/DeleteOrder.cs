using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class DeleteOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id)
    {
        try
        {
            var existingOrder = await orderService.GetByIdAsync(id);
            
            if (existingOrder is null)
            {
                return Results.NotFound(new { message = "Pedido não encontrado" });
            }
            
            await orderService.DeleteAsync(id);
            
            return Results.NoContent();
        }
        catch (Exception)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}