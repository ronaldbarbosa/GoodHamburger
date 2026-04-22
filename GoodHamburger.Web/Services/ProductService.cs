using System.Net.Http.Json;

namespace GoodHamburger.Web.Services;

public class ProductService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string ProductsPath = "/api/products";

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        var response = await _client.GetFromJsonAsync<List<ProductResponse>>(ProductsPath);
        return response ?? [];
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _client.GetFromJsonAsync<ProductResponse>($"{ProductsPath}/{id}");
        }
        catch
        {
            return null;
        }
    }
}

public class ProductCategoryService(HttpClient client)
{
    private readonly HttpClient _client = client;
    private const string CategoriesPath = "/api/product-categories";

    public async Task<List<ProductCategoryResponse>> GetAllAsync()
    {
        var response = await _client.GetFromJsonAsync<List<ProductCategoryResponse>>(CategoriesPath);
        return response ?? [];
    }

    public async Task<ProductCategoryResponse?> GetByIdAsync(int id)
    {
        try
        {
            return await _client.GetFromJsonAsync<ProductCategoryResponse>($"{CategoriesPath}/{id}");
        }
        catch
        {
            return null;
        }
    }
}