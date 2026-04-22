using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;

namespace GoodHamburger.Core.Services;

public class OrderItemService : ServiceBase<OrderItem>, IOrderItemService
{
    private readonly IOrderService _orderService;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    
    public OrderItemService(
        IOrderService orderService, 
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository) 
        : base(orderItemRepository)
    {
        _orderService = orderService;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
    }

    public override async Task<OrderItem> CreateAsync(OrderItem entity)
    {
        var order = await _orderService.GetByIdAsync(entity.OrderId);
        
        if (order == null)
            throw new EntityNotFoundException("Pedido", entity.OrderId);
        
        var product = await _productRepository.GetByIdAsync(entity.ProductId);
        
        if (product == null)
            throw new EntityNotFoundException("Produto", entity.Id);

        if (!product.IsActive)
            throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");
        
        entity.Product = product;
        order.Items.Add(entity);

        _orderService.ValidateDuplicateCategories(order.Items.Select(i => i.Product)!);
        _orderService.ValidateProductQuantity(order.Items);
        _orderService.RecalculateTotals(order);
        
        await _orderItemRepository.AddAsync(entity);
        await _orderService.UpdateAsync(order);
        
        return entity;
    }

    public override async Task UpdateAsync(OrderItem entity)
    {
        var order = await _orderService.GetByIdAsync(entity.OrderId);
        
        if (order == null)
            throw new EntityNotFoundException("Pedido", entity.OrderId);
        
        var product = await _productRepository.GetByIdAsync(entity.ProductId);
        
        if (product == null)
            throw new EntityNotFoundException("Produto", entity.Id);

        if (!product.IsActive)
            throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");
        
        entity.Product = product;
        order.Items.RemoveAll(i => i.Id == entity.Id);
        order.Items.Add(entity);
        
        _orderService.ValidateDuplicateCategories(order.Items.Select(i => i.Product)!);
        _orderService.ValidateProductQuantity(order.Items);
        _orderService.RecalculateTotals(order);
        
        await _orderService.UpdateAsync(order);
        await base.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(int id)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id);
        
        if (orderItem == null)
            throw new EntityNotFoundException("Item do Pedido", id);
        
        await base.DeleteAsync(id);
        
        var order = await _orderService.GetByIdAsync(orderItem.OrderId);
        if (order != null)
        {
            _orderService.RecalculateTotals(order);
            await _orderService.UpdateAsync(order);
        }
    }
}