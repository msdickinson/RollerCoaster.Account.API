CREATE PROCEDURE Account.InsertPasswordAttemptFailed
( 
	@accountId int
)
AS 
	insert into Account.FailedLogin
	(
		AccountId,
		DateTimeCreated
	)
	VALUES
	(
		@accountId,
		SYSDATETIME()
	)