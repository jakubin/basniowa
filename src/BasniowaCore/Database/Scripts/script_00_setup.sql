:on error exit

BEGIN TRANSACTION

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = '_SchemaVersion'))
BEGIN
	CREATE TABLE [dbo].[_SchemaVersion]
	(
		[SchemaVersion] INT NOT NULL
	)
	
	INSERT INTO [dbo].[_SchemaVersion] ([SchemaVersion]) VALUES (0)
END

COMMIT
GO