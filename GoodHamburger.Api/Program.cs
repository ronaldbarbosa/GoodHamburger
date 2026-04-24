using GoodHamburger.Api.Endpoints;
using GoodHamburger.Api.IoC;
using GoodHamburger.Api.Middleware;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options => options.AddPolicy("AllowBlazor", policy =>
{
    var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
    policy.WithOrigins(origins)
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

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
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();

app
    .MapOrderItemEndpoints()
    .MapOrderEndpoints()
    .MapProductCategoryEndpoints()
    .MapProductEndpoints();

app.Run();