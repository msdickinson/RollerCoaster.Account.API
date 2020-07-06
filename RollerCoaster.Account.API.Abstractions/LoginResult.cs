using System;
using System.Collections.Generic;
using System.Text;

namespace RollerCoaster.Account.API.Abstractions
{
    public enum LoginResult
    {
        InvaildPassword,
        AccountNotFound,
        Successful,
        AccountLocked
    }
}
