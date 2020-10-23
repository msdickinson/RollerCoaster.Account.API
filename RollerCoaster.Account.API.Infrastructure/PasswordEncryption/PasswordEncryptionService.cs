using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Models;
using System;
using System.Security.Cryptography;

namespace RollerCoaster.Account.API.Infrastructure.PasswordEncryption
{
    public class PasswordEncryptionService : IPasswordEncryptionService
    {
        public EncryptResult Encrypt(string password, string salt = null)
        {
            byte[] saltByteArray;
            if (string.IsNullOrWhiteSpace(salt))
            {
                saltByteArray = new byte[16];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(saltByteArray);
            }
            else
            {
                saltByteArray = Convert.FromBase64String(salt);
            }
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltByteArray,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            return new EncryptResult
            {
                Hash = hash,
                Salt = Convert.ToBase64String(saltByteArray)
            };
        }
    }
}
