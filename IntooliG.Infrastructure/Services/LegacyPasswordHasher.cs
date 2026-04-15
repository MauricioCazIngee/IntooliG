using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IntooliG.Infrastructure.Services;

public static class LegacyPasswordHasher
{
    private const string HexChars = "0123456789abcdefABCDEF";

    public static string HashPassword(string password)
    {
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA1);
        var hash = pbkdf2.GetBytes(32);
        var hashBytes = new byte[1 + 16 + 32];
        hashBytes[0] = 0x00;
        Buffer.BlockCopy(salt, 0, hashBytes, 1, 16);
        Buffer.BlockCopy(hash, 0, hashBytes, 1 + 16, 32);
        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            return false;

        if (storedHash.Length == 32 && storedHash.All(c => HexChars.Contains(c)))
            return VerifyMD5(password, storedHash);

        if (storedHash.Length == 44 && storedHash.EndsWith('='))
            return VerifySHA256Base64(password, storedHash);

        try
        {
            var hashBytes = Convert.FromBase64String(storedHash);
            if (hashBytes.Length == 49 && hashBytes[0] == 0x00)
                return VerifyLegacyPBKDF2(password, hashBytes);
        }
        catch
        {
            // ignorar Base64 inválido
        }

        return false;
    }

    public static bool IsLegacyStoredHash(string? storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
            return false;

        if (storedHash.Length == 32 && storedHash.All(c => HexChars.Contains(c)))
            return true;

        if (storedHash.Length == 44 && storedHash.EndsWith('='))
            return true;

        try
        {
            var hashBytes = Convert.FromBase64String(storedHash);
            return hashBytes.Length == 49 && hashBytes[0] == 0x00;
        }
        catch
        {
            return false;
        }
    }

    private static bool VerifyMD5(string password, string storedHash)
    {
        using var md5 = MD5.Create();
        var inputBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = md5.ComputeHash(inputBytes);
        var computedHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return computedHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
    }

    private static bool VerifySHA256Base64(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(inputBytes);
        var computed = Convert.ToBase64String(hashBytes);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(storedHash));
    }

    private static bool VerifyLegacyPBKDF2(string password, byte[] storedHashBytes)
    {
        var salt = new byte[16];
        Buffer.BlockCopy(storedHashBytes, 1, salt, 0, 16);

        var storedHashValue = new byte[32];
        Buffer.BlockCopy(storedHashBytes, 1 + 16, storedHashValue, 0, 32);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000, HashAlgorithmName.SHA1);
        var computedHash = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashValue);
    }
}
