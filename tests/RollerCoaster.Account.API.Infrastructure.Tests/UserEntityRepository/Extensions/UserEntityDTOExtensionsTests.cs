using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Models;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Extensions
{
    [TestClass]
    public class UserEntityDTOExtensionsTests
    {
        [TestMethod]
        public void ToUserEntityDTO_Runs_ReturnsUserEntityDTO()
        {
            // Arrange
            var userEntityDTO = new UserEntityDTO
            {
                ActivateEmailToken = "SampleActivateEmailToken",
                EmailActivated = false,
                Email = "SampleEmail",
                EmailPreference = EmailPreference.AccountOnly,
                EmailPreferenceToken = "SampleEmailPreferenceToken",
                PasswordHash = "SamplePasswordHash",
                Role = Role.User,
                Salt = "SampleSalt",
                Username = "SampleUsername",
                _etag = "SampleVersion",
                Id = "SampleUsername",
                Key = "SampleUsername",
            };

            // Act
            var userEntityData = userEntityDTO.ToUserEntityData();

            // Assert
            Assert.AreEqual(userEntityDTO.ActivateEmailToken   , userEntityData.ActivateEmailToken);
            Assert.AreEqual(userEntityDTO.EmailActivated       , userEntityData.EmailActivated);
            Assert.AreEqual(userEntityDTO.Email                , userEntityData.Email);
            Assert.AreEqual(userEntityDTO.EmailPreference      , userEntityData.EmailPreference);
            Assert.AreEqual(userEntityDTO.EmailPreferenceToken , userEntityData.EmailPreferenceToken);
            Assert.AreEqual(userEntityDTO.PasswordHash         , userEntityData.PasswordHash);
            Assert.AreEqual(userEntityDTO.Role                 , userEntityData.Role);
            Assert.AreEqual(userEntityDTO.Salt                 , userEntityData.Salt);
            Assert.AreEqual(userEntityDTO.Username             , userEntityData.Username);
            Assert.AreEqual(userEntityDTO._etag                , userEntityData.Version);
            Assert.AreEqual(userEntityDTO.Id                   , userEntityData.Username);
            Assert.AreEqual(userEntityDTO.Key                  , userEntityData.Username);
        }
    }
}
