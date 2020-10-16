namespace RollerCoaster.Account.API.Logic.Models
{
    public enum CreateUserAccountResult
    {
        DuplicateUser,
        Successful,
        InvaildEmailFormat,
        InvaildEmailDomain
    }
}
