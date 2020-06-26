CREATE PROCEDURE [Account].[SelectAccountByEmail]
	@email varchar(50)
AS

WITH AccountLocked_CTE
AS
(select Account.Account.AccountId,
	  IIF(count(*) >= 5, 1, 0) as Locked
	  from Account.Account
	  left join Account.FailedLogin on Account.Account.AccountId = Account.FailedLogin.AccountId
	  Where Account.FailedLogin.DateTimeCreated >= DATEADD(minute, -5, SYSDATETIME())
	  group by Account.AccountId
)
select 
	Account.Account.AccountId,
	Account.Account.Username,
	Account.Account.PasswordHash,
	Account.Account.Salt,
	Account.Account.Email,
	Account.Account.EmailPreference,
	Account.Account.EmailPreferenceToken,
	Account.Account.ActivateEmailToken,
	Account.Account.EmailActivated,
	AccountLocked_CTE.Locked
from Account.Account
left join AccountLocked_CTE on Account.Account.AccountId = AccountLocked_CTE.AccountId
where Account.Account.Email = @email
	
RETURN 0
