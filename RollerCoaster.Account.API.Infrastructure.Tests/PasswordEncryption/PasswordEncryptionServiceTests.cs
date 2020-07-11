using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Org.BouncyCastle.Crypto.Prng;
using RollerCoaster.Account.API.Infrastructure.PasswordEncryption;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.Tests.PasswordEncryption
{
        [TestClass]
        public class PasswordEncryptionServiceTests : BaseTest
        {
            #region SendActivateEmailAsync

            [TestMethod]
            public async Task Encrypt_SaltIsNull_ReturnsEncryptResult()
            {
                await RunDependencyInjectedTestAsync
                (
                    async (serviceProvider) =>
                    {
                        //Setup
                        var password = "samplePassword";
                     
                        var uut = serviceProvider.GetRequiredService<IPasswordEncryptionService>();
                        var uutConcrete = (PasswordEncryptionService)uut;

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

        [TestMethod]
        public async Task Encrypt_SaltIsNotNull_ReturnsEncryptResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var password = "samplePassword";
                    var salt = "daiqYbN6xgm2dlXDi+onZw==";
                    var expectedHash = "ino8pWjmaym0mWY01d3LHpfllD2VgV+m7174pLjwpWk=";

                    var uut = serviceProvider.GetRequiredService<IPasswordEncryptionService>();
                    var uutConcrete = (PasswordEncryptionService)uut;

                    //Act
                    var observed = uut.Encrypt(password, salt);

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
                serviceCollection.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
                serviceCollection.AddSingleton(Mock.Of<IRandomGenerator>());
                return serviceCollection;
            }
            #endregion
        }
    }
