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
                    Assert.AreEqual(insertAccountResult, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region SelectAccountByUserNameAsync

        [TestMethod]
        public async Task SelectAccountByUserNameAsync_Runs_QueryFirstOrDefaultAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var selectAccountByUserNameRequest = new SelectAccountByUserNameRequest 
                    { 
                        Username = "SampleUserName" 
                    };

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<SelectAccountByUserNameRequest>(),
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
                            accountResult
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByUserNameAsync(selectAccountByUserNameRequest);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                           (
                                observedConnectionString,
                                observedSQL,
                                observedParam,
                                observedCommandType
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(uutConcrete._connectionString, observedConnectionString);
                    Assert.AreEqual(connectionString, observedConnectionString);
                    Assert.AreEqual(AccountDBService.SELECT_ACCOUNT_BY_USERNAME, observedSQL);
                    Assert.AreEqual(selectAccountByUserNameRequest, (SelectAccountByUserNameRequest)observedParam);
                    Assert.AreEqual(CommandType.StoredProcedure, observedCommandType);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SelectAccountByUserNameAsync_Runs_ReturnsInsertAccountResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var selectAccountByUserNameRequest = new SelectAccountByUserNameRequest
                    {
                        Username = "SampleUserName"
                    };

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<SelectAccountByUserNameRequest>(),
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
                            accountResult
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByUserNameAsync(selectAccountByUserNameRequest);

                    //Assert
                    Assert.AreEqual(accountResult, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region InsertPasswordAttemptFailedAsync

        [TestMethod]
        public async Task InsertPasswordAttemptFailedAsync_Runs_ExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var insertPasswordAttemptFailedRequest = new InsertPasswordAttemptFailedRequest();

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertPasswordAttemptFailedRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        });


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.InsertPasswordAttemptFailedAsync(insertPasswordAttemptFailedRequest);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                           (
                                observedConnectionString,
                                observedSQL,
                                observedParam,
                                observedCommandType
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(uutConcrete._connectionString, observedConnectionString);
                    Assert.AreEqual(connectionString, observedConnectionString);
                    Assert.AreEqual(AccountDBService.INSERT_PASSWORD_ATTEMPT_FAILED, observedSQL);
                    Assert.AreEqual(insertPasswordAttemptFailedRequest, (InsertPasswordAttemptFailedRequest)observedParam);
                    Assert.AreEqual(CommandType.StoredProcedure, observedCommandType);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task InsertPasswordAttemptFailedAsync_Runs_ReturnsSuccessfuly()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var insertPasswordAttemptFailedRequest = new InsertPasswordAttemptFailedRequest();

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<InsertPasswordAttemptFailedRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        });


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.InsertPasswordAttemptFailedAsync(insertPasswordAttemptFailedRequest);

                    //Assert
                    //If Act Throws This will fail
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }


        #endregion

        #region InsertPasswordAttemptFailedAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceRequest = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest();

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        });


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.UpdateEmailPreferenceAsync(updateEmailPreferenceRequest);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                           (
                                observedConnectionString,
                                observedSQL,
                                observedParam,
                                observedCommandType
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(uutConcrete._connectionString, observedConnectionString);
                    Assert.AreEqual(connectionString, observedConnectionString);
                    Assert.AreEqual(AccountDBService.INSERT_PASSWORD_ATTEMPT_FAILED, observedSQL);
                    Assert.AreEqual(updateEmailPreferenceRequest, (InsertPasswordAttemptFailedRequest)observedParam);
                    Assert.AreEqual(CommandType.StoredProcedure, observedCommandType);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ReturnsSuccessfuly()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var updateEmailPreferenceRequest = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest();

                    var accountResult = new Abstractions.Account();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .Callback((string connectionString, string sql, object param, CommandType? commandType) =>
                        {
                            observedConnectionString = connectionString;
                            observedSQL = sql;
                            observedParam = param;
                            observedCommandType = commandType;
                        });


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.UpdateEmailPreferenceAsync(updateEmailPreferenceRequest);

                    //Assert
                    //If Act Throws This will fail
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
