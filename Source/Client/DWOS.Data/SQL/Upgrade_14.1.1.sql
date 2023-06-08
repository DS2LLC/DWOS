-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.1.1'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Add new Is Paperless process to table
ALTER TABLE dbo.Process ADD IsPaperless bit NOT NULL CONSTRAINT DF_Process_IsPaperless DEFAULT 1
GO