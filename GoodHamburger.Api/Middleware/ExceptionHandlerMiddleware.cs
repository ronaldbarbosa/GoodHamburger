using System.Text.Json;
using GoodHamburger.Api.Models.Responses;
using Microsoft.AspNetCore.Http;

namespace GoodHamburger.Api.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        
var response = ex switch
        {
            System.Text.Json.JsonException => new ErrorResponse("Formato inválido. Verifique o JSON enviado e tente novamente."),
            ArgumentException => new ErrorResponse($"Dados inválidos: {ex.Message}"),
            FormatException => new ErrorResponse($"Formato inválido: {ex.Message}"),
            BadHttpRequestException => new ErrorResponse("Dados inválidos na requisição. Verifique o formato e tente novamente."),
            _ => new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes.")
        };
        
        context.Response.StatusCode = ex is System.Text.Json.JsonException or ArgumentException or FormatException or BadHttpRequestException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}