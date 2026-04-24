using GoodHamburger.Api.Endpoints.OrderEndpoints;
using GoodHamburger.Api.Endpoints.OrderItemEndpoints;
using GoodHamburger.Api.Endpoints.ProductCategoryEndpoints;
using GoodHamburger.Api.Endpoints.ProductEndpoints;
using GoodHamburger.Api.Models.Responses;

namespace GoodHamburger.Api.Endpoints;

public static class EndpointsExtensions
{
    public static WebApplication MapOrderItemEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/order-items")
            .WithTags("Itens do Pedido");

        group.MapPost("", PostOrderItem.Handle)
            .WithDisplayName("CriarItemPedido")
            .WithDescription("Criar um item relacionado a um pedido")
            .Produces<OrderItemResponse>(StatusCodes.Status201Created)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("", GetOrderItems.Handle)
            .WithDisplayName("ListarItensPedido")
            .WithDescription("Listar todos os itens de pedido")
            .Produces<IEnumerable<OrderItemResponse>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetOrderItem.Handle)
            .WithDisplayName("ObterItemPedido")
            .WithDescription("Obter um item de pedido pelo id")
            .Produces<OrderItemResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", UpdateOrderItem.Handle)
            .WithDisplayName("AtualizarItemPedido")
            .WithDescription("Atualizar um item de pedido")
            .Produces<OrderItemResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", DeleteOrderItem.Handle)
            .WithDisplayName("ExcluirItemPedido")
            .WithDescription("Excluir um item de pedido")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        return app;
    }
    
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders")
            .WithTags("Pedidos");

        group.MapPost("", PostOrder.Handle)
            .WithDisplayName("CriarPedido")
            .WithDescription("Criar um novo pedido")
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("", GetOrders.Handle)
            .WithDisplayName("ListarPedidos")
            .WithDescription("Listar todos os pedidos")
            .Produces<IEnumerable<OrderResponse>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetOrder.Handle)
            .WithDisplayName("ObterPedido")
            .WithDescription("Obter um pedido pelo id")
            .Produces<OrderResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", UpdateOrder.Handle)
            .WithDisplayName("AtualizarPedido")
            .WithDescription("Atualizar um pedido")
            .Produces<OrderResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", DeleteOrder.Handle)
            .WithDisplayName("ExcluirPedido")
            .WithDescription("Excluir um pedido")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id}/confirm", ConfirmOrder.Handle)
            .WithDisplayName("ConfirmarPedido")
            .WithDescription("Confirmar um pedido pendente")
            .Produces<OrderResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id}/cancel", CancelOrder.Handle)
            .WithDisplayName("CancelarPedido")
            .WithDescription("Cancelar um pedido")
            .Produces<OrderResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapPatch("/{id}/status", UpdateOrderStatus.Handle)
            .WithDisplayName("AlterarStatusPedido")
            .WithDescription("Avançar o status de um pedido na progressão")
            .Produces<OrderResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ValidationResponse>(StatusCodes.Status400BadRequest)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        return app;
    }
    
    public static WebApplication MapProductCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/product-categories")
            .WithTags("Categorias de Produto");

        group.MapGet("", GetProductCategories.Handle)
            .WithDisplayName("ListarCategorias")
            .WithDescription("Listar todas as categorias")
            .Produces<IEnumerable<ProductCategoryResponse>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetProductCategory.Handle)
            .WithDisplayName("ObterCategoria")
            .WithDescription("Obter uma categoria pelo id")
            .Produces<ProductCategoryResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        return app;
    }
    
    public static WebApplication MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Cardápio");

        group.MapGet("", GetProducts.Handle)
            .WithDisplayName("ListarProdutos")
            .WithDescription("Listar todos os produtos")
            .Produces<IEnumerable<ProductResponse>>()
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetProduct.Handle)
            .WithDisplayName("ObterProduto")
            .WithDescription("Obter um produto pelo id")
            .Produces<ProductResponse>()
            .Produces<ValidationResponse>(StatusCodes.Status404NotFound)
            .Produces<ErrorResponse>(StatusCodes.Status500InternalServerError);

        return app;
    }
}