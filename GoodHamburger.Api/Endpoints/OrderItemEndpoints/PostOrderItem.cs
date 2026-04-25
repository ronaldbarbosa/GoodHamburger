using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class PostOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        CreateOrderItemRequest request,
        CancellationToken ct)
    {
        var orderItem = new OrderItem
        {
            OrderId = request.OrderId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var result = await orderItemService.CreateAsync(orderItem, ct);

        var response = new OrderItemResponse(
            result.Id,
            new ProductResponse(
                result.ProductId,
                result.Product!.Name,
                result.Product.Price.ToString(),
                new ProductCategoryResponse(result.Product!.CategoryId, result.Product!.Category!.Name),
                result.Product!.ImageUrl),
            result.Quantity,
            result.UnitPrice.ToString());

        return Results.Created($"/api/order-items/{result.Id}", response);
    }
}
