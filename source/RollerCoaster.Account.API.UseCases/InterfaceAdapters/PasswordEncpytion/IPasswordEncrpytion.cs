using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption.Models;

namespace RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption
{
    public interface IPasswordEncryption
    {
        EncryptResult Encrypt(string password);
        EncryptResult EncryptWithSalt(string password, string salt);
    }
}
