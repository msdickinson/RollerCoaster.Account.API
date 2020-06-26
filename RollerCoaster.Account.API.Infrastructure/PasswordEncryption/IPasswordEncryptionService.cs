using RollerCoaster.Account.API.Infrastructure.PasswordEncryption.Models;

namespace RollerCoaster.Account.API.Infrastructure.PasswordEncryption
{
    public interface IPasswordEncryptionService
    {
        EncryptResult Encrypt(string password, string salt = null);
    }
}