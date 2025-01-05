namespace Gringotts.Api.Shared.Results;

/// <summary>
/// Represents the outcome of an operation, with a success or multiple failure errors.
/// </summary>
public record OperationResult
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public readonly bool IsSuccess;

    /// <summary>
    /// Contains the errors if the operation failed; otherwise, an empty list.
    /// </summary>
    public readonly List<ErrorResponse> Errors;

    /// <summary>
    /// Initializes a successful <see cref="OperationResult"/> instance.
    /// </summary>
    protected OperationResult()
    {
        IsSuccess = true;
        Errors = [];
    }

    /// <summary>
    /// Initializes a failed <see cref="OperationResult"/> instance with specific errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    protected OperationResult(IEnumerable<ErrorResponse> errors)
    {
        IsSuccess = false;
        Errors = [..errors];
    }

    /// <summary>
    /// Creates a successful operationResult.
    /// </summary>
    public static OperationResult Success() => new();

    /// <summary>
    /// Creates a failed operationResult with specified errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    public static OperationResult Failure(params ErrorResponse[] errors) => new(errors);

    /// <summary>
    /// Implicitly converts an <see cref="ErrorResponse"/> to a failed <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="errorResponse">The errorResponse to convert.</param>
    public static implicit operator OperationResult(ErrorResponse errorResponse) => new([errorResponse]);
    
    public static implicit operator OperationResult(List<ErrorResponse> errors) => new(errors.ToArray());
}

/// <summary>
/// Represents the outcome of an operation that produces a value, with a success or multiple failure errors.
/// </summary>
/// <typeparam name="TValue">The type of the operationResult value.</typeparam>
public sealed record TypedOperationResult<TValue> : OperationResult
{
    /// <summary>
    /// Gets the value if the operation was successful; otherwise, null.
    /// </summary>
    public readonly TValue? Value;

    /// <summary>
    /// Initializes a successful <see cref="TypedOperationResult{TValue}"/> instance with a specified value.
    /// </summary>
    /// <param name="value">The operationResult value.</param>
    private TypedOperationResult(TValue value) =>
        Value = value;

    /// <summary>
    /// Initializes a failed <see cref="TypedOperationResult{TValue}"/> instance with specific errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    private TypedOperationResult(IEnumerable<ErrorResponse> errors) : base(errors) =>
        Value = default;

    /// <summary>
    /// Implicitly converts an <see cref="ErrorResponse"/> to a failed <see cref="TypedOperationResult{TValue}"/>.
    /// </summary>
    /// <param name="errorResponse">The errorResponse to convert.</param>
    public static implicit operator TypedOperationResult<TValue>(ErrorResponse errorResponse) => new([errorResponse]);
    
    public static implicit operator TypedOperationResult<TValue>(List<ErrorResponse> errors) => new(errors.ToArray());
    
    /// <summary>
    /// Implicitly converts a value to a successful <see cref="TypedOperationResult{TValue}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator TypedOperationResult<TValue>(TValue value) => new(value);

    /// <summary>
    /// Creates a successful operationResult with a specified value.
    /// </summary>
    /// <param name="value">The operationResult value.</param>
    public static TypedOperationResult<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed operationResult with specified errors.
    /// </summary>
    /// <param name="errors">The list of errors describing the failures.</param>
    public new static TypedOperationResult<TValue> Failure(params ErrorResponse[] errors) => new(errors);
}