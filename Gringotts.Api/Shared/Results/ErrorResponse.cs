namespace Gringotts.Api.Shared.Results;

/// <summary>
/// Represents an error with a specific type, code, and description.
/// </summary>
public record ErrorResponse(string Code, string Description, ErrorResponse.ErrorType Type)
{
    /// <summary>
    /// Enumerates possible types of errors.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>Indicates a general failure error.</summary>
        Failure = 0,

        /// <summary>Indicates that the requested resource was not found.</summary>
        NotFound = 1,

        /// <summary>Indicates that a validation error occurred.</summary>
        Validation = 2,

        /// <summary>Indicates a conflict error, such as a version conflict.</summary>
        Conflict = 3,

        /// <summary>Indicates an unauthorized access attempt.</summary>
        AccessUnauthorized = 4,

        /// <summary>Indicates a forbidden access attempt.</summary>
        AccessForbidden = 5
    }

    /// <summary>
    /// Creates a failure error with a specific code and description.
    /// </summary>
    public static ErrorResponse Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a "not found" error with a specific code and description.
    /// </summary>
    public static ErrorResponse NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Creates a validation error with a specific code and description.
    /// </summary>
    public static ErrorResponse Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>
    /// Creates a conflict error with a specific code and description.
    /// </summary>
    public static ErrorResponse Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized access error with a specific code and description.
    /// </summary>
    public static ErrorResponse AccessUnAuthorized(string code, string description) =>
        new(code, description, ErrorType.AccessUnauthorized);

    /// <summary>
    /// Creates a forbidden access error with a specific code and description.
    /// </summary>
    public static ErrorResponse AccessForbidden(string code, string description) =>
        new(code, description, ErrorType.AccessForbidden);
}