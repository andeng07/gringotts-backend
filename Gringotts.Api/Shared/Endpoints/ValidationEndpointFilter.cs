using FluentValidation;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Shared.Endpoints;

public class ValidationEndpointFilter<TRequest>(IValidator<TRequest>? validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(validator, $"validator for {nameof(TRequest)} cannot be null");

        var request = context.Arguments.OfType<TRequest>().First();

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            return Result.Failure(result.Errors.Select(e =>
                new Error(validator.GetType() + "." + e.ErrorCode, e.ErrorMessage, Error.ErrorType.Validation)));
        }
        
        return await next(context);
    }
}