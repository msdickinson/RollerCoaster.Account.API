CREATE PROCEDURE Account.EmailPreferenceUpsert
( 
	@emailPreferenceId int,
	@name nvarchar(max)
)
AS 
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
  BEGIN TRAN
 
    IF EXISTS ( SELECT * FROM Account.EmailPreference WITH (UPDLOCK) WHERE EmailPreferenceId = @emailPreferenceId )
 
      UPDATE Account.EmailPreference
         SET [Name] = @name
       WHERE EmailPreferenceId = @emailPreferenceId;
 
    ELSE 
 
      INSERT Account.EmailPreference 
	  ( EmailPreferenceId,  [Name])
      VALUES 
	  ( @emailPreferenceId, @name );
 
  COMMIT