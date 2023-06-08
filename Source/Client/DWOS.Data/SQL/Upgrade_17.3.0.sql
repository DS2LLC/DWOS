-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.3.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Order Priority for Receiving

ALTER TABLE dbo.Receiving ADD
    Priority nvarchar(50) NULL
GO

ALTER TABLE dbo.Receiving ADD CONSTRAINT
    FK_Receiving_d_Priority FOREIGN KEY
    (
    Priority
    ) REFERENCES dbo.d_Priority
    (
    PriorityID
    ) ON UPDATE CASCADE
     ON DELETE SET NULL
GO

INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('Receiving.Priority','Ability to set priority during Receiving.', 'Shipping')
GO

-- Delete Media Permission
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('DeleteMedia','Ability to delete media and document links.', 'Other')
GO

INSERT INTO [dbo].[SecurityGroup_Role] (SecurtyGroupID, SecurityRoleID)
SELECT SecurityGroupID, 'DeleteMedia'
FROM SecurityGroup;

GO

-- Employee Performance Report Permission
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('EmployeePerformanceReport','Ability to run the Employee Performance Report.', 'Reports')
GO

-- Process Question Groups for Process Answer Report
ALTER TABLE dbo.ProcessQuestion ADD
    IdentifiesProcessGroup bit NOT NULL CONSTRAINT DF_ProcessQuestion_IdentifiesProcessGroup DEFAULT 0
GO

-- DaysOfWeek - table to store workweek info
CREATE TABLE dbo.DayOfWeek
    (
    DayOfWeekID int NOT NULL,
    IsWorkday bit NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.DayOfWeek ADD CONSTRAINT
    PK_DayOfWeek PRIMARY KEY CLUSTERED 
    (
    DayOfWeekID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO dbo.DayOfWeek(DayOfWeekID, IsWorkday) VALUES
(0, 0), -- The IDs match the DayOfWeek enum in .NET, starting with Sunday
(1, 1),
(2, 1),
(3, 1),
(4, 1),
(5, 1),
(6, 0);

GO

--
-- Process Burden
--

-- Rate
ALTER TABLE dbo.Process
ADD
    BurdenCostRate decimal(8, 5) NULL
GO

-- Total for Order
ALTER TABLE dbo.OrderProcesses ADD
    BurdenCost decimal(11, 5) NULL
GO

--
-- Optionally list product classes in order entry.
--
CREATE TABLE dbo.ProductClass
    (
    ProductClassID int NOT NULL IDENTITY (1, 1),
    Name nvarchar(255) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.ProductClass ADD CONSTRAINT
    PK_ProductClass PRIMARY KEY CLUSTERED 
    (
    ProductClassID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
