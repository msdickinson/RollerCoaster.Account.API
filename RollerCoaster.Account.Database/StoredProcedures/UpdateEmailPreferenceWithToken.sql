CREATE PROCEDURE [Account].[UpdateEmailPreferenceWithToken]
	@emailPreferenceToken varchar(50),
	@emailPreference int
AS

declare @vaildToken bit = 
	IIF(
		  (
			  select count(*)
			  from Account.Account
			  where Account.Account.EmailPreferenceToken = @emailPreferenceToken
	      ) = 0,
		  0,
		  1) 
IF (@vaildToken = 1)
BEGIN
	update Account.Account
	Set
		EmailPreference = @emailPreference
	where Account.Account.EmailPreferenceToken = @emailPreferenceToken
END

select @vaildToken as VaildToken
