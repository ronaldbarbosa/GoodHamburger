using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
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
                var validation = new ValidationResponse([new ValidationItemResponse("id", "Pedido não encontrado.")]);
                return Results.NotFound(validation);
            }

            if (existingOrder.Status != OrderStatus.Cancelled)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("status", "Apenas pedidos cancelados podem ser excluídos.")]);
                return Results.BadRequest(validation);
            }

            await orderService.DeleteAsync(id);

            return Results.NoContent();
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}