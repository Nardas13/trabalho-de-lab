using System.Security.Cryptography;
namespace AutoHubProjeto.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16;   // 128 bits
        private const int KeySize = 32;   // 256 bits
        private const int Iterations = 100_000;

        public static byte[] HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            var result = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(key, 0, result, SaltSize, KeySize);
            return result;
        }

        public static bool VerifyPassword(string password, byte[] stored)
        {
            if (stored == null || stored.Length != SaltSize + KeySize)
                return false;

            var salt = new byte[SaltSize];
            var key = new byte[KeySize];

            Buffer.BlockCopy(stored, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(stored, SaltSize, key, 0, KeySize);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var keyToCheck = pbkdf2.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(key, keyToCheck);
        }
    }
}