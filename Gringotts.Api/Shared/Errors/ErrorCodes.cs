namespace Gringotts.Api.Shared.Errors;

public static class AuthErrorCodes
{
    public const string CredentialsInvalid = "Auth.Credentials.Invalid";  
}

public static class ValidationErrorCodes
{
    public const string EmailRequired = "Validation.Email.Required";
    public const string InvalidEmailFormat = "Validation.Email.InvalidFormat"; 
    public const string PasswordTooShort = "Validation.Password.TooShort";
    public const string PasswordRequired = "Validation.Password.Required";
}