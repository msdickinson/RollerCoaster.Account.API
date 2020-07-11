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

        #region InsertAccount

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
                    var username = "SampleUserName";

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
                                It.IsAny<object>(),
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
                    var observed = await uut.SelectAccountByUserNameAsync(username);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.SELECT_ACCOUNT_BY_USERNAME,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(username, observedParam.GetType().GetProperty("Username").GetValue(observedParam));
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
                    var username = "SampleUserName";

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
                                It.IsAny<object>(),
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
                    var observed = await uut.SelectAccountByUserNameAsync(username);

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
                    var accountId = 1;

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
                                It.IsAny<object>(),
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
                    await uut.InsertPasswordAttemptFailedAsync(accountId);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                           (
                                uutConcrete._connectionString,
                                AccountDBService.INSERT_PASSWORD_ATTEMPT_FAILED,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(accountId, observedParam.GetType().GetProperty("AccountId").GetValue(observedParam));
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
                    var accountId = 1;

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
                    await uut.InsertPasswordAttemptFailedAsync(accountId);

                    //Assert
                    //If Act Throws This will fail
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }


        #endregion

        # region UpdateEmailPreferenceWithTokenAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_Runs_QueryFirstAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var emailPreferenceToken = "SampleEmailPreferenceToken";
                    var emailPreference = EmailPreference.Any;
                    var updateEmailPreferenceWithTokenResult = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstAsync<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
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
                            updateEmailPreferenceWithTokenResult
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(emailPreferenceToken, emailPreference);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstAsync<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.UPDATE_EMAIL_PREFERENCES_WITH_TOKEN,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(emailPreferenceToken, observedParam.GetType().GetProperty("EmailPreferenceToken").GetValue(observedParam));
                    Assert.AreEqual(emailPreference, observedParam.GetType().GetProperty("EmailPreference").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UpdateEmailPreferenceWithTokenAsync_Runs_ReturnsUpdateEmailPreferenceWithTokenResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var emailPreferenceToken = "SampleEmailPreferenceToken";
                    var emailPreference = EmailPreference.Any;
                    var updateEmailPreferenceWithTokenResult = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstAsync<Infrastructure.AccountDB.Models.UpdateEmailPreferenceWithTokenResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
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
                            updateEmailPreferenceWithTokenResult
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.UpdateEmailPreferenceWithTokenAsync(emailPreferenceToken, emailPreference);

                    //Assert
                    Assert.AreEqual(updateEmailPreferenceWithTokenResult, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UpdateEmailPreferenceAsync

        [TestMethod]
        public async Task UpdateEmailPreferenceAsync_Runs_ExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var emailPreference = EmailPreference.Any;

                    var updateEmailPreferenceRequest = new Infrastructure.AccountDB.Models.UpdateEmailPreferenceRequest();

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
                                It.IsAny<object>(),
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
                    await uut.UpdateEmailPreferenceAsync(accountId, emailPreference);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                           (
                                uutConcrete._connectionString,
                                AccountDBService.UPDATE_EMAIL_PREFERENCES,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(accountId, observedParam.GetType().GetProperty("AccountId").GetValue(observedParam));
                    Assert.AreEqual(emailPreference, observedParam.GetType().GetProperty("EmailPreference").GetValue(observedParam));
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
                    var accountId = 1;
                    var emailPreference = EmailPreference.Any;

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
                                It.IsAny<object>(),
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
                    await uut.UpdateEmailPreferenceAsync(accountId, emailPreference);

                    //Assert
                    //Act Throws is not successful
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region ActivateEmailWithTokenAsync

        [TestMethod]
        public async Task ActivateEmailWithTokenAsync_Runs_QueryFirstAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailToken = "SampleActivateEmailToken";
                    var activateEmailWithTokenResult = new ActivateEmailWithTokenResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstAsync<ActivateEmailWithTokenResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
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
                            activateEmailWithTokenResult
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.ActivateEmailWithTokenAsync(activateEmailToken);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstAsync<Infrastructure.AccountDB.Models.ActivateEmailWithTokenResult>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.UPDATE_EMAIL_ACTIVE_WITH_TOKEN,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(activateEmailToken, observedParam.GetType().GetProperty("ActivateEmailToken").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task ActivateEmailWithTokenAsync_Runs_ReturnsActivateEmailWithTokenResult()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var activateEmailToken = "SampleActivateEmailToken";
                    var activateEmailWithTokenResult = new ActivateEmailWithTokenResult();

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstAsync<ActivateEmailWithTokenResult>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
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
                            activateEmailWithTokenResult
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.ActivateEmailWithTokenAsync(activateEmailToken);

                    //Assert
                    Assert.AreEqual(activateEmailWithTokenResult, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region SelectAccountByEmailAsync

        [TestMethod]
        public async Task SelectAccountByEmailAsync_Runs_QueryFirstOrDefaultAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail";
                    var account = new Abstractions.Account();

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
                                It.IsAny<object>(),
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
                            account
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByEmailAsync(email);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.SELECT_ACCOUNT_BY_EMAIL,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(email, observedParam.GetType().GetProperty("Email").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task ActivateEmailWithTokenAsync_Runs_ReturnsAccount()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var email = "SampleEmail";
                    var account = new Abstractions.Account();

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .ReturnsAsync
                        (
                            account
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByEmailAsync(email);

                    //Assert
                    Assert.AreEqual(account, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region SelectAccountByAccountIdAsync

        [TestMethod]
        public async Task SelectAccountByAccountIdAsync_Runs_QueryFirstOrDefaultAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var account = new Abstractions.Account();

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
                                It.IsAny<object>(),
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
                            account
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByAccountIdAsync(accountId);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.SELECT_ACCOUNT_BY_ACCOUNT_ID,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(accountId, observedParam.GetType().GetProperty("AccountId").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SelectAccountByAccountIdAsync_Runs_ReturnsAccount()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var account = new Abstractions.Account();

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<Abstractions.Account>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .ReturnsAsync
                        (
                            account
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountByAccountIdAsync(accountId);

                    //Assert
                    Assert.AreEqual(account, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region InsertPasswordResetTokenAsync

        [TestMethod]
        public async Task InsertPasswordResetTokenAsync_Runs_ExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var passwordResetToken = "SamplePasswordResetToken";

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
                                It.IsAny<object>(),
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
                    await uut.InsertPasswordResetTokenAsync(accountId, passwordResetToken);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                            (
                                uutConcrete._connectionString,
                                AccountDBService.INSERT_PASSWORD_RESET_TOKEN,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                            ),
                            Times.Once
                       );
                    Assert.AreEqual(accountId, observedParam.GetType().GetProperty("AccountId").GetValue(observedParam));
                    Assert.AreEqual(passwordResetToken, observedParam.GetType().GetProperty("PasswordResetToken").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task InsertPasswordResetTokenAsync_Runs_ReturnsSuccessfuly()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var passwordResetToken = "SamplePasswordResetToken";

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
                                It.IsAny<CommandType>()
                            )
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.InsertPasswordResetTokenAsync(accountId, passwordResetToken);

                    //Assert
                    //If Act Throws This will fail
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region SelectAccountIdFromPasswordResetTokenAsync

        [TestMethod]
        public async Task SelectAccountIdFromPasswordResetTokenAsync_Runs_QueryFirstOrDefaultAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var passwordResetToken = "SampleEmail";
                    var accountId = (int?)1;

                    var observedConnectionString = (string)null;
                    var observedSQL = (string)null;
                    var observedParam = (object)null;
                    var observedCommandType = (CommandType?)null;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<int?>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
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
                            accountId
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountIdFromPasswordResetTokenAsync(passwordResetToken);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.QueryFirstOrDefaultAsync<int?>
                           (
                                uutConcrete._connectionString,
                                AccountDBService.SELECT_ACCOUNTID_BY_PASSWORD_RESET_TOKEN,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );
                    Assert.AreEqual(passwordResetToken, observedParam.GetType().GetProperty("PasswordResetToken").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task SelectAccountIdFromPasswordResetTokenAsyncc_Runs_ReturnsAccountId()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var passwordResetToken = "SampleEmail";
                    var accountId = (int?)1;

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.QueryFirstOrDefaultAsync<int?>
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
                                It.IsAny<CommandType>()
                            )
                        )
                        .ReturnsAsync
                        (
                            accountId
                        );


                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    var observed = await uut.SelectAccountIdFromPasswordResetTokenAsync(passwordResetToken);

                    //Assert
                    Assert.AreEqual(accountId, observed);
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        #endregion

        #region UpdatePasswordAsync

        [TestMethod]
        public async Task UpdatePasswordAsync_Runs_ExecuteAsyncCalled()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var passwordHash = "SamplePasswordHash";
                    var salt = "SampleSalt";

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
                                It.IsAny<object>(),
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
                    await uut.UpdatePasswordAsync(accountId, passwordHash, salt);

                    //Assert
                    sqlServiceMock
                       .Verify(
                            sqlService => sqlService.ExecuteAsync
                           (
                                uutConcrete._connectionString,
                                AccountDBService.UPDATE_PASSWORD,
                                It.IsAny<object>(),
                                CommandType.StoredProcedure
                           ),
                           Times.Once
                       );

                    Assert.AreEqual(accountId, observedParam.GetType().GetProperty("AccountId").GetValue(observedParam));
                    Assert.AreEqual(passwordHash, observedParam.GetType().GetProperty("PasswordHash").GetValue(observedParam));
                    Assert.AreEqual(salt, observedParam.GetType().GetProperty("Salt").GetValue(observedParam));
                },
               serviceCollection => ConfigureServices(serviceCollection)
           );
        }

        [TestMethod]
        public async Task UpdatePasswordAsync_Runs_ReturnsSuccessfuly()
        {
            await RunDependencyInjectedTestAsync
            (
                async (serviceProvider) =>
                {
                    //Setup
                    var accountId = 1;
                    var passwordHash = "SamplePasswordHash";
                    var salt = "SampleSalt";

                    var sqlServiceMock = serviceProvider.GetMock<ISQLService>();
                    sqlServiceMock
                        .Setup
                        (
                            sqlService => sqlService.ExecuteAsync
                            (
                                It.IsAny<string>(),
                                It.IsAny<string>(),
                                It.IsAny<object>(),
                                It.IsAny<CommandType>()
                            )
                        );

                    var uut = serviceProvider.GetRequiredService<IAccountDBService>();
                    var uutConcrete = (AccountDBService)uut;

                    //Act
                    await uut.UpdatePasswordAsync(accountId, passwordHash, salt);

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
