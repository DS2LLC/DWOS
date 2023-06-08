-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- DateTimeIn/DateTimeOut input types
--

INSERT INTO d_InputType (InputType, AllowInInspection)
VALUES
('DateTimeIn', 1),
('DateTimeOut', 1);

GO
