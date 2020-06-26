CREATE PROCEDURE Account.UpdatePassword
( 
    @accountId int,
    @passwordHash varchar(50),
    @salt varchar(50)
)
AS 
  UPDATE Account.Account
  Set 
    PasswordHash = @passwordHash,
    Salt = @salt
  where AccountId = @accountId
