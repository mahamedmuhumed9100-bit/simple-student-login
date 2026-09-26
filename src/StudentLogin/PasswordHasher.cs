using System.Security.Cryptography;

namespace StudentLogin;

/// <summary>
/// Salted, slow password hashing with PBKDF2.
///
/// A plain SHA-256 hash is fast, and the same password always gives the same
/// hash, so leaked hashes can be cracked with precomputed tables or a GPU.
/// PBKDF2 fixes both: a random salt per password makes every hash unique, and
/// 100,000 iterations make each guess expensive.
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;   // bytes
    private const int HashSize = 32;   // bytes
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    /// <summary>Returns "iterations.salt.hash" (salt and hash in Base64).</summary>
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string storedHash)
    {
        // The iteration count is stored with the hash, so it can be raised in
        // future without breaking passwords hashed with the old value.
        string[] parts = storedHash.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations) || iterations <= 0)
            return false;

        byte[] salt, expected;
        try
        {
            salt = Convert.FromBase64String(parts[1]);
            expected = Convert.FromBase64String(parts[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expected.Length);

        // Constant-time comparison: a normal == can return as soon as one byte
        // differs, and that timing difference can leak information.
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
