using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;

namespace GoodHamburger.Core.Services;

public class OrderItemService : ServiceBase<OrderItem>, IOrderItemService
{
    private readonly IOrderService _orderService;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderItemService(
        IOrderService orderService,
        IOrderItemRepository orderItemRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
        : base(orderItemRepository, unitOfWork)
    {
        _orderService = orderService;
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<OrderItem> CreateAsync(OrderItem entity, CancellationToken ct = default)
    {
        var order = await _orderService.GetByIdAsync(entity.OrderId, ct);

        if (order == null)
            throw new EntityNotFoundException("Pedido", entity.OrderId);

        var product = await _productRepository.GetByIdAsync(entity.ProductId, ct);

        if (product == null)
            throw new EntityNotFoundException("Produto", entity.ProductId);

        if (!product.IsActive)
            throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");

        entity.Product = product;
        entity.UnitPrice = product.Price;
        order.Items.Add(entity);

        _orderService.ValidateDuplicateCategories(order.Items.Select(i => i.Product)!);
        _orderService.ValidateProductQuantity(order.Items);
        _orderService.RecalculateTotals(order);

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await _orderItemRepository.AddAsync(entity, ct);
            await _orderService.UpdateAsync(order, ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }

        return entity;
    }

    public override async Task UpdateAsync(OrderItem entity, CancellationToken ct = default)
    {
        var order = await _orderService.GetByIdAsync(entity.OrderId, ct);

        if (order == null)
            throw new EntityNotFoundException("Pedido", entity.OrderId);

        var product = await _productRepository.GetByIdAsync(entity.ProductId, ct);

        if (product == null)
            throw new EntityNotFoundException("Produto", entity.ProductId);

        if (!product.IsActive)
            throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");

        entity.Product = product;
        entity.UnitPrice = product.Price;
        order.Items.RemoveAll(i => i.Id == entity.Id);
        order.Items.Add(entity);

        _orderService.ValidateDuplicateCategories(order.Items.Select(i => i.Product)!);
        _orderService.ValidateProductQuantity(order.Items);
        _orderService.RecalculateTotals(order);

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await _orderService.UpdateAsync(order, ct);
            await base.UpdateAsync(entity, ct);
            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    public override async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var orderItem = await _orderItemRepository.GetByIdAsync(id, ct);

        if (orderItem == null)
            throw new EntityNotFoundException("Item do Pedido", id);

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await base.DeleteAsync(id, ct);

            var order = await _orderService.GetByIdAsync(orderItem.OrderId, ct);
            if (order != null)
            {
                _orderService.RecalculateTotals(order);
                await _orderService.UpdateAsync(order, ct);
            }

            await _unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
}
