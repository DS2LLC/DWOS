-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.1.0'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Add Column IsActive to d_Material table.
ALTER TABLE d_Material ADD IsActive bit NOT NULL DEFAULT ((1))
GO

-- Add Column IsActive to d_Airframe table.
ALTER TABLE d_Airframe ADD IsActive bit NOT NULL DEFAULT ((1))
GO

-- Add a new security role for running reports that contain financial data.
INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('SalesReports', 'Ability to run sales related reports.', 'Reports')
GO



-- Add a new security role for holiday manager
INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('HolidayManager', 'Ability to edit the holiday manager.', 'Reports')
GO