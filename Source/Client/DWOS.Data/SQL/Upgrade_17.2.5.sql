-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.2.5'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Customer Relationships

CREATE TABLE dbo.CustomerRelationship
    (
    CustomerRelationshipId int NOT NULL IDENTITY (1, 1),
    CustomerA int NOT NULL,
    CustomerB int NOT NULL,
    ShowOnPortal bit NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerRelationship ADD CONSTRAINT
    PK_CustomerRelationship PRIMARY KEY CLUSTERED 
    (
    CustomerRelationshipId
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerRelationship ADD CONSTRAINT
    FK_CustomerRelationship_Customer FOREIGN KEY
    (
    CustomerA
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO
ALTER TABLE dbo.CustomerRelationship ADD CONSTRAINT
    FK_CustomerRelationship_Customer1 FOREIGN KEY
    (
    CustomerB
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO