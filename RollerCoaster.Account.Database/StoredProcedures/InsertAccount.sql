CREATE PROCEDURE [Account].[Insert]
	@username varchar(50),
	@passwordhash varchar(50),
	@salt varchar(50),
	@email varchar(50),
	@emailPreference int,
	@emailPreferenceToken uniqueidentifier,
	@activateEmailToken uniqueidentifier
AS
	begin transaction
		declare @duplicateUser bit = 
			(
				select count(*)
				from Account.Account
				where Username = @username
			);
		if(@duplicateUser = 0)
		BEGIN
			insert into Account.Account
				(
					Username,
					PasswordHash,
					Salt,
					Email,
					EmailPreference,
					EmailPreferenceToken,
					[ActivateEmailToken],
					EmailActivated
				)
				VALUES
				(
					LOWER(@username),
					@passwordhash,
					@salt,
					@email,
					@emailPreference,
					@emailPreferenceToken,
					@activateEmailToken,
					0
				)
		END;
	commit 
SELECT @duplicateUser as DuplicateUser, SCOPE_IDENTITY() as AccountId