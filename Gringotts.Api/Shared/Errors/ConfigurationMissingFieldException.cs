namespace Gringotts.Api.Shared.Errors;

public class ConfigurationMissingFieldException(string key)
    : Exception($"Configuration key '{key}' is missing or null.");
