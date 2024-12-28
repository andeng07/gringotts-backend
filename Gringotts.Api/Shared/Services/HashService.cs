using System.Security.Cryptography;
using System.Text;

namespace Gringotts.Api.Shared.Services;

using BCrypt = BCrypt.Net.BCrypt;

public class HashService
{
    
    public string Hash(string input)
    {
        return BCrypt.EnhancedHashPassword(input);
    }

    public bool Verify(string input, string hash)
    {
        return BCrypt.EnhancedVerify(input, hash);
    }
}