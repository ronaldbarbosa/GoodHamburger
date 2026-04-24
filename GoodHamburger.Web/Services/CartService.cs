using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class CartService
{
    private OrderResponse? _currentCart;
    private readonly List<CartItem> _items = [];
    private bool _initialized;
    public event Action? OnCartChanged;
    public event Action? OnInitialized;
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();
    public int ItemCount => _items.Sum(i => i.Quantity);
    public OrderResponse? CurrentCart => _currentCart;
    public bool IsInitialized => _initialized;

    public async Task InitializeAsync(HttpClient client)
    {
        var orders = await client.GetFromJsonAsync<List<OrderResponse>>("/api/orders");
        var pendingOrder = orders?.FirstOrDefault(o => o.Status == "Pending");

        if (pendingOrder is not null)
        {
            _currentCart = pendingOrder;
            _items.Clear();
            foreach (var item in pendingOrder.Items)
            {
                _items.Add(new CartItem(item.Id, item.Product.Id, item.Product.Name, item.Product.Category.Name, item.Quantity, item.UnitPrice));
            }
        }
        _initialized = true;
        OnCartChanged?.Invoke();
        OnInitialized?.Invoke();
    }

    public async Task<bool> AddItemAsync(HttpClient client, int productId, string productName, string categoryName, int quantity, string unitPrice)
    {
        if (_items.Any(i => i.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
            return false;

        if (_currentCart is null)
        {
            var newOrder = new CreateOrderRequest([new CreateOrderItemRequest(productId, quantity, null)]);
            var response = await client.PostAsJsonAsync("/api/orders", newOrder);
            _currentCart = await response.Content.ReadFromJsonAsync<OrderResponse>();

            if (_currentCart is not null)
            {
                _items.Clear();
                _items.Add(new CartItem(0, productId, productName, categoryName, quantity, unitPrice));
            }
        }
        else
        {
            var newItem = new CreateOrderItemRequest(productId, quantity, _currentCart.Id);
            var response = await client.PostAsJsonAsync("/api/order-items", newItem);
            var createdItem = await response.Content.ReadFromJsonAsync<OrderItemResponse>();

            if (createdItem is not null)
            {
                _items.Add(new CartItem(createdItem.Id, productId, productName, categoryName, quantity, unitPrice));
            }

            _currentCart = await client.GetFromJsonAsync<OrderResponse>($"/api/orders/{_currentCart.Id}");
        }

        OnCartChanged?.Invoke();
        return true;
    }

    public async Task RemoveItemAsync(HttpClient client, int itemId)
    {
        await client.DeleteAsync($"/api/order-items/{itemId}");

        _items.RemoveAll(i => i.Id == itemId);

        if (_currentCart is not null)
        {
            _currentCart = await client.GetFromJsonAsync<OrderResponse>($"/api/orders/{_currentCart.Id}");
        }
        OnCartChanged?.Invoke();
    }

    public async Task ConfirmCartAsync(HttpClient client)
    {
        if (_currentCart is null) return;

        await client.PatchAsync($"/api/orders/{_currentCart.Id}/confirm", null);

        _currentCart = null;
        _items.Clear();
        OnCartChanged?.Invoke();
    }

    public void ClearLocal()
    {
        _currentCart = null;
        _items.Clear();
        OnCartChanged?.Invoke();
    }
}
