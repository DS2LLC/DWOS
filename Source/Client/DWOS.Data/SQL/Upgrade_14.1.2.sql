-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.1.2'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO


-- Add new Frozen 
ALTER TABLE dbo.Process ADD FrozenBy nvarchar(250) NULL, FrozenDate date NULL
GO


-- Update Unused Media to NOT delete media of Type PartTemp
ALTER PROCEDURE [dbo].[Delete_UnusedMedia]
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @rows INT

  SET @rows = (SELECT count(*)
               FROM
                 Media)

  DELETE
  FROM
    Media
  WHERE
    FileExtension != 'PartTemp' AND
    MediaID IN (SELECT m.MediaID
                FROM
                  Media m
                WHERE
                  NOT EXISTS (SELECT *
                              FROM
                                Part_Media pm
                              WHERE
                                m.MediaID = pm.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [Order_Media] o
                              WHERE
                                m.MediaID = o.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [QuotePart_Media] o
                              WHERE
                                m.MediaID = o.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [Users] u
                              WHERE
                                m.MediaID = u.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)

END
GO


-- Updat Fields Table with additional columns
ALTER TABLE dbo.Fields ADD
	IsRequired bit NOT NULL CONSTRAINT DF_Fields_IsRequired DEFAULT 1,
	IsSystem bit NOT NULL CONSTRAINT DF_Fields_IsSystem DEFAULT 0,
	IsVisible bit NOT NULL CONSTRAINT DF_Fields_IsVisible DEFAULT 1
GO

INSERT INTO [dbo].[Fields] ([Name],[Alias],[Category],[IsRequired],[IsSystem] ,[IsVisible])
     VALUES ('Material', null,'Part',0,1,1)
GO

INSERT INTO [dbo].[Fields] ([Name],[Alias],[Category],[IsRequired],[IsSystem] ,[IsVisible])
     VALUES ('Surface Area', null,'Part',0,1,1)
GO

INSERT INTO [dbo].[Fields] ([Name],[Alias],[Category],[IsRequired],[IsSystem] ,[IsVisible])
     VALUES ('Documents', null,'Order',1,0,1)
GO

INSERT INTO [dbo].[Fields] ([Name],[Alias],[Category],[IsRequired],[IsSystem] ,[IsVisible])
     VALUES ('Manufacturer', null,'Part',0,1,1)
GO

-- Change Part Rev to a System level Field
UPDATE Fields SET [IsSystem] = 1 WHERE Name = 'Part Rev.'
GO

-- Delete any references at the customer level to Part Rev
DELETE FROM Customer_Fields WHERE FieldID = (SELECT FieldId FROM Fields WHERE Name = 'Part Rev.')
GO