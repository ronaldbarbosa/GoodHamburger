using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class UpdateOrderStatus
{
    private static readonly HashSet<OrderStatus> TerminalStatuses = [OrderStatus.Completed, OrderStatus.Cancelled];
    private static readonly HashSet<OrderStatus> ForbiddenTargets = [OrderStatus.Pending, OrderStatus.Cancelled];

    public static async Task<IResult> Handle(
        IOrderService orderService,
        int id,
        UpdateOrderStatusRequest request)
    {
        try
        {
            if (!Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus))
            {
                var invalid = new ValidationResponse([new ValidationItemResponse("status", "Status inválido.")]);
                return Results.BadRequest(invalid);
            }

            var order = await orderService.GetByIdAsync(id);

            if (order is null)
            {
                var notFound = new ValidationResponse([new ValidationItemResponse("id", "Pedido não encontrado.")]);
                return Results.NotFound(notFound);
            }

            if (TerminalStatuses.Contains(order.Status))
            {
                var terminal = new ValidationResponse([new ValidationItemResponse("status", "Este pedido não pode ser alterado.")]);
                return Results.BadRequest(terminal);
            }

            if (ForbiddenTargets.Contains(newStatus))
            {
                var forbidden = new ValidationResponse([new ValidationItemResponse("status", "Status inválido para esta operação.")]);
                return Results.BadRequest(forbidden);
            }

            if ((int)newStatus <= (int)order.Status)
            {
                var backwards = new ValidationResponse([new ValidationItemResponse("status", "O status deve avançar na progressão do pedido.")]);
                return Results.BadRequest(backwards);
            }

            order.Status = newStatus;
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
        catch (Exception)
        {
            return Results.InternalServerError(new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes."));
        }
    }
}
