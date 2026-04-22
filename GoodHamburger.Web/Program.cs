using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GoodHamburger.Web;
using GoodHamburger.Web.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<OrderItemService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProductCategoryService>();
builder.Services.AddScoped<CartService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5198") });

await builder.Build().RunAsync();