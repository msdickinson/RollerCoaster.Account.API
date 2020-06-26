using DickinsonBros.Guid.Abstractions;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using RollerCoaster.Account.API.Logic.Models;
using System.Threading.Tasks;
namespace RollerCoaster.Account.API.Logic
{
    public class AccountManager : IAccountManager
    {
        internal readonly IGuidService _guidService;
        internal readonly IAccountDBService _accountDBService;
        internal readonly IPasswordEncryptionService _passwordEncryptionService;
        internal readonly IAccountEmailService _accountEmailService;
        public AccountManager
        (
            IGuidService guidService,
            IAccountDBService accountDBService,
            IPasswordEncryptionService passwordEncryptionService,
            IAccountEmailService accountEmailService
        )
        {
            _guidService = guidService;
            _accountDBService = accountDBService;
            _passwordEncryptionService = passwordEncryptionService;
            _accountEmailService = accountEmailService;
        }
        public async Task<CreateAccountDescriptor> CreateAsync(string username, string password, string email)
        {
            var createAccountDescriptor = new CreateAccountDescriptor();

            var encryptResult = _passwordEncryptionService.Encrypt(password);

            var activateEmailToken = _guidService.NewGuid().ToString();
            var emailPreferenceToken = _guidService.NewGuid().ToString();

            var insertAccountResult = await
                _accountDBService.InsertAccountAsync
                (
                    new InsertAccountRequest
                    {
                        Username = username,
                        PasswordHash = encryptResult.Hash,
                        Salt = encryptResult.Salt,
                        ActivateEmailToken = activateEmailToken,
                        EmailPreferenceToken = emailPreferenceToken,
                        Email = email,
                        EmailPreference = EmailPreference.Any
                    }
               ).ConfigureAwait(false);

            if (insertAccountResult.DuplicateUser)
            {
                createAccountDescriptor.Result = CreateAccountResult.DuplicateUser;
                return createAccountDescriptor;
            }

            if (email != null)
            {
                await _accountEmailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken);
            }

            createAccountDescriptor.Result = CreateAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }
    }
}
