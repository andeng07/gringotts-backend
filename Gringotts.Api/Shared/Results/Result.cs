namespace Gringotts.Api.Shared.Results;

/// <summary>
/// Represents the outcome of an operation, with a success or multiple failure errors.
/// </summary>
public record Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public readonly bool IsSuccess;

    /// <summary>
    /// Contains the errors if the operation failed; otherwise, an empty list.
    /// </summary>
    public readonly List<Error> Errors;

    /// <summary>
    /// Initializes a successful <see cref="Result"/> instance.
    /// </summary>
    protected Result()
    {
        IsSuccess = true;
        Errors = [];
    }

    /// <summary>
    /// Initializes a failed <see cref="Result"/> instance with specific errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    protected Result(IEnumerable<Error> errors)
    {
        IsSuccess = false;
        Errors = [..errors];
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new();

    /// <summary>
    /// Creates a failed result with specified errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    public static Result Failure(params Error[] errors) => new(errors);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result(Error error) => new(new List<Error> { error });
}

/// <summary>
/// Represents the outcome of an operation that produces a value, with a success or multiple failure errors.
/// </summary>
/// <typeparam name="TValue">The type of the result value.</typeparam>
public sealed record TypedResult<TValue> : Result
{
    /// <summary>
    /// Gets the value if the operation was successful; otherwise, null.
    /// </summary>
    public readonly TValue? Value;

    /// <summary>
    /// Initializes a successful <see cref="TypedResult{TValue}"/> instance with a specified value.
    /// </summary>
    /// <param name="value">The result value.</param>
    private TypedResult(TValue value) =>
        Value = value;

    /// <summary>
    /// Initializes a failed <see cref="TypedResult{TValue}"/> instance with specific errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    private TypedResult(IEnumerable<Error> errors) : base(errors) =>
        Value = default;

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="TypedResult{TValue}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator TypedResult<TValue>(Error error) => new(new List<Error> { error });

    /// <summary>
    /// Implicitly converts a value to a successful <see cref="TypedResult{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator TypedResult<TValue>(TValue value) => new(value);

    /// <summary>
    /// Creates a successful result with a specified value.
    /// </summary>
    /// <param name="value">The result value.</param>
    public static TypedResult<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed result with specified errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    public new static TypedResult<TValue> Failure(params Error[] errors) => new(errors);
}