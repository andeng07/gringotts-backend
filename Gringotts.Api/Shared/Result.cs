namespace Gringotts.Api.Shared;

/// <summary>
/// A read-only struct representing the result of an operation, which can be a success containing a value
/// of type <typeparamref name="TValue"/> or a failure containing an error of type.<typeparamref name="TError"/>
/// </summary>
/// <typeparam name="TValue">The type of the value when the operation succeeds.</typeparam>
/// <typeparam name="TError">The type of the value when the operation fails.</typeparam>
public readonly struct Result<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    /// <summary>
    /// Gets a value indicating whether the result represents a successful operation.
    /// If <c>true</c>, the result contains a value of type <typeparamref name="TValue"/>; 
    /// if <c>false</c>, it contains an error of type <typeparamref name="TError"/>.
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Initializes a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value representing the success result.</param>
    private Result(TValue value)
    {
        IsSuccess = true;
        _value = value;
        _error = default;
    }

    /// <summary>
    /// Initializes a failure result containing the specified error.
    /// </summary>
    /// <param name="error">The error representing the failure result.</param>
    private Result(TError error)
    {
        IsSuccess = false;
        _error = error;
        _value = default;
    }
    
    /// <summary>
    /// Matches the result to a function depending on whether the result is a success or a failure.
    /// If the result is successful, the <paramref name="success"/> function is invoked with the success value;
    /// otherwise, the <paramref name="error"/> function is invoked with the error value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result produced by the match functions.</typeparam>
    /// <param name="success">The function to invoke if the result is successful.</param>
    /// <param name="error">The function to invoke if the result is a failure.</param>
    /// <returns>The result of invoking either the <paramref name="success"/> or <paramref name="error"/> function.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> error) =>
        IsSuccess ? success(_value!) : error(_error!);
    
    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TValue"/> to a successful <see cref="Result{TValue, TError}"/>.
    /// </summary>
    /// <param name="value">The value to be wrapped in a successful result.</param>
    public static implicit operator Result<TValue, TError>(TValue value) => new(value);
    
    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="TError"/> to a failed <see cref="Result{TValue, TError}"/>.
    /// </summary>
    /// <param name="error">The error to be wrapped in a failure result.</param>
    public static implicit operator Result<TValue, TError>(TError error) => new(error);
}