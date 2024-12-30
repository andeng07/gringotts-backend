namespace Gringotts.Api.Shared.Errors;

public static class AuthErrorCodes
{
    public const string CredentialsInvalid = "Auth.Credentials.Invalid";  
    
    public const string EmailAlreadyRegistered = "Authentication.Register.EmailAlreadyRegistered";
    
    public const string TokenNotFound = "Auth.Token.NotFound";
    public const string InvalidTokenFormat = "Auth.Token.InvalidFormat";
    
    public const string AccessDenied = "Auth.AccessDenied";
}

public static class ValidationErrorCodes
{
    public const string EmailRequired = "Validation.Email.Required";
    public const string InvalidEmailFormat = "Validation.Email.InvalidFormat"; 
    
    public const string PasswordTooShort = "Validation.Password.TooShort";
    public const string PasswordRequired = "Validation.Password.Required";
    
    public const string CardIdRequired = "Validation.CardId.Required";
    
    public const string SchoolIdRequired = "Validation.SchoolId.Required";
    
    public const string FirstNameRequired = "Validation.FirstName.Required";
    public const string LastNameRequired = "Validation.LastName.Required";
    
    public const string InvalidTokenSubjectFormat = "Validation.Token.SubjectFormat";
}

public static class UserErrorCodes
{
    public const string UserNotFound = "User.NotFound";
}