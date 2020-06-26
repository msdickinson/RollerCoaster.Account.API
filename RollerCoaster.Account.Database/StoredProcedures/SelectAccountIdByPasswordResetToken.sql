CREATE PROCEDURE [Account].[UpdateEmailPreferencesWithToken]
	@emailPreferenceToken varchar(50),
	@emailPreference int
AS

declare @vaildToken bit = 
	CASE WHEN EXISTS 
	(
		select count(*)
		from Account.Account
		where Account.Account.EmailPreferenceToken = @emailPreferenceToken 
	) 
	THEN 1 
	ELSE 0 
	END 

IF (@vaildToken = 1)
BEGIN
	update Account.Account
	Set
		EmailPreference = @emailPreference
	where Account.Account.EmailPreferenceToken = @emailPreferenceToken
END

select @vaildToken as VaildToken
