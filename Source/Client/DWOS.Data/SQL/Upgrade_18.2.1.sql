﻿-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.2.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Permission for changing file extensions for media
--
 INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('ChangeMediaFileExtension','Ability to change file extensions for media.', 'Other')
GO

INSERT INTO [dbo].[SecurityGroup_Role] (SecurityGroupID, SecurityRoleID)
SELECT SecurityGroupID, 'ChangeMediaFileExtension'
FROM SecurityGroup_Role
WHERE SecurityRoleID = 'DeleteMedia';

GO

--
-- Fix truncation errors in triggers
--

ALTER TRIGGER [dbo].[d_Manufacturer_Audit_Update] ON [dbo].[d_Manufacturer]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- adapted from script generated by AutoAudit on Jul 22 2009 10:55PM
 -- originally created by Paul Nielsen 
DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 IF UPDATE([ManufacturerID])

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(), 'dbo.d_Manufacturer', 'u', Convert(VARCHAR(20), Inserted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ManufacturerID]', Convert(VARCHAR(50), Deleted.[ManufacturerID]),  Convert(VARCHAR(50), Inserted.[ManufacturerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[ManufacturerID] = Deleted.[ManufacturerID]
               AND isnull(Inserted.[ManufacturerID],'') <> isnull(Deleted.[ManufacturerID],'')

 IF UPDATE([COC])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()),CONVERT(VARCHAR(50), APP_NAME()), Host_Name(), 'dbo.d_Manufacturer', 'u', Convert(VARCHAR(20), Inserted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[COC]', Convert(VARCHAR(50), Deleted.[COC]),  Convert(VARCHAR(50), Inserted.[COC]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[ManufacturerID] = Deleted.[ManufacturerID]
               AND isnull(Inserted.[COC],'') <> isnull(Deleted.[COC],'');

 GO

 ALTER TRIGGER [dbo].[d_Manufacturer_Audit_Insert] ON [dbo].[d_Manufacturer]
 AFTER Insert
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- adapted from script generated by AutoAudit on Jul 22 2009 10:55PM
 -- originally created by Paul Nielsen 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, NewValue, RowVersion)
   SELECT  Inserted.Created, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(), 'dbo.d_Manufacturer', 'i', CONVERT(VARCHAR(20), Inserted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ManufacturerID]', Cast(Inserted.[ManufacturerID] as VARCHAR(50)), 1
          FROM Inserted
          WHERE Inserted.[ManufacturerID] is not null;

 GO

 ALTER TRIGGER [dbo].[d_Manufacturer_Audit_Delete] ON [dbo].[d_Manufacturer]
 AFTER Delete
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- adapted from script generated by AutoAudit on Jul 22 2009 10:55PM
 -- originally created by Paul Nielsen 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(),'dbo.d_Manufacturer', 'd', CONVERT(VARCHAR(20), deleted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[ManufacturerID]', Convert(VARCHAR(50), Deleted.[ManufacturerID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[ManufacturerID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(),'dbo.d_Manufacturer', 'd', CONVERT(VARCHAR(20), deleted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[COC]', Convert(VARCHAR(50), Deleted.[COC]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[COC] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(),'dbo.d_Manufacturer', 'd', CONVERT(VARCHAR(20), deleted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Created]', Convert(VARCHAR(50), Deleted.[Created]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Created] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(),'dbo.d_Manufacturer', 'd', CONVERT(VARCHAR(20), deleted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Modified]', Convert(VARCHAR(50), Deleted.[Modified]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Modified] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, CONVERT(VARCHAR(50), suser_sname()), CONVERT(VARCHAR(50), APP_NAME()), Host_Name(),'dbo.d_Manufacturer', 'd', CONVERT(VARCHAR(20), deleted.[ManufacturerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[RowVersion]', Convert(VARCHAR(50), Deleted.[RowVersion]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[RowVersion] is not null;

GO
