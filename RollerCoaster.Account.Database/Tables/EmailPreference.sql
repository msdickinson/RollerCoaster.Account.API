CREATE TABLE [Account].[EmailPreference]
(
	[EmailPreferenceId] INT NOT NULL IDENTITY(1,1) CONSTRAINT EmailPreference_PK PRIMARY KEY,
	[Name] varchar(50) UNIQUE NOT NULL
)
