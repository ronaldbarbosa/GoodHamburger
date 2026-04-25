using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class PostOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        CreateOrderRequest request,
        CancellationToken ct)
    {
        var order = new Order
        {
            Items = request.Items!.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };

        var result = await orderService.CreateAsync(order, ct);

        var response = new OrderResponse(
            result.Id,
            result.Items.Select(oi => new OrderItemResponse(
                oi.Id,
                new ProductResponse(
                    oi.ProductId,
                    oi.Product!.Name,
                    oi.Product!.Price.Amount,
                    new ProductCategoryResponse(oi.Product!.CategoryId, oi.Product!.Category!.Name),
                    oi.Product!.ImageUrl),
                oi.Quantity,
                oi.UnitPrice.Amount)).ToList(),
            result.Subtotal.Amount,
            result.Discount.Amount,
            result.Total.Amount,
            result.CreatedAt,
            result.Status.ToString());

        return Results.Created($"/api/orders/{result.Id}", response);
    }
}
