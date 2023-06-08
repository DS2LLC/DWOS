-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Additional serial number functionality
--
ALTER TABLE OrderSerialNumber
ALTER COLUMN Number nvarchar(20) NULL;

GO

ALTER TABLE dbo.OrderSerialNumber ADD
    Active bit NOT NULL CONSTRAINT DF_OrderSerialNumber_Active DEFAULT 1,
    DateRemoved datetime2(7) NULL
GO

--
-- Rejoin
--

INSERT INTO d_OrderChangeType (
    OrderChangeType,
    Name
    )
VALUES (
    3,
    'Rejoin'
    );

GO

INSERT INTO d_OrderChangeReason (
    ChangeType,
    Name
    )
VALUES (
    3,
    'Workflow'
    );

GO

 -- Permission for rejoin
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('OrderEntry.Rejoin', 'Ability to rejoin orders in order entry.', 'Sales')
GO