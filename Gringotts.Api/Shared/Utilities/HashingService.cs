namespace Gringotts.Api.Shared.Utilities;

using BCrypt = BCrypt.Net.BCrypt;

/// <summary>
/// Provides hashing and verification services using the BCrypt hashing algorithm.
/// </summary>
public class HashingService
{
    /// <summary>
    /// Hashes the provided input using the BCrypt enhanced hash function.
    /// </summary>
    /// <param name="input">The string input to be hashed.</param>
    /// <returns>A hashed string generated from the input.</returns>
    public string Hash(string input)
    {
        return BCrypt.EnhancedHashPassword(input);
    }

    /// <summary>
    /// Verifies whether the provided input matches the stored hash.
    /// </summary>
    /// <param name="input">The plain text input to verify against the stored hash.</param>
    /// <param name="hash">The hashed value to verify the input against.</param>
    /// <returns>True if the input matches the stored hash; otherwise, false.</returns>
    public bool Verify(string input, string hash)
    {
        return BCrypt.EnhancedVerify(input, hash);
    }
}