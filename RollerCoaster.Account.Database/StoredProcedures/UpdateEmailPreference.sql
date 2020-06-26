CREATE PROCEDURE [Account].[UpdateEmailPreference]
	@accountId int,
	@emailPreference int
AS
	update Account.Account
	Set EmailPreference = @emailPreference
	where Account.Account.AccountId = @accountId