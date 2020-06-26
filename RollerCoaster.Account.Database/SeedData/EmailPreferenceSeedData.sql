SET IDENTITY_INSERT Account.EmailPreference  ON


EXEC Account.EmailPreferenceUpsert 
		@emailPreferenceId = 1,
		@name = 'Any'
GO


EXEC Account.EmailPreferenceUpsert 
		@emailPreferenceId = 2,
		@name = 'AccountOnly'
go

EXEC Account.EmailPreferenceUpsert 
		@emailPreferenceId = 3,
		@name = 'None'
go

Delete 
From Account.EmailPreference 
Where emailPreferenceId Not In (1,2,3)

SET IDENTITY_INSERT Account.EmailPreference  OFF