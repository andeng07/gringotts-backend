using FluentValidation;
using Gringotts.Api.Shared.Results;

namespace Gringotts.Api.Shared.Filters.Validation;

/// <summary>
/// Provides a filter for endpoint requests, validating incoming requests using the specified
/// <see cref="IValidator{TRequest}"/> implementation for type <typeparamref name="TRequest"/>.
/// </summary>
/// <param name="validator">An instance of <see cref="IValidator{TRequest}"/> used to validate incoming requests.</param>
/// <typeparam name="TRequest">The type of the request object that this filter validates.</typeparam>
public class RequestValidationFilter<TRequest>(IValidator<TRequest>? validator) : IEndpointFilter
{
    /// <summary>
    /// Invokes the filter to validate the request. If validation passes, the request is allowed to proceed.
    /// Otherwise, a failure operationResult is returned with validation errors.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{TResult}"/> that completes with the next delegate's operationResult if validation succeeds,
    /// or a failure operationResult containing validation errors if validation fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="validator"/> is <c>null</c>, indicating a validator must be provided.
    /// </exception>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(validator, $"validator for {nameof(TRequest)} cannot be null");

        var request = context.Arguments.OfType<TRequest>().First();

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(error =>
                new ErrorResponse(error.ErrorCode, error.ErrorMessage, ErrorResponse.ErrorType.Validation));

            return Microsoft.AspNetCore.Http.Results.BadRequest(errors);
        }

        return await next(context);
    }
}