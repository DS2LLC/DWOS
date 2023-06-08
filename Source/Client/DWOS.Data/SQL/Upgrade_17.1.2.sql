-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.1.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

ALTER TABLE ShipmentPackage ADD GrossWeight DECIMAL(16, 8) NULL;
