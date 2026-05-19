using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Omnimarket.Api.Utils
{
    public class Criptografia
    {
        public static void CriarPasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerificarPasswordHash(string password, byte[] hash, byte[] salt)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (hash == null || salt == null)
                return false;

            if (hash.Length != 64)
                throw new ArgumentException("Hash inválido (esperado 64 bytes)");

            if (salt.Length != 128)
                throw new ArgumentException("Salt inválido (esperado 128 bytes)");

            using var hmac = new System.Security.Cryptography.HMACSHA512(salt);

            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }
        /*
             // Ajuste conforme performance do seu servidor; 100k é um exemplo comum
            private const int Iterations = 100_000;
            private const int SaltSize = 16; // 128 bits
            private const int KeySize = 32;  // 256 bits

            public static void HashPassword(string password, out byte[] hash, out byte[] salt)
            {
                salt = RandomNumberGenerator.GetBytes(SaltSize);

                hash = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: KeySize);
            }

            public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
            {
                var computed = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: storedSalt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: Iterations,
                    numBytesRequested: KeySize);

                // comparação em tempo constante
                return CryptographicOperations.FixedTimeEquals(computed, storedHash);
    }

        */




    }

}