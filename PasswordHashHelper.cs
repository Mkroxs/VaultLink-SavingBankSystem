using System;
using System.Security.Cryptography;

namespace VaultLinkBankSystem.Helpers
{
    public static class PasswordHashHelper
    {
        // 16-byte salt + 32-byte SHA-256 hash = 48 bytes → 64 Base64 characters
        private const int SaltSize = 16;
        private const int HashSize = 32;        // SHA-256
        private const int Iterations = 310000;    // Banking-standard in 2025

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            byte[] result = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, result, 0, SaltSize);
            Array.Copy(hash, 0, result, SaltSize, HashSize);

            return Convert.ToBase64String(result); // Always 64 characters
        }

        public static bool VerifyPassword(string password, string storedHashBase64)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHashBase64))
                return false;

            byte[] buffer;
            try { buffer = Convert.FromBase64String(storedHashBase64); }
            catch { return false; }

            if (buffer.Length != SaltSize + HashSize) return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(buffer, 0, salt, 0, SaltSize);

            byte[] storedHash = new byte[HashSize];
            Array.Copy(buffer, SaltSize, storedHash, 0, HashSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] testHash = pbkdf2.GetBytes(HashSize);

            return SlowEquals(storedHash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }

        // Reuse for PINs (6-digit PINs are just short passwords)
        public static string HashPIN(string pin) => HashPassword(pin);
        public static bool VerifyPIN(string pin, string storedHash) => VerifyPassword(pin, storedHash);
    }
}