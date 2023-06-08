--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '20.3.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Attach COC on shipping notifications
ALTER TABLE dbo.d_Contact ADD
    IncludeCOCInShippingNotifications bit NOT NULL CONSTRAINT DF_d_Contact_IncludeCOCInShippingNotifications DEFAULT 0;
GO