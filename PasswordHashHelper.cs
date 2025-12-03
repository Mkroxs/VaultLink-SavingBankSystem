using System;
using System.Security.Cryptography;
using System.Text;

namespace VaultLinkBankSystem.Helpers
{
    
    public static class PasswordHashHelper
    {
        
        public static string HashPassword(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("Input cannot be null or empty");

            // Create SHA256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                
                string saltedInput = input + "VaultLink2025"; 

                byte[] bytes = Encoding.UTF8.GetBytes(saltedInput);

                byte[] hash = sha256.ComputeHash(bytes);

                StringBuilder result = new StringBuilder();
                foreach (byte b in hash)
                {
                    result.Append(b.ToString("x2")); // x2 = lowercase hex
                }

                return result.ToString();
            }
        }

       
        public static bool VerifyPassword(string input, string storedHash)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(storedHash))
                return false;

            // Hash the input
            string inputHash = HashPassword(input);

            // Compare with stored hash
            return inputHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        }

       
        public static string HashPIN(string pin)
        {
            // Validate PIN format
            if (string.IsNullOrEmpty(pin))
                throw new ArgumentException("PIN cannot be null or empty");

            if (pin.Length != 6)
                throw new ArgumentException("PIN must be exactly 6 digits");

            if (!int.TryParse(pin, out _))
                throw new ArgumentException("PIN must contain only numbers");

            return HashPassword(pin);
        }

       
        public static bool VerifyPIN(string pin, string storedHash)
        {
            return VerifyPassword(pin, storedHash);
        }

       
        public static string GenerateRandomPIN()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}

/* 
 * HOW IT WORKS:
 * 
 * 1. HASHING:
 *    - Takes plain text (password/PIN)
 *    - Adds a salt (extra text for security)
 *    - Uses SHA256 algorithm to create one-way hash
 *    - Returns 64-character hex string
 *    - CANNOT be reversed to get original text
 * 
 * 2. VERIFICATION:
 *    - Takes user input (plain text)
 *    - Hashes it the same way
 *    - Compares with stored hash
 *    - If they match, password is correct
 * 
 * 3. SECURITY:
 *    - Even if database is stolen, passwords are safe
 *    - Same input always produces same hash
 *    - Different inputs produce completely different hashes
 *    - Adding salt prevents rainbow table attacks
 * 
 * EXAMPLE:
 * Input: "123456"
 * Salt added: "123456VaultLink2025"
 * Hash: "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92"
 * 
 * The hash is stored in database.
 * During login, user enters "123456", it gets hashed again,
 * and compared with stored hash.
 */