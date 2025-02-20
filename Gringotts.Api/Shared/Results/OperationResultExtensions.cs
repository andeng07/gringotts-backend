namespace Gringotts.Api.Shared.Results;

/// <summary>
/// Provides extension methods for working with <see cref="OperationResult"/> and <see cref="TypedOperationResult{TValue}"/>.
/// </summary>
public static class OperationResultExtensions
{
    /// <summary>
    /// Matches the outcome of a <see cref="OperationResult"/> to either a success or failure action,
    /// allowing multiple errors.
    /// </summary>
    /// <typeparam name="T">The return type of the match operationResult.</typeparam>
    /// <param name="operationResult">The <see cref="OperationResult"/> to match against.</param>
    /// <param name="onSuccess">The function to execute if the operationResult is successful.</param>
    /// <param name="onFailure">The function to execute if the operationResult is a failure, receiving the list of errors.</param>
    /// <returns>The operationResult of either the success or failure function.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is <c>null</c>.
    /// </exception>
    public static T Match<T>(
        this OperationResult operationResult,
        Func<T> onSuccess,
        Func<List<ErrorResponse>, T> onFailure)
    {
        return operationResult.IsSuccess ? onSuccess() : onFailure(operationResult.Errors);
    }

    /// <summary>
    /// Matches the outcome of a <see cref="TypedOperationResult{TValue}"/> to either a success or failure action,
    /// allowing multiple errors.
    /// </summary>
    /// <typeparam name="T">The return type of the match operationResult.</typeparam>
    /// <typeparam name="TValue">The type of the operationResult value.</typeparam>
    /// <param name="typedOperationResult">The <see cref="TypedOperationResult{TValue}"/> to match against.</param>
    /// <param name="onSuccess">The function to execute if the operationResult is successful, receiving the success value.</param>
    /// <param name="onFailure">The function to execute if the operationResult is a failure, receiving the list of errors.</param>
    /// <returns>The operationResult of either the success or failure function.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is <c>null</c>.
    /// </exception>
    public static T Match<T, TValue>(
        this TypedOperationResult<TValue> typedOperationResult,
        Func<TValue, T> onSuccess,
        Func<List<ErrorResponse>, T> onFailure)
    {
        return typedOperationResult.IsSuccess ? onSuccess(typedOperationResult.Value!) : onFailure(typedOperationResult.Errors);
    }
}