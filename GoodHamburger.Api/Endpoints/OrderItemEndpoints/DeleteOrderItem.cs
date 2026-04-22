using GoodHamburger.Api.Models.Responses;
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
                var validation = new ValidationResponse([new ValidationItemResponse("id", "Item do pedido não foi encontrado.")]);
                return Results.NotFound(validation);
            }
            
            await orderItemService.DeleteAsync(id);
            
            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}