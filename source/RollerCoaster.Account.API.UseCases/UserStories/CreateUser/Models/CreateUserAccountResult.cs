namespace RollerCoaster.Account.API.UseCases.UserStorys.CreateUser.Models
{
    public enum CreateUserAccountResult
    {
        DuplicateUser,
        Successful,
        InvaildEmailFormat,
        InvaildEmailDomain,
        FailedToProcess,
        BadRequest
    }
}
