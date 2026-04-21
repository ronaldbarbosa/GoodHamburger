using GoodHamburger.Api.Models.Requests;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Services;

namespace GoodHamburger.Api.Endpoints.OrderItemEndpoints;

public static class PostOrderItem
{
    public static async Task<IResult> Handle(
        IOrderItemService orderItemService,
        CreateOrderItemRequest  request)
    {
        try
        {
            if (request.OrderId is null)
            {
                var validation = new ValidationResponse([new ValidationItemResponse("orderId", "É necessário informar o id do pedido")]);
                return Results.BadRequest(validation);
            }
            
            var orderItem = new OrderItem
            {
                OrderId = (int)request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            
            var orderItemResult = await orderItemService.CreateAsync(orderItem);
            
            var response = new OrderItemResponse(
                orderItemResult.Id,
                new ProductResponse(
                    orderItemResult.ProductId, 
                    orderItemResult.Product!.Name,
                    orderItemResult.Product.Price.ToString(),
                    new ProductCategoryResponse(orderItemResult.Product.Category!.Name)),
                orderItemResult.Quantity,
                orderItemResult.UnitPrice.ToString());
            
            return Results.Created($"/api/order-items/{orderItemResult.Id}", response);
        }
        catch (EntityNotFoundException ex)
        {
            var validation = new ValidationResponse([new ValidationItemResponse(ex.EntityType, ex.Message)]);
            return Results.BadRequest(ex.Message);
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