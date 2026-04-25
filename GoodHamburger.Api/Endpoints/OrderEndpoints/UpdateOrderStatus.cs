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
        UpdateOrderStatusRequest request,
        CancellationToken ct)
    {
        Enum.TryParse<OrderStatus>(request.Status, ignoreCase: true, out var newStatus);

        var order = await orderService.GetByIdAsync(id, ct);

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
        await orderService.UpdateAsync(order, ct);

        var response = new OrderResponse(
            order.Id,
            order.Items.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId,
                    oi.Product!.Name,
                    oi.Product!.Price.Amount,
                    new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                    oi.Product!.ImageUrl),
                oi.Quantity,
                oi.UnitPrice.Amount)).ToList(),
            order.Subtotal.Amount,
            order.Discount.Amount,
            order.Total.Amount,
            order.CreatedAt,
            order.Status.ToString());

        return Results.Ok(response);
    }
}
