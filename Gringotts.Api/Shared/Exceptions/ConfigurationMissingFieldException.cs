namespace Gringotts.Api.Shared.Exceptions;

public class ConfigurationMissingFieldException(string key)
    : Exception($"Configuration key '{key}' is missing or null.");
