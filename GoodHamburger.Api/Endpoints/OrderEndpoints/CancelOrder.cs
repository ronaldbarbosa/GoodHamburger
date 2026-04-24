using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class CancelOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id)
    {
        try
        {
            var order = await orderService.GetByIdAsync(id);

            if (order is null)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("id", "Pedido não encontrado.")]);
                return Results.NotFound(validation);
            }

            if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("status", "Pedidos concluídos ou já cancelados não podem ser cancelados.")]);
                return Results.BadRequest(validation);
            }

            order.Status = OrderStatus.Cancelled;
            await orderService.UpdateAsync(order);

            var response = new OrderResponse(
                order.Id,
                order.Items.Select(oi => new OrderItemResponse(
                    oi.Id,
                    new ProductResponse(
                        oi.ProductId,
                        oi.Product!.Name,
                        oi.Product!.Price.ToString(),
                        new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name)),
                    oi.Quantity,
                    oi.UnitPrice.ToString())).ToList(),
                order.Subtotal.ToString(),
                order.Discount.ToString(),
                order.Total.ToString(),
                order.CreatedAt,
                order.Status.ToString());

            return Results.Ok(response);
        }
        catch (Exception ex) when (ex is BusinessRuleViolationException or EntityNotFoundException)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("", ex.Message)]);
            return Results.BadRequest(validation);
        }
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}
