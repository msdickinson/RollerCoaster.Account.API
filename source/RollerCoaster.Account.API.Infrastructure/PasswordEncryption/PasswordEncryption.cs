using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption.Models;
using System;
using System.Security.Cryptography;

namespace RollerCoaster.Account.API.Infrastructure.PasswordEncryption
{
    public class PasswordEncryption : IPasswordEncryption
    {
        public EncryptResult Encrypt(string password)
        {
            byte[] saltByteArray = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltByteArray);

            var hash = GenerateHash(password, saltByteArray);

            return new EncryptResult
            {
                Hash = hash,
                Salt = Convert.ToBase64String(saltByteArray)
            };
        }

        public EncryptResult EncryptWithSalt(string password, string salt)
        {
            var saltByteArray = Convert.FromBase64String(salt);

            var hash = GenerateHash(password, saltByteArray);

            return new EncryptResult
            {
                Hash = hash,
                Salt = Convert.ToBase64String(saltByteArray)
            };
        }

        private string GenerateHash(string password, byte[] saltByteArray)
        {
            return
            Convert.ToBase64String
            (
                KeyDerivation.Pbkdf2
                (
                    password: password,
                    salt: saltByteArray,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 32
                )
             );
        }
    }
}
