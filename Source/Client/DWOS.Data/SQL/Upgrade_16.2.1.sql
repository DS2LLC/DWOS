-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.2.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Remove unused ContainerWeight setting
DELETE FROM ApplicationSettings
WHERE SettingName = 'ContainerWeight';

GO