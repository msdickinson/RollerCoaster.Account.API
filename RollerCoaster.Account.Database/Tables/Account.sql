CREATE TABLE [Account].[Account]
(
	AccountId INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_Account PRIMARY KEY,
	Username varchar(50) CONSTRAINT Account_UNQ UNIQUE NOT NULL,
	PasswordHash varchar(50) NOT NULL,
	Salt varchar(50) NOT NULL,
	Email varchar(50) NULL,
	EmailPreference INT NOT NULL CONSTRAINT FK_Account_EmailPreference FOREIGN KEY REFERENCES  [Account].[EmailPreference](EmailPreferenceId),
	EmailPreferenceToken uniqueidentifier NOT NULL,
	ActivateEmailToken uniqueidentifier NOT NULL,
	EmailActivated bit NOT NULL,
	EmailActivatedDate DATETIME2 NULL,
	PasswordResetToken varchar(50) NULL,
    PasswordResetTokenCreated DATETIME2 NULL
)
