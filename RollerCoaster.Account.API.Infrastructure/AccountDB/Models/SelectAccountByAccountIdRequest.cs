﻿using System.Diagnostics.CodeAnalysis;

namespace RollerCoaster.Account.API.Infrastructure.AccountDB.Models
{
    [ExcludeFromCodeCoverage]
    public class SelectAccountByAccountIdRequest
    {
        public int AccountId { get; set; }
    }
}