--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '21.1.0.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO


--Flag new parts from receiving
ALTER TABLE dbo.Part ADD
	CreatedInReceiving bit NULL
GO
ALTER TABLE dbo.Part SET (LOCK_ESCALATION = TABLE)
GO