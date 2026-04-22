using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.IoC;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy("AllowBlazor", policy =>
    policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()));

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

app.UseCors("AllowBlazor");
app.UseHttpsRedirection();

app
    .MapOrderItemEndpoints()
    .MapOrderEndpoints()
    .MapProductCategoryEndpoints()
    .MapProductEndpoints();

app.Run();