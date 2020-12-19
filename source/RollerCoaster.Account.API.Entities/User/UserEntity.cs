using RollerCoaster.Account.API.Entities.Models;
using System;
using System.Linq;

namespace RollerCoaster.Account.API.Entities.User
{
    public class UserEntity : IUserEntity
    {
        internal readonly UserEntityData _userEntityData;

        public UserEntity(UserEntityData userEntityData)
        {
            _userEntityData = new UserEntityData();

            ActivateEmailToken      = userEntityData.ActivateEmailToken;
            Email                   = userEntityData.Email;
            EmailActivated          = userEntityData.EmailActivated;
            EmailPreferenceToken    = userEntityData.EmailPreferenceToken;
            Role                    = userEntityData.Role;
            Username                = userEntityData.Username;
            PasswordHash            = userEntityData.PasswordHash;
            Salt                    = userEntityData.Salt;
            EmailPreference         = userEntityData.EmailPreference;

            _userEntityData.Version = userEntityData.Version;
        }

        public UserEntity(UserEntityRequest userEntityRequest)
        {
            _userEntityData = new UserEntityData();

            ActivateEmailToken      = userEntityRequest.ActivateEmailToken;      
            Email                   = userEntityRequest.Email;
            EmailActivated          = userEntityRequest.EmailActivated;
            EmailPreferenceToken    = userEntityRequest.EmailPreferenceToken;  
            Role                    = userEntityRequest.Role;                                  
            Username                = userEntityRequest.Username;
            PasswordHash            = userEntityRequest.PasswordHash;
            Salt                    = userEntityRequest.Salt;
            EmailPreference         = userEntityRequest.EmailPreference;
        }

        public string Username
        {
            get
            {
                return _userEntityData.Username;
            }
            internal set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(Username));
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(Username));
                }

                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(Username), "Length exceeds 100");
                }

                _userEntityData.Username = value;
            }
        }

        public string PasswordHash
        {
            get
            {
                return _userEntityData.PasswordHash;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(PasswordHash));
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(PasswordHash));
                }
                if (value.Length > 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(PasswordHash), "Length exceeds 50");
                }

                _userEntityData.PasswordHash = value;
            }
        }

        public string Salt
        {
            get
            {
                return _userEntityData.Salt;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(Salt));
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(Salt));
                }
                if (value.Length > 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(Salt), "Length exceeds 50");
                }

                _userEntityData.Salt = value;
            }
        }

        public string ActivateEmailToken
        {
            get
            {
                return _userEntityData.ActivateEmailToken;
            }
            internal set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(ActivateEmailToken));
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(ActivateEmailToken));
                }
                if (value.Length > 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(ActivateEmailToken), "Length exceeds 50");
                }

                _userEntityData.ActivateEmailToken = value;
            }
        }

        public string EmailPreferenceToken
        {
            get
            {
                return _userEntityData.EmailPreferenceToken;
            }
            internal set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(EmailPreferenceToken));
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(nameof(EmailPreferenceToken));
                }
                if (value.Length > 50)
                {
                    throw new ArgumentOutOfRangeException(nameof(EmailPreferenceToken), "Length exceeds 50");
                }

                _userEntityData.EmailPreferenceToken = value;
            }
        }

        public string Email
        {
            get
            {
                return _userEntityData.Email;
            }
            internal set
            {
                if (value == null)
                {
                    _userEntityData.Email = null;
                    return;
                }
                if (value.Length == value.Where(character => character == ' ').Count())
                {
                    throw new ArgumentException(nameof(Email));
                }
                if (value.Length > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(Email), "Length exceeds 100");
                }

                _userEntityData.Email = value;
            }
        }

        public EmailPreference EmailPreference
        {
            get
            {
                return _userEntityData.EmailPreference;
            }
            set
            {
                _userEntityData.EmailPreference = value;
            }
        }

        public bool EmailActivated
        {
            get
            {
                return _userEntityData.EmailActivated;
            }
            set
            {
                _userEntityData.EmailActivated = value;
            }
        }

        public Role Role
        {
            get
            {
                return _userEntityData.Role;
            }
            set
            {
                _userEntityData.Role = value;
            }
        }

        public UserEntityData GetState()
        {
            return _userEntityData;
        }
    }
}
