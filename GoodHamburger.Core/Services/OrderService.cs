using GoodHamburger.Core.Entities;
using GoodHamburger.Core.Exceptions;
using GoodHamburger.Core.Interfaces.Repositories;
using GoodHamburger.Core.Interfaces.Services;
using GoodHamburger.Core.Services.Shared;

namespace GoodHamburger.Core.Services;

public class OrderService : ServiceBase<Order>, IOrderService
{
    private readonly IOrderRepository _orderOrderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDiscountCalculatorService _discountCalculatorService;

    public OrderService(
        IOrderRepository orderOrderItemRepository,
        IProductRepository productRepository,
        IDiscountCalculatorService discountCalculatorService) : base(orderOrderItemRepository)
    {
        _orderOrderItemRepository = orderOrderItemRepository;
        _productRepository = productRepository;
        _discountCalculatorService = discountCalculatorService;
    }

    public override async Task<Order> CreateAsync(Order entity)
    {
        var products = new List<Product>();

        foreach (var item in entity.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new EntityNotFoundException("Produto", item.ProductId);

            if (!product.IsActive)
                throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");

            item.Product = product;
            item.UnitPrice = product.Price;
            products.Add(product);
        }

        ValidateDuplicateCategories(products);
        ValidateProductQuantity(entity.Items);
        RecalculateTotals(entity);

        return await base.CreateAsync(entity);
    }

    public override async Task<Order> UpdateAsync(Order entity)
    {
        var existing = await _orderOrderItemRepository.GetByIdAsync(entity.Id);
        if (existing == null)
            throw new EntityNotFoundException("Pedido", entity.Id);

        var products = new List<Product>();

        foreach (var item in entity.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new EntityNotFoundException("Produto", item.ProductId);

            if (!product.IsActive)
                throw new BusinessRuleViolationException($"Produto '{product.Name}' não está ativo.");

            item.Product = product;
            item.UnitPrice = product.Price;
            item.OrderId = entity.Id;
            products.Add(product);
        }

        ValidateDuplicateCategories(products);
        ValidateProductQuantity(entity.Items);
        RecalculateTotals(entity);

        await base.UpdateAsync(entity);
        return entity;
    }

    public override async Task DeleteAsync(int id)
    {
        var entity = await _orderOrderItemRepository.GetByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException("Pedido", id);

        await base.DeleteAsync(id);
    }

    public void ValidateDuplicateCategories(IEnumerable<Product> products)
    {
        var seenCategories = new HashSet<int>();
        foreach (var product in products)
        {
            if (!seenCategories.Add(product.CategoryId))
                throw new DuplicateItemException(product.Category?.Name ?? "Desconhecida");
        }
    }

    public void ValidateProductQuantity(IEnumerable<OrderItem> orderItems)
    {
        if (orderItems.Any(i => i.Quantity != 1))
            throw new InvalidItemQuantityException();
    }

    public void RecalculateTotals(Order order)
    {
        order.Subtotal = _discountCalculatorService.CalculateSubtotal(order);
        var (discount, _) = _discountCalculatorService.Calculate(order);
        order.Discount = discount;
        order.Total = order.Subtotal - discount;
    }
}
