using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Identity.Domain.Users;

public interface IPasswordHasher
{
    string Hash(string password);
}

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required", nameof(password));
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100_000, 32);
        return $"pbkdf2-256|{Convert.ToBase64String(salt)}|{Convert.ToBase64String(hash)}";
    }
    
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('|');
        if (parts.Length != 3)
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var storedHash = Convert.FromBase64String(parts[2]);

        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100_000,
            numBytesRequested: 32
        );

        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }

}
