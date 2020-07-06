using DickinsonBros.Guid.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
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
        internal readonly AdminOptions _adminOptions;

        public AccountManager
        (
            IGuidService guidService,
            IAccountDBService accountDBService,
            IPasswordEncryptionService passwordEncryptionService,
            IAccountEmailService accountEmailService,
            IOptions<AdminOptions> adminOptions
        )
        {
            _guidService = guidService;
            _accountDBService = accountDBService;
            _passwordEncryptionService = passwordEncryptionService;
            _accountEmailService = accountEmailService;
            _adminOptions = adminOptions.Value;
        }
        public async Task<CreateUserAccountDescriptor> CreateUserAsync(string username, string password, string email)
        {
            var createAccountDescriptor = new CreateUserAccountDescriptor();

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
                        EmailPreference = EmailPreference.Any,
                        Role = Role.User
                    }
               ).ConfigureAwait(false);

            if (insertAccountResult.DuplicateUser)
            {
                createAccountDescriptor.Result = CreateUserAccountResult.DuplicateUser;
                return createAccountDescriptor;
            }

            if (email != null)
            {
                await _accountEmailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken);
            }

            createAccountDescriptor.Result = CreateUserAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }    
        public async Task<CreateAdminAccountDescriptor> CreateAdminAsync(string username, string token, string password, string email)
        {
            var createAccountDescriptor = new CreateAdminAccountDescriptor();

            if (_adminOptions.Token != token)
            {
                createAccountDescriptor.Result = CreateAdminAccountResult.InvaildToken;
                return createAccountDescriptor;
            }

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
                        EmailPreference = EmailPreference.Any,
                        Role = Role.Admin
                    }
               ).ConfigureAwait(false);

            if (insertAccountResult.DuplicateUser)
            {
                createAccountDescriptor.Result = CreateAdminAccountResult.DuplicateUser;
                return createAccountDescriptor;
            }

            if (email != null)
            {
                await _accountEmailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken);
            }

            createAccountDescriptor.Result = CreateAdminAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }
        public async Task<LoginDescriptor> LoginAsync(string username, string password)
        {
            var loginDescriptor = new LoginDescriptor();

            var selectAccountByUserNameRequest = new SelectAccountByUserNameRequest { Username = username };
            var account = await _accountDBService.SelectAccountByUserNameAsync(selectAccountByUserNameRequest);

            if (account == null)
            {
                loginDescriptor.Result = LoginResult.AccountNotFound;
                return loginDescriptor;
            }

            if (account.Locked)
            {
                loginDescriptor.Result = LoginResult.AccountLocked;
                return loginDescriptor;
            }

            var encryptResult = _passwordEncryptionService.Encrypt(password, account.Salt);

            if (account.PasswordHash != encryptResult.Hash)
            {
                loginDescriptor.Result = LoginResult.InvaildPassword;

                await _accountDBService.InsertPasswordAttemptFailedAsync(new InsertPasswordAttemptFailedRequest { AccountId = account.AccountId});

                return loginDescriptor;
            }

            loginDescriptor.Result = LoginResult.Successful;
            loginDescriptor.AccountId = account.AccountId;
            loginDescriptor.Role = account.Role;

            return loginDescriptor;
        }

        public async Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference)
        {
            await _accountDBService.UpdateEmailPreferenceAsync
            (
                new Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest 
                { 
                    AccountId = accountId,
                    EmailPreference = emailPreference 
                }
            );
        }

        public async Task<Abstractions.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string token, EmailPreference emailPreference)
        {
            var updateEmailPreferenceWithTokenResult = 
                await _accountDBService.UpdateEmailPreferenceWithTokenAsync
                      (
                          new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenRequest 
                          {  
                              EmailPreferenceToken = token,
                              EmailPreference = emailPreference
                          }
                      );

            if (!updateEmailPreferenceWithTokenResult.VaildToken)
            {
                return Abstractions.UpdateEmailPreferenceWithTokenResult.InvaildToken;
            }

            return Abstractions.UpdateEmailPreferenceWithTokenResult.Successful;
        }
    }
}
