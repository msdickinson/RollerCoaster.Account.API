using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Entities.User;
using RollerCoaster.Account.API.UseCases.Exceptions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.Email.Models;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryReader;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.UserEntityRepositoryWriter;
using RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models;
using System;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.UseCases.UserStorys.CreateUser
{
    public class CreateUserAccountInteractor : ICreateUserAccountInteractor
    {
        internal readonly IEmail _email;
        internal readonly IPasswordEncryption _passwordEncryption;
        internal readonly IGuidFactory _guidFactory;
        internal readonly IUserEntityRepositoryReader _userEntityRepositoryReader;
        internal readonly IUserEntityRepositoryWriter _userEntityRepositoryWriter;

        public CreateUserAccountInteractor
        (
            IEmail email,
            IPasswordEncryption passwordEncryption,
            IGuidFactory guidFactory,
            IUserEntityRepositoryReader userEntityRepositoryReader,
            IUserEntityRepositoryWriter userEntityRepositoryWriter
        )
        {
            _email = email;
            _passwordEncryption = passwordEncryption;
            _guidFactory = guidFactory;
            _userEntityRepositoryReader = userEntityRepositoryReader;
            _userEntityRepositoryWriter = userEntityRepositoryWriter;
        }

        public async Task<string> CreateUserAccountAsync(CreateUserAccountRequest createUserRequest)
        {
            await VaildateRequestAsync(createUserRequest).ConfigureAwait(false);
            var userEntity = CreateUserEntity(createUserRequest);
            userEntity = await SaveUserAsync(userEntity).ConfigureAwait(false);
            await SendActivateEmailAsync(userEntity).ConfigureAwait(false);

            return userEntity.Username;
        }

        #region VaildateRequestAsync
        internal async Task VaildateRequestAsync(CreateUserAccountRequest createUserRequest)
        {
            await VaildateEmailAsync(createUserRequest.Email).ConfigureAwait(false);
            VaildateUsername(createUserRequest.Username);
            VaildatePassword(createUserRequest.Password);
        }

        internal void VaildatePassword(string password)
        {
            if (password is null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (password.Length < 7)
            {
                throw new ArgumentException($"length is less then 7", nameof(password));
            }
        }

        internal void VaildateUsername(string username)
        {
            if (username is null)
            {
                throw new ArgumentNullException(nameof(username));
            }
        }

        internal async Task VaildateEmailAsync(string email)
        {
            if (email == null)
            {
                return;
            }

            await _email.VaildateEmailAsync(email).ConfigureAwait(false);
        }

        #endregion

        #region CreateUserEntity

        internal UserEntity CreateUserEntity(CreateUserAccountRequest createUserRequest)
        {
            var encryptResult = _passwordEncryption.Encrypt(createUserRequest.Password);

            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = _guidFactory.NewGuid().ToString(),
                Email = createUserRequest.Email,
                EmailActivated = false,
                EmailPreference = EmailPreference.Any,
                EmailPreferenceToken = _guidFactory.NewGuid().ToString(),
                PasswordHash = encryptResult.Hash,
                Role = Role.User,
                Salt = encryptResult.Salt,
                Username = createUserRequest.Username
            };

            var userEntity = new UserEntity(userEntityRequest);

            return userEntity;
        }

        #endregion

        #region SaveUserAsync
        internal async Task<UserEntity> SaveUserAsync(UserEntity userEntity)
        {
            if (await _userEntityRepositoryReader.UsernameExistsAsync(userEntity.Username).ConfigureAwait(false))
            {
                throw new DuplicateUsernameException();
            }

            //if 
            //(
            //    userEntity.Email != null &&
            //    await _userEntityRepositoryReader.EmailExistsAsync(userEntity.Email).ConfigureAwait(false)
            //)
            //{
            //    throw new DuplicateEmailException();
            //}

            var userEntityData = await _userEntityRepositoryWriter.SaveAsync(userEntity.GetState()).ConfigureAwait(false);
            return new UserEntity(userEntityData);
        }

        #endregion

        #region SendActivateEmailAsync
        internal async Task SendActivateEmailAsync(UserEntity userEntity)
        {
            if 
            (
                userEntity.Email is null || 
                userEntity.EmailActivated ||
                (
                    userEntity.EmailPreference != EmailPreference.AccountOnly &&
                    userEntity.EmailPreference != EmailPreference.Any
                )
            )
            {
                return;
            }

            var sendActivateEmailRequest = new SendActivateEmailRequest
            {
                ActivateToken = userEntity.ActivateEmailToken,
                Email = userEntity.Email,
                EmailPreferenceToken = userEntity.EmailPreferenceToken,
                Username = userEntity.Username
            };

            await _email.SendActivateEmailAsync(sendActivateEmailRequest).ConfigureAwait(false);
        }
        #endregion

    }
}