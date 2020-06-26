CREATE PROCEDURE [Account].UpdateEmailActiveWithToken
	@activateEmailToken varchar(50)
AS

declare @vaildToken bit = 
	IIF(
		  (
			  select count(*)
			  from Account.Account
		      where Account.Account.ActivateEmailToken = @activateEmailToken 
	      ) = 0,
		  0,
		  1) 

declare @emailWasAlreadyActivated bit = 
	IIF(
		  (
			  select count(*)
			  from Account.Account
		      where Account.Account.ActivateEmailToken = @activateEmailToken and
			        Account.Account.EmailActivated = 1
	      ) = 0,
		  0,
		  1) 

IF (@vaildToken = 1 AND @emailWasAlreadyActivated = 0)
BEGIN
	update Account.Account
	Set
		EmailActivated = 1,
		EmailActivatedDate = SYSUTCDATETIME()
	where Account.Account.ActivateEmailToken = @activateEmailToken
END

select 
	@vaildToken as VaildToken,
	@emailWasAlreadyActivated as EmailWasAlreadyActivated


