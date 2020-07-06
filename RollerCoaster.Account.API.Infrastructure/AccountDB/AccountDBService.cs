using DickinsonBros.SQL.Abstractions;
using Microsoft.Extensions.Options;
using RollerCoaster.Account.API.Abstractions;
using RollerCoaster.Account.API.Infrastructure.AccountDB.Models;
using System.Data;
using System.Threading.Tasks;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB
{
    public class AccountDBService : IAccountDBService
    {
        internal readonly string _connectionString;
        internal readonly ISQLService _sqlService;

        internal const string INSERT_ACCOUNT = "[Account].[Insert]";
        internal const string SELECT_ACCOUNT_BY_USERNAME = "[Account].[SelectAccountByUsername]";
        internal const string INSERT_PASSWORD_ATTEMPT_FAILED = "[Account].[InsertPasswordAttemptFailed]";
        internal const string UPDATE_EMAIL_PREFERENCE = "[Account].[UpdateEmailPreference]";
        internal const string UPDATE_EMAIL_PREFERENCES_WITH_TOKEN = "[Account].[UpdateEmailPreferenceWithToken]";

        public AccountDBService
        (
            IOptions<RollerCoasterDBOptions> rollerCoasterDBOptions,
            ISQLService sqlService
        )
        {
            _connectionString = rollerCoasterDBOptions.Value.ConnectionString;
            _sqlService = sqlService;
        }

        public async Task<InsertAccountResult> InsertAccountAsync(InsertAccountRequest insertAccountRequest)
        {
            return await _sqlService
                        .QueryFirstAsync<InsertAccountResult>
                         (
                             _connectionString,
                             INSERT_ACCOUNT,
                             insertAccountRequest,
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Abstractions.Account> SelectAccountByUserNameAsync(SelectAccountByUserNameRequest selectAccountByUserName)
        {
            return await _sqlService
                        .QueryFirstOrDefaultAsync<Abstractions.Account>
                         (
                             _connectionString,
                             SELECT_ACCOUNT_BY_USERNAME,
                             selectAccountByUserName,
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task InsertPasswordAttemptFailedAsync(InsertPasswordAttemptFailedRequest insertPasswordAttemptFailedRequest)
        {
            await _sqlService
                   .ExecuteAsync
                   (
                       _connectionString,
                       INSERT_PASSWORD_ATTEMPT_FAILED,
                       insertPasswordAttemptFailedRequest,
                       commandType: CommandType.StoredProcedure
                   );
        }

        public async Task UpdateEmailPreferenceAsync(Models.UpdateEmailPreferenceRequest updateEmailPreferenceRequest)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _connectionString,
                      UPDATE_EMAIL_PREFERENCE,
                      updateEmailPreferenceRequest,
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task<Models.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(Models.UpdateEmailPreferenceWithTokenRequest updateEmailPreferenceWithTokenRequest)
        {
            return await _sqlService
                         .QueryFirstAsync<Models.UpdateEmailPreferenceWithTokenResult>
                         (
                             _connectionString,
                             UPDATE_EMAIL_PREFERENCES_WITH_TOKEN,
                             updateEmailPreferenceWithTokenRequest,
                             commandType: CommandType.StoredProcedure
                         );
        }

    }
}
