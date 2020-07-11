namespace RollerCoaster.Account.API.Logic.Models
{
    public enum UpdatePasswordResult
    {
        Successful,
        AccountLocked,
        InvaildExistingPassword,
        AccountNotFound
    }
}
