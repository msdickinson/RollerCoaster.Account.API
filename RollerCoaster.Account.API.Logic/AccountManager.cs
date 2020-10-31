using DickinsonBros.Email.Abstractions;
using DickinsonBros.Guid.Abstractions;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountEmail;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using RollerCoaster.Account.API.Logic.Models;
using System.Linq;
using System.Threading.Tasks;
namespace RollerCoaster.Account.API.Logic
{
    public class AccountManager : IAccountManager
    {
        internal readonly IGuidService _guidService;
        internal readonly IAccountDBService _accountDBService;
        internal readonly IPasswordEncryptionService _passwordEncryptionService;
        internal readonly IAccountEmailService _accountEmailService;
        internal readonly AccountManagerOptions _accountManagerOptions;
        internal readonly IEmailService _emailService;

        public AccountManager
        (
            IGuidService guidService,
            IAccountDBService accountDBService,
            IPasswordEncryptionService passwordEncryptionService,
            IAccountEmailService accountEmailService,
            IEmailService emailService,
            IOptions<AccountManagerOptions> accountManagerOptions
        )
        {
            _guidService = guidService;
            _accountDBService = accountDBService;
            _passwordEncryptionService = passwordEncryptionService;
            _accountEmailService = accountEmailService;
            _emailService = emailService;
            _accountManagerOptions = accountManagerOptions.Value;
        }
        public async Task<CreateUserAccountDescriptor> CreateUserAsync(string username, string password, string email)
        {
            var createAccountDescriptor = new CreateUserAccountDescriptor();

            if(email != null)
            {

                if (!_emailService.IsValidEmailFormat(email))
                {
                    createAccountDescriptor.Result = CreateUserAccountResult.InvaildEmailFormat;
                    return createAccountDescriptor;
                }
                if (!await _emailService.ValidateEmailDomain(email.Split("@").Last()).ConfigureAwait(false))
                {
                    createAccountDescriptor.Result = CreateUserAccountResult.InvaildEmailDomain;
                    return createAccountDescriptor;
                }
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
                await _accountEmailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken).ConfigureAwait(false);
            }

            createAccountDescriptor.Result = CreateUserAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }    
        
        public async Task<CreateAdminAccountDescriptor> CreateAdminAsync(string username, string token, string password, string email)
        {
            var createAccountDescriptor = new CreateAdminAccountDescriptor();

            if (_accountManagerOptions.AdminToken != token)
            {
                createAccountDescriptor.Result = CreateAdminAccountResult.InvaildToken;
                return createAccountDescriptor;
            }

            if (!_emailService.IsValidEmailFormat(email))
            {
                createAccountDescriptor.Result = CreateAdminAccountResult.InvaildEmailFormat;
                return createAccountDescriptor;
            }
            if (!await _emailService.ValidateEmailDomain(email).ConfigureAwait(false))
            {
                createAccountDescriptor.Result = CreateAdminAccountResult.InvaildEmailDomain;
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
                await _accountEmailService.SendActivateEmailAsync(email, username, activateEmailToken, emailPreferenceToken).ConfigureAwait(false);
            }

            createAccountDescriptor.Result = CreateAdminAccountResult.Successful;
            createAccountDescriptor.AccountId = insertAccountResult.AccountId;

            return createAccountDescriptor;
        }
       
        public async Task<LoginDescriptor> LoginAsync(string username, string password)
        {
            var loginDescriptor = new LoginDescriptor();

            var account = await _accountDBService.SelectAccountByUserNameAsync(username).ConfigureAwait(false);

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

                await _accountDBService.InsertPasswordAttemptFailedAsync(account.AccountId).ConfigureAwait(false);

                return loginDescriptor;
            }

            loginDescriptor.Result = LoginResult.Successful;
            loginDescriptor.AccountId = account.AccountId;
            loginDescriptor.Role = account.Role;

            return loginDescriptor;
        }

        public async Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference)
        {
            await _accountDBService.UpdateEmailPreferenceAsync(accountId, emailPreference).ConfigureAwait(false);
        }

        public async Task<Abstractions.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string token, EmailPreference emailPreference)
        {
            var updateEmailPreferenceWithTokenResult =
                await _accountDBService.UpdateEmailPreferenceWithTokenAsync(token.ToString(), emailPreference).ConfigureAwait(false);

            if (!updateEmailPreferenceWithTokenResult.VaildToken)
            {
                return Abstractions.UpdateEmailPreferenceWithTokenResult.InvaildToken;
            }

            return Abstractions.UpdateEmailPreferenceWithTokenResult.Successful;
        }

        public async Task<ActivateEmailResult> ActivateEmailAsync(string token)
        {
            var activateEmailWithTokenResult = await _accountDBService.ActivateEmailWithTokenAsync(token).ConfigureAwait(false);

            if (!activateEmailWithTokenResult.VaildToken)
            {
                return ActivateEmailResult.InvaildToken;
            }

            if (activateEmailWithTokenResult.EmailWasAlreadyActivated)
            {
                return ActivateEmailResult.EmailWasAlreadyActivated;
            }

            return ActivateEmailResult.Successful;
        }

        public async Task<UpdatePasswordResult> UpdatePasswordAsync(int accountId, string existingPassword, string newPassword)
        {
            var account = await _accountDBService.SelectAccountByAccountIdAsync(accountId).ConfigureAwait(false);

            if (account == null)
            {
                return UpdatePasswordResult.AccountNotFound;
            }

            if (account.Locked)
            {
                return UpdatePasswordResult.AccountLocked;
            }

            var encryptResult = _passwordEncryptionService.Encrypt(existingPassword, account.Salt);

            if (account.PasswordHash != encryptResult.Hash)
            {
                await _accountDBService.InsertPasswordAttemptFailedAsync(accountId).ConfigureAwait(false);

                return UpdatePasswordResult.InvaildExistingPassword;
            }

            var newEncryptResult = _passwordEncryptionService.Encrypt(newPassword);

            await _accountDBService.UpdatePasswordAsync(account.AccountId, newEncryptResult.Hash, newEncryptResult.Salt).ConfigureAwait(false);

            return UpdatePasswordResult.Successful;
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(string token, string newPassword)
        {
            var accountId = await _accountDBService.SelectAccountIdFromPasswordResetTokenAsync(token).ConfigureAwait(false);

            if (accountId == null)
            {
                return ResetPasswordResult.TokenInvaild;
            }

            var newEncryptResult = _passwordEncryptionService.Encrypt(newPassword);

            await _accountDBService.UpdatePasswordAsync((int)accountId, newEncryptResult.Hash, newEncryptResult.Salt).ConfigureAwait(false);

            return ResetPasswordResult.Successful;
        }

        public async Task<RequestPasswordResetEmailResult> RequestPasswordResetEmailAsync(string email)
        {
            var account = await _accountDBService.SelectAccountByEmailAsync(email).ConfigureAwait(false);

            if (account == null)
            {
                return RequestPasswordResetEmailResult.EmailNotFound;
            }

            if (!account.EmailActivated)
            {
                return RequestPasswordResetEmailResult.EmailNotActivated;
            }

            if (account.EmailPreference == EmailPreference.None)
            {
                return RequestPasswordResetEmailResult.NoEmailSentDueToEmailPreference;
            }

            string passwordResetToken = _guidService.NewGuid().ToString();
            await _accountDBService.InsertPasswordResetTokenAsync(account.AccountId, passwordResetToken).ConfigureAwait(false);

            await _accountEmailService.SendPasswordResetEmailAsync(account.Email, passwordResetToken, account.EmailPreferenceToken.ToString())
            .ConfigureAwait(false);

            return RequestPasswordResetEmailResult.Successful;
        }
    }
}
