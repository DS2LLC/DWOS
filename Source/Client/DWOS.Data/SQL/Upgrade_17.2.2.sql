-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.2.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Rename permission for order rejoin
UPDATE SecurityRole
SET SecurityRoleID = 'OrderRejoin',
    Description = 'Ability to rejoin orders.'
WHERE SecurityRoleID = 'OrderEntry.Rejoin'

GO

-- Permission for splitting orders
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('OrderSplit', 'Ability to split orders.', 'Sales')
GO

INSERT INTO SecurityGroup_Role (SecurtyGroupID, SecurityRoleID)
SELECT SecurtyGroupID,
    'OrderSplit'
FROM SecurityGroup_Role
WHERE SecurityRoleID = 'OrderEntry.Edit';

GO