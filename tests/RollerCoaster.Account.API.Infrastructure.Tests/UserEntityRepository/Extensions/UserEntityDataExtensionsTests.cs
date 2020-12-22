using Microsoft.VisualStudio.TestTools.UnitTesting;
using RollerCoaster.Account.API.Entities.Models;
using RollerCoaster.Account.API.Infrastructure.UserEntityRepository.Extensions;

namespace RollerCoaster.Account.API.Infrastructure.Tests.UserEntityRepository.Extensions
{
    [TestClass]
    public class UserEntityDataExtensionsTests
    {
        [TestMethod]
        public void ToUserEntityDTO_Runs_ReturnsUserEntityDTO()
        {
            // Arrange
            var userEntityData = new UserEntityData
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
                Version = "SampleVersion"
            };

            // Act
            var userEntityDTO = userEntityData.ToUserEntityDTO();

            // Assert
            Assert.AreEqual(userEntityData.ActivateEmailToken   , userEntityDTO.ActivateEmailToken);
            Assert.AreEqual(userEntityData.EmailActivated       , userEntityDTO.EmailActivated);
            Assert.AreEqual(userEntityData.Email                , userEntityDTO.Email);
            Assert.AreEqual(userEntityData.EmailPreference      , userEntityDTO.EmailPreference);
            Assert.AreEqual(userEntityData.EmailPreferenceToken , userEntityDTO.EmailPreferenceToken);
            Assert.AreEqual(userEntityData.PasswordHash         , userEntityDTO.PasswordHash);
            Assert.AreEqual(userEntityData.Role                 , userEntityDTO.Role);
            Assert.AreEqual(userEntityData.Salt                 , userEntityDTO.Salt);
            Assert.AreEqual(userEntityData.Username             , userEntityDTO.Username);
            Assert.AreEqual(userEntityData.Version              , userEntityDTO._etag);
        }
    }
}
