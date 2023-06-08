-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.2.3'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Update tokens for COC template
UPDATE Templates
SET Tokens = '%ACCEPTEDTEXT%, %REJECTEDTEXT%, %TOTALTEXT%, %PROCESSTEXT%, %SERIALNUMBERS%, %IMPORTVALUE%'
WHERE TemplateID = 'COCData';

GO

--
-- Order Product Class
--

-- Allow fields that are both system-wide and customer-specific
ALTER TABLE dbo.Fields ADD
    IsCustomer bit NOT NULL CONSTRAINT DF_Fields_IsCustomer DEFAULT 0
GO

-- Existing fields are either per-customer or system-wide
UPDATE Fields
SET IsCustomer = ~IsSystem;

GO

-- Product Class Field
INSERT INTO Fields (Name, Alias, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Product Class', NULL, 'Order', 0, 1, 0, 1
);

-- Order Product Class table 
CREATE TABLE dbo.OrderProductClass
    (
    OrderProductClassID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    ProductClass nvarchar(255) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderProductClass ADD CONSTRAINT
    PK_OrderProductClass PRIMARY KEY CLUSTERED 
    (
    OrderProductClassID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderProductClass ADD CONSTRAINT
    FK_OrderProductClass_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION
     ON DELETE  CASCADE

GO

--
-- Add new security groups
--
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('AddContact','Ability to add contacts in Order Entry.', 'Sales')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('FieldMigration', 'Ability to use the field migration tool.', 'Administration')
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('ProcessingAnswersReport','Ability to run report.', 'Reports')
GO

--
-- Security Group tabs
--
CREATE TABLE dbo.SecurityGroupTab
    (
    SecurityGroupTabID int NOT NULL IDENTITY (1, 1),
    SecurityGroupID int NOT NULL,
    Name nvarchar(255) NOT NULL,
    DataType nvarchar(20) NOT NULL,
    TabKey nvarchar(36) NOT NULL,
    Layout nvarchar(MAX) NULL,
    TabOrder int NOT NULL,
    Version int NULL
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.SecurityGroupTab ADD CONSTRAINT
    PK_SecurityGroupTab PRIMARY KEY CLUSTERED 
    (
    SecurityGroupTabID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.SecurityGroupTab ADD CONSTRAINT
    FK_SecurityGroupTab_SecurityGroup FOREIGN KEY
    (
    SecurityGroupID
    ) REFERENCES dbo.SecurityGroup
    (
    SecurityGroupID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
GO

--
-- Lists for custom fields
--
ALTER TABLE dbo.CustomField ADD
    ListID int NULL
GO

ALTER TABLE dbo.CustomField ADD CONSTRAINT
    FK_CustomField_Lists FOREIGN KEY
    (
    ListID
    ) REFERENCES dbo.Lists
    (
    ListID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION

GO
