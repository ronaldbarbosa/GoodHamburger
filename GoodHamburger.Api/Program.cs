using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.IoC;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api-docs", options =>
    {
        options.WithTitle("API Good Hamburger - Documentação");
    });
}

app.UseHttpsRedirection();

app
    .MapOrderItemEndpoints()
    .MapOrderEndpoints()
    .MapProductCategoryEndpoints()
    .MapProductEndpoints();

app.Run();