using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.PasswordEncryption;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.PasswordEncryption
{
    [TestClass]
    public class PasswordEncryptionTests : BaseTest
    {
        #region Encrypt

        [TestMethod]
        public async Task Encrypt_Runs_ReturnsEncryptResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var password = "samplePassword";

                    var uut = serviceProvider.GetRequiredService<IPasswordEncryption>();
                    var uutConcrete = (Infrastructure.PasswordEncryption.PasswordEncryption)uut;

                    //Act
                    var observed = uut.Encrypt(password);

                    Assert.IsNotNull(observed);
                    Assert.IsNotNull(observed.Hash);
                    Assert.IsNotNull(observed.Salt);

                    await Task.CompletedTask;
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region EncryptWithSalt
        [TestMethod]
        public async Task EncryptWithSalt_Runs_ReturnsEncryptResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var password = "samplePassword";
                    var salt = "daiqYbN6xgm2dlXDi+onZw==";
                    var expectedHash = "ino8pWjmaym0mWY01d3LHpfllD2VgV+m7174pLjwpWk=";

                    var uut = serviceProvider.GetRequiredService<IPasswordEncryption>();
                    var uutConcrete = (Infrastructure.PasswordEncryption.PasswordEncryption)uut;

                    //Act
                    var observed = uutConcrete.EncryptWithSalt(password, salt);

                    Assert.IsNotNull(observed);
                    Assert.IsNotNull(expectedHash, observed.Hash);
                    Assert.IsNotNull(salt, observed.Salt);

                    await Task.CompletedTask;
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }
        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPasswordEncryption, Infrastructure.PasswordEncryption.PasswordEncryption>();
            return serviceCollection;
        }
        #endregion
    }
}
