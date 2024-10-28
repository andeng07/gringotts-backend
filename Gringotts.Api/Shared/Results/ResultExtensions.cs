namespace Gringotts.Api.Shared.Results;

/// <summary>
/// Provides extension methods for working with <see cref="Result"/> and <see cref="TypedResult{TValue}"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Matches the outcome of a <see cref="Result"/> to either a success or failure action,
    /// allowing multiple errors.
    /// </summary>
    /// <typeparam name="T">The return type of the match result.</typeparam>
    /// <param name="result">The <see cref="Result"/> to match against.</param>
    /// <param name="onSuccess">The function to execute if the result is successful.</param>
    /// <param name="onFailure">The function to execute if the result is a failure, receiving the list of errors.</param>
    /// <returns>The result of either the success or failure function.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is <c>null</c>.
    /// </exception>
    public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<List<Error>, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Errors);
    }

    /// <summary>
    /// Matches the outcome of a <see cref="TypedResult{TValue}"/> to either a success or failure action,
    /// allowing multiple errors.
    /// </summary>
    /// <typeparam name="T">The return type of the match result.</typeparam>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="typedResult">The <see cref="TypedResult{TValue}"/> to match against.</param>
    /// <param name="onSuccess">The function to execute if the result is successful, receiving the success value.</param>
    /// <param name="onFailure">The function to execute if the result is a failure, receiving the list of errors.</param>
    /// <returns>The result of either the success or failure function.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is <c>null</c>.
    /// </exception>
    public static T Match<T, TValue>(
        this TypedResult<TValue> typedResult,
        Func<TValue, T> onSuccess,
        Func<List<Error>, T> onFailure)
    {
        return typedResult.IsSuccess ? onSuccess(typedResult.Value!) : onFailure(typedResult.Errors);
    }
}