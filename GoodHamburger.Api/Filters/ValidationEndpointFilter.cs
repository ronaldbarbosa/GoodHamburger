using FluentValidation;
using GoodHamburger.Api.Models.Responses;

namespace GoodHamburger.Api.Filters;

public class ValidationEndpointFilter<T>(IValidator<T> validator) : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return await next(context);

        var result = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            var items = result.Errors
                .Select(e => new ValidationItemResponse(e.PropertyName, e.ErrorMessage))
                .ToList();
            return Results.BadRequest(new ValidationResponse(items));
        }

        return await next(context);
    }
}
