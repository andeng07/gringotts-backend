namespace Gringotts.Api.Shared.Errors;

public static class AuthErrorCodes
{
    public const string CredentialsInvalid = "Authentication.Credentials.Invalid";  
    
    public const string EmailAlreadyRegistered = "Authentication.Register.EmailAlreadyRegistered";
    
    public const string TokenNotFound = "Authentication.Token.NotFound";
    public const string InvalidTokenFormat = "Authentication.Token.InvalidFormat";
    
    public const string AccessDenied = "Authentication.AccessDenied";
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

public static class ReaderErrorCodes
{
    public const string NameRequired = "Reader.Name.Required";
    public const string NameTooLong = "Reader.Name.TooLong";
    public const string LocationNotFound = "Reader.Location.NotFound";
    public const string ReaderCreationFailed = "Reader.Creation.Failed";
}

public static class LocationErrorCodes
{
    public const string BuildingNameRequired = "Location.BuildingName.Required";
    public const string BuildingNameEmpty = "Location.BuildingName.Empty";
    public const string BuildingNameTooLong = "Location.BuildingName.TooLong";
    
    public const string RoomNameEmpty = "Location.RoomName.Empty";
    public const string RoomNameTooLong = "Location.RoomName.TooLong";

    public const string LocationAlreadyExists = "Location.AlreadyExists";
    public const string LocationNotFound = "Location.NotFound";
}


public static class UserErrorCodes
{
    public const string UserNotFound = "User.NotFound";
}