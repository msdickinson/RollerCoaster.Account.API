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
    }
}
