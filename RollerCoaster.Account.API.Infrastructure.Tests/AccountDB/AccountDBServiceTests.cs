using DickinsonBros.SQL.Abstractions;
using DickinsonBros.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Data;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Logic.Tests
{
    [TestClass]
    public class AccountDBServiceTests : BaseTest
    {
        const string connectionString = "SampleConnectionString";

        #region CreateAsync

        [TestMethod]
        public async Task InsertAccountAsync_Runs_QueryFirstAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var insertAccountRequest = new InsertAccountRequest();
                    var insertAccountResult = new InsertAccountResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.QueryFirstAsync<InsertAccountResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertAccountRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        })
                        .ReturnsAsync
                        (
                            insertAccountResult
                        );

               
                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.InsertAccountAsync(insertAccountRequest);

                    //Assert
                    sqlServiceMock
                       .Verify(
                           sqlService => sqlService.QueryFirstAsync<InsertAccountResult>
                           (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertAccountRequest>(),
                                It.IsAny<CommandType>()
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(uutConcrete._connectionString, observedConnectionString);
                    Assert.AreEqual(connectionString, observedConnectionString);
                    Assert.AreEqual(AccountDBService.INSERT_ACCOUNT, observedSQL);
                    Assert.AreEqual(insertAccountRequest, (InsertAccountRequest)observedParam);
                    Assert.AreEqual(CommandType.StoredProcedure, observedCommandType);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task InsertAccountAsync_Runs_ReturnsInsertAccountResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var insertAccountRequest = new InsertAccountRequest();
                    var insertAccountResult = new InsertAccountResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            passwordEncryptionService => passwordEncryptionService.QueryFirstAsync<InsertAccountResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertAccountRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        })
                        .ReturnsAsync
                        (
                            insertAccountResult
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.InsertAccountAsync(insertAccountRequest);

                    //Assert
                    sqlServiceMock
                       .Verify(
                           sqlService => sqlService.QueryFirstAsync<InsertAccountResult>
                           (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertAccountRequest>(),
                                It.IsAny<CommandType>()
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(insertAccountResult, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }


        #endregion

        #region Helpers

        private IServiceCollection ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAccountDBService, AccountDBService>();
            serviceCollection.AddSingleton(Mock.Of<ISQLService>());

            serviceCollection.AddOptions<RollerCoasterDBOptions>()
                .Configure((rollerCoasterDBOptions) => rollerCoasterDBOptions.ConnectionString = connectionString);

            return serviceCollection;
        }
        #endregion
    }
}
