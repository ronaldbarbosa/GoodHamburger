using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderEndpoints;

public static class PostOrder
{
    public static async Task<IResult> Handle(
        IOrderService orderService,
        CreateOrderRequest request)
    {
        try
        {
            var order = new Order
            {
                Items = request.Items?.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList() ?? []
            };
            
            var orderResult = await orderService.CreateAsync(order);
            
            var response = new OrderResponse(
                orderResult.Id,
                orderResult.Items.Select(oi => new OrderItemResponse(
                    oi.Id,
                    new ProductResponse(
                        oi.ProductId,
                        oi.Product?.Name ?? "",
                        oi.Product?.Price.ToString() ?? "0",
                        new ProductCategoryResponse(oi.Product?.Category?.Name ?? "")),
                    oi.Quantity,
                    oi.UnitPrice.ToString())).ToList(),
                orderResult.Subtotal.ToString(),
                orderResult.Discount.ToString(),
                orderResult.Total.ToString(),
                orderResult.CreatedAt,
                orderResult.Status.ToString());
            
            return Results.Created($"/api/orders/{orderResult.Id}", response);
        }
        catch (EntityNotFoundException ex)
        {
            var validation = new ValidationResponse([new ValidationItemResponse(ex.EntityType, ex.Message)]);
            return Results.BadRequest(validation);
        }
        catch (Exception ex) when(ex is DuplicateItemException or BusinessRuleViolationException)
        {
            var validation = new ValidationResponse([new ValidationItemResponse("", ex.Message)]);
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.InternalServerError("Erro ao processar solicitação. Tente novamente em alguns instantes.");
        }
    }
}