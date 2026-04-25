using System.Text.Json;
using GoodHamburger.Api.Models.Responses;
using GoodHamburger.Core.Exceptions;

namespace GoodHamburger.Api.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            // requisição cancelada pelo cliente — não logar como erro
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        switch (ex)
        {
            case EntityNotFoundException e:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                var notFound = new ValidationResponse([new ValidationItemResponse(e.EntityType, e.Message)]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(notFound, options));
                return;

            case DuplicateItemException or BusinessRuleViolationException or InvalidItemQuantityException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var field = ex is InvalidItemQuantityException iqe ? iqe.EntityType : string.Empty;
                var business = new ValidationResponse([new ValidationItemResponse(field, ex.Message)]);
                await context.Response.WriteAsync(JsonSerializer.Serialize(business, options));
                return;

            case JsonException or FormatException or BadHttpRequestException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var badRequest = new ErrorResponse("Formato inválido. Verifique os dados enviados e tente novamente.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(badRequest, options));
                return;

            default:
                logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var error = new ErrorResponse("Erro ao processar solicitação. Tente novamente em alguns instantes.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(error, options));
                return;
        }
    }
}

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlerMiddleware>();
}
