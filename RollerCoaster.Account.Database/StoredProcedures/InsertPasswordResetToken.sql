CREATE PROCEDURE Account.InsertPasswordResetToken
( 
	@accountId int,
    @passwordResetToken varchar(50)
)
AS 
  UPDATE Account.Account
  Set 
    PasswordResetToken = @passwordResetToken,
    PasswordResetTokenCreated = SYSDATETIME()
  where AccountId = @accountId
