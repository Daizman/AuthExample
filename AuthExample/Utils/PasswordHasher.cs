using System.Security.Cryptography;
using System.Text;

namespace AuthExample.Utils;

public static class PasswordHasher
{
    private const int HashingIterations = 100000;
    private const int HashSize = 32;
    private const int SaltSize = 16;

    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA256;

    public static byte[] HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, HashingIterations, _hashAlgorithm, HashSize);

        return Encoding.UTF8.GetBytes($"{Convert.ToHexString(salt)}-{Convert.ToHexString(hash)}");
    }

    public static bool VerifyPassword(byte[] hashedPassword, string password)
    {
        var hashedPasswordString = Encoding.UTF8.GetString(hashedPassword);
        var parts = hashedPasswordString.Split('-');
        if (parts.Length != 2)
        {
            throw new FormatException("Invalid hashed password format.");
        }

        var salt = Convert.FromHexString(parts[0]);
        var hash = Convert.FromHexString(parts[1]);

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, HashingIterations, _hashAlgorithm, HashSize);

        if (hash.Length != computedHash.Length)
        {
            return false;
        }

        for (var i = 0; i < hash.Length; i++)
        {
            if (hash[i] != computedHash[i])
            {
                return false;
            }
        }

        return true;
    }
}
