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

        internal const string SELECT_ACCOUNTID_BY_PASSWORD_RESET_TOKEN = "[Account].[SelectAccountIdByPasswordResetToken]";
        internal const string SELECT_ACCOUNT_BY_ACCOUNT_ID = "[Account].[SelectAccountByAccountId]";
        internal const string SELECT_ACCOUNT_BY_USERNAME = "[Account].[SelectAccountByUsername]";
        internal const string SELECT_ACCOUNT_BY_EMAIL = "[Account].[SelectAccountByEmail]";
        internal const string INSERT_ACCOUNT = "[Account].[Insert]";
        internal const string INSERT_PASSWORD_RESET_TOKEN = "[Account].[InsertPasswordResetToken]";
        internal const string INSERT_PASSWORD_ATTEMPT_FAILED = "[Account].[InsertPasswordAttemptFailed]";
        internal const string UPDATE_PASSWORD = "[Account].[UpdatePassword]";
        internal const string UPDATE_EMAIL_PREFERENCES = "[Account].[UpdateEmailPreference]";
        internal const string UPDATE_EMAIL_PREFERENCES_WITH_TOKEN = "[Account].[UpdateEmailPreferenceWithToken]";
        internal const string UPDATE_EMAIL_ACTIVE_WITH_TOKEN = "[Account].[UpdateEmailActiveWithToken]";
        internal const string DELETE_PASSWORD_RESET_TOKEN = "[Account].[DeletePasswordResetToken]";

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

        public async Task<Abstractions.Account> SelectAccountByUserNameAsync(string username)
        {
            return await _sqlService
                        .QueryFirstOrDefaultAsync<Abstractions.Account>
                         (
                             _connectionString,
                             SELECT_ACCOUNT_BY_USERNAME,
                             new
                             {
                                 Username = username
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Abstractions.Account> SelectAccountByEmailAsync(string email)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<Abstractions.Account>
                         (
                             _connectionString,
                             SELECT_ACCOUNT_BY_EMAIL,
                             new
                             {
                                 Email = email
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task<Abstractions.Account> SelectAccountByAccountIdAsync(int accountId)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<Abstractions.Account>
                          (
                              _connectionString,
                              SELECT_ACCOUNT_BY_ACCOUNT_ID,
                              new
                              {
                                  AccountId = accountId
                              },
                              commandType: CommandType.StoredProcedure
                          );
        }

        public async Task InsertPasswordResetTokenAsync(int accountId, string passwordResetToken)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _connectionString,
                      INSERT_PASSWORD_RESET_TOKEN,
                      new
                      {
                          AccountId = accountId,
                          PasswordResetToken = passwordResetToken
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task<int?> SelectAccountIdFromPasswordResetTokenAsync(string passwordResetToken)
        {
            return await _sqlService
                         .QueryFirstOrDefaultAsync<int?>
                         (
                             _connectionString,
                             SELECT_ACCOUNTID_BY_PASSWORD_RESET_TOKEN,
                             new
                             {
                                 PasswordResetToken = passwordResetToken
                             },
                             commandType: CommandType.StoredProcedure
                         );
        }

        public async Task UpdatePasswordAsync(int accountId, string passwordHash, string salt)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _connectionString,
                      UPDATE_PASSWORD,
                      new
                      {
                          AccountId = accountId,
                          PasswordHash = passwordHash,
                          Salt = salt
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task UpdateEmailPreferenceAsync(int accountId, EmailPreference emailPreference)
        {
            await _sqlService
                  .ExecuteAsync
                  (
                      _connectionString,
                      UPDATE_EMAIL_PREFERENCES,
                      new
                      {
                          AccountId = accountId,
                          EmailPreference = emailPreference
                      },
                      commandType: CommandType.StoredProcedure
                  );
        }

        public async Task<Models.UpdateEmailPreferenceWithTokenResult> UpdateEmailPreferenceWithTokenAsync(string emailPreferenceToken, EmailPreference emailPreference)
        {
            return await _sqlService
               .QueryFirstAsync<Models.UpdateEmailPreferenceWithTokenResult>
                   (
                       _connectionString,
                       UPDATE_EMAIL_PREFERENCES_WITH_TOKEN,
                       new
                       {
                           EmailPreferenceToken = emailPreferenceToken,
                           EmailPreference = emailPreference
                       },
                       commandType: CommandType.StoredProcedure
                   );
        }

        public async Task<ActivateEmailWithTokenResult> ActivateEmailWithTokenAsync(string activateEmailToken)
        {
            return await _sqlService
              .QueryFirstAsync<ActivateEmailWithTokenResult>
              (
                  _connectionString,
                  UPDATE_EMAIL_ACTIVE_WITH_TOKEN,
                  new
                  {
                      ActivateEmailToken = activateEmailToken
                  },
                  commandType: CommandType.StoredProcedure
              );
        }

        public async Task InsertPasswordAttemptFailedAsync(int accountId)
        {
            await _sqlService
               .ExecuteAsync
               (
                   _connectionString,
                   INSERT_PASSWORD_ATTEMPT_FAILED,
                   new
                   {
                       AccountId = accountId
                   },
                   commandType: CommandType.StoredProcedure
               );
        }
    }
}
