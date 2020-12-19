using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Entities.User;
using System;
using System.Text;

namespace RollerCoaster.Account.API.Entities.Tests.User
{
    [TestClass]
    public class UserEntityTests
    {
        #region  Constructor

        [TestMethod]
        public void Constructor_UserEntityData_SetsValues()
        {
            //Setup
            var userEntityData = new UserEntityData
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt",
                Version = "SampleVersion"
            };

            //Act
            var observed = new UserEntity(userEntityData);

            //Assert
            var userEntityDataObserved = observed.GetState();

            Assert.AreEqual(userEntityData.ActivateEmailToken    , userEntityDataObserved.ActivateEmailToken);
            Assert.AreEqual(userEntityData.Email                 , userEntityDataObserved.Email);
            Assert.AreEqual(userEntityData.EmailActivated        , userEntityDataObserved.EmailActivated);
            Assert.AreEqual(userEntityData.EmailPreference       , userEntityDataObserved.EmailPreference);
            Assert.AreEqual(userEntityData.EmailPreferenceToken  , userEntityDataObserved.EmailPreferenceToken);
            Assert.AreEqual(userEntityData.PasswordHash          , userEntityDataObserved.PasswordHash);
            Assert.AreEqual(userEntityData.Role                  , userEntityDataObserved.Role);
            Assert.AreEqual(userEntityData.Salt, userEntityDataObserved.Salt);
            Assert.AreEqual(userEntityData.Username              , userEntityDataObserved.Username);
            Assert.AreEqual(userEntityData.Version               , userEntityDataObserved.Version);
        }

        [TestMethod]
        public void Constructor_UserEntityRequest_SetsValues()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act 
            var observed = new UserEntity(userEntityRequest);

            //Assert

            var userEntityDataObserved = observed.GetState();
            Assert.AreEqual(userEntityRequest.ActivateEmailToken, userEntityDataObserved.ActivateEmailToken);
            Assert.AreEqual(userEntityRequest.Email, userEntityDataObserved.Email);
            Assert.AreEqual(userEntityRequest.EmailActivated, userEntityDataObserved.EmailActivated);
            Assert.AreEqual(userEntityRequest.EmailPreference, userEntityDataObserved.EmailPreference);
            Assert.AreEqual(userEntityRequest.EmailPreferenceToken, userEntityDataObserved.EmailPreferenceToken);
            Assert.AreEqual(userEntityRequest.PasswordHash, userEntityDataObserved.PasswordHash);
            Assert.AreEqual(userEntityRequest.Role, userEntityDataObserved.Role);
            Assert.AreEqual(userEntityRequest.Salt, userEntityDataObserved.Salt);
            Assert.AreEqual(userEntityRequest.Username, userEntityDataObserved.Username);
            Assert.AreEqual(null, userEntityDataObserved.Version);
        }

        #endregion

        #region Username

        [TestMethod]
        public void UsernameGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.Username;

            //Verfiy
            Assert.AreEqual(userEntityRequest.Username, observed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(UserEntity.Username))]
        public void UsernameSet_Null_ThrowsArgumentNullException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = null,
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.Username))]
        public void UsernameSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }
       
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.Username))]
        public void UsernameSet_UsernameLengthIsGreaterThen100_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = new StringBuilder(101).Insert(0, "A", 101).ToString(),
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void UsernameSet_UsernameLengthIsLessThenOrEqualTo100_SetsUsername()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = new StringBuilder(100).Insert(0, "A", 100).ToString(),
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            //Verfiy
            Assert.AreEqual(userEntityRequest.Username, observed.Username);
        }

        #endregion

        #region PasswordHash

        [TestMethod]
        public void PasswordHashGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.PasswordHash;

            //Verfiy
            Assert.AreEqual(userEntityRequest.PasswordHash, observed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(UserEntity.PasswordHash))]
        public void PasswordHashSet_Null_ThrowsArgumentNullException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = null,
                Salt = "SampleSalt"
            };

            //Act
           new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.PasswordHash))]
        public void PasswordHashSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.PasswordHash))]
        public void PasswordHashSet_PasswordHashLengthIsGreaterThen50_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = new StringBuilder(51).Insert(0, "A", 51).ToString(),
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void PasswordHashSet_PasswordHashLengthIsLessThenOrEqualTo50_SetsPasswordHash()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = new StringBuilder(50).Insert(0, "A", 50).ToString(),
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            // Assert
            Assert.AreEqual(userEntityRequest.PasswordHash, observed.PasswordHash);
        }

        #endregion

        #region Salt

        [TestMethod]
        public void SaltGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.Salt;

            //Verfiy
            Assert.AreEqual(userEntityRequest.Salt, observed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(UserEntity.Salt))]
        public void SaltSet_Null_ThrowsArgumentNullException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = null
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.Salt))]
        public void SaltSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = ""
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.Salt))]
        public void SaltSet_SaltLengthIsGreaterThen50_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePassword",
                Salt = new StringBuilder(51).Insert(0, "A", 51).ToString()
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void SaltSet_SaltLengthIsLessThenOrEqualTo50_SetsSalt()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SampleSalt",
                Salt = new StringBuilder(50).Insert(0, "A", 50).ToString()
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            // Assert
            Assert.AreEqual(userEntityRequest.Salt, observed.Salt);
        }

        #endregion

        #region ActivateEmailToken

        [TestMethod]
        public void ActivateEmailTokenGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.ActivateEmailToken;

            //Verfiy
            Assert.AreEqual(userEntityRequest.ActivateEmailToken, observed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(UserEntity.ActivateEmailToken))]
        public void ActivateEmailTokenSet_Null_ThrowsArgumentNullException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = null,
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.ActivateEmailToken))]
        public void ActivateEmailTokenSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.ActivateEmailToken))]
        public void ActivateEmailTokenSet_ActivateEmailTokenLengthIsGreaterThen50_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = new StringBuilder(51).Insert(0, "A", 51).ToString(),
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePassword",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void ActivateEmailTokenSet_ActivateEmailTokenLengthIsLessThenOrEqualTo50_SetsActivateEmailToken()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = new StringBuilder(50).Insert(0, "A", 50).ToString(),
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SampleSalt",
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            // Assert
            Assert.AreEqual(userEntityRequest.ActivateEmailToken, observed.ActivateEmailToken);
        }

        #endregion

        #region EmailPreferenceToken

        [TestMethod]
        public void EmailPreferenceTokenGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.EmailPreferenceToken;

            //Verfiy
            Assert.AreEqual(userEntityRequest.EmailPreferenceToken, observed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), nameof(UserEntity.EmailPreferenceToken))]
        public void EmailPreferenceTokenSet_Null_ThrowsArgumentNullException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = null,
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.EmailPreferenceToken))]
        public void EmailPreferenceTokenSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.EmailPreferenceToken))]
        public void EmailPreferenceTokenSet_EmailPreferenceTokenLengthIsGreaterThen50_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = new StringBuilder(51).Insert(0, "A", 51).ToString(),
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePassword",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void EmailPreferenceTokenSet_EmailPreferenceTokenLengthIsLessThenOrEqualTo50_SetsEmailPreferenceToken()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = new StringBuilder(50).Insert(0, "A", 50).ToString(),
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SampleSalt",
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            // Assert
            Assert.AreEqual(userEntityRequest.EmailPreferenceToken, observed.EmailPreferenceToken);
        }

        #endregion

        #region Email

        [TestMethod]
        public void EmailGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.Email;

            //Verfiy
            Assert.AreEqual(userEntityRequest.Email, observed);
        }

        [TestMethod]
        public void EmailSet_Null_EmailIsNull()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = null,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            //Verfiy
            Assert.IsNull(observed.Email);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), nameof(UserEntity.Email))]
        public void EmailSet_Empty_ThrowsArgumentException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "",
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), nameof(UserEntity.Email))]
        public void EmailSet_EmailLengthIsGreaterThen100_ThrowsArgumentOutOfRangeException()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = new StringBuilder(101).Insert(0, "A", 101).ToString(),
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePassword",
                Salt = "SampleSalt"
            };

            //Act
            new UserEntity(userEntityRequest);

            //Verfiy
        }

        [TestMethod]
        public void EmailSet_EmailLengthIsLessThenOrEqualTo100_SetsEmail()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = new StringBuilder(100).Insert(0, "A", 100).ToString(),
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SampleSalt",
                Salt = "SampleSalt"
            };

            //Act
            var observed = new UserEntity(userEntityRequest);

            // Assert
            Assert.AreEqual(userEntityRequest.Email, observed.Email);
        }

        #endregion

        #region EmailPreference

        [TestMethod]
        public void EmailPreferenceGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.EmailPreference;

            //Verfiy
            Assert.AreEqual(userEntityRequest.EmailPreference, observed);
        }

        [TestMethod]
        public void EmailPreferenceSet_EnumEmailPreference_SetsEmailPreference()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = null,
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                Role = Role.User,
                EmailPreference = EmailPreference.Any,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            userEntity.EmailPreference = EmailPreference.None;

            //Verfiy
            Assert.AreEqual(EmailPreference.None, userEntity.EmailPreference);
        }

        #endregion

        #region EmailActivated

        [TestMethod]
        public void EmailActivatedGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.EmailActivated;

            //Verfiy
            Assert.AreEqual(userEntityRequest.EmailActivated, observed);
        }

        [TestMethod]
        public void EmailActivatedSet_Runs_SetsEmailActivated()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = null,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                Role = Role.User,
                EmailActivated = true,
                EmailPreference = EmailPreference.Any,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            userEntity.EmailActivated = false;

            //Verfiy
            Assert.IsFalse(userEntity.EmailActivated);
        }

        #endregion

        #region Role

        [TestMethod]
        public void RoleGet_Runs_ReturnsValue()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            var observed = userEntity.Role;

            //Verfiy
            Assert.AreEqual(userEntityRequest.Role, observed);
        }

        [TestMethod]
        public void RoleSet_EnumRole_SetsRole()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = null,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                Role = Role.User,
                EmailPreference = EmailPreference.Any,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act
            userEntity.Role = Role.Admin;

            //Verfiy
            Assert.AreEqual(Role.Admin, userEntity.Role);
        }

        #endregion

        #region GetState

        [TestMethod]
        public void GetState_Runs_ReturnsState()
        {
            //Setup
            var userEntityRequest = new UserEntityRequest
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                Email = "SampleEmail@Email.com",
                EmailActivated = true,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                EmailPreference = EmailPreference.Any,
                Role = Role.User,
                Username = "SampleUsername",
                PasswordHash = "SamplePasswordHash",
                Salt = "SampleSalt"
            };

            var userEntity = new UserEntity(userEntityRequest);

            //Act 
            var observed = userEntity.GetState();

            //Assert
            Assert.AreEqual(userEntityRequest.ActivateEmailToken        , observed.ActivateEmailToken);
            Assert.AreEqual(userEntityRequest.Email                     , observed.Email);
            Assert.AreEqual(userEntityRequest.EmailActivated            , observed.EmailActivated);
            Assert.AreEqual(userEntityRequest.EmailPreference           , observed.EmailPreference);
            Assert.AreEqual(userEntityRequest.EmailPreferenceToken      , observed.EmailPreferenceToken);
            Assert.AreEqual(userEntityRequest.PasswordHash              , observed.PasswordHash);
            Assert.AreEqual(userEntityRequest.Role                      , observed.Role);
            Assert.AreEqual(userEntityRequest.Salt                      , observed.Salt);
            Assert.AreEqual(userEntityRequest.Username                  , observed.Username);
            Assert.AreEqual(null                                        , observed.Version);
        }

        #endregion
    }
}