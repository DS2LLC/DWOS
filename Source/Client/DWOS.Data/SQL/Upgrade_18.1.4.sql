-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.1.4'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Process Product Class
--
CREATE TABLE dbo.ProcessProductClass
    (
    ProcessProductClassID int NOT NULL IDENTITY (1, 1),
    ProcessID int NOT NULL,
    ProductClass nvarchar(255) NULL,
    MaterialUnitCost decimal(8, 5) NULL,
    MaterialUnit nvarchar(3) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.ProcessProductClass ADD CONSTRAINT
    PK_ProcessProductClass PRIMARY KEY CLUSTERED 
    (
    ProcessProductClassID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ProcessProductClass ADD CONSTRAINT
    FK_ProcessProductClass_Process FOREIGN KEY
    (
    ProcessID
    ) REFERENCES dbo.Process
    (
    ProcessID
    ) ON UPDATE CASCADE
     ON DELETE CASCADE
    
GO

--
-- Delete unused settings
--

DELETE FROM ApplicationSettings
WHERE SettingName = 'ScheduleProductionType';

GO

DELETE FROM ApplicationSettings
WHERE SettingName = 'MultiSite';

GO

DELETE FROM ApplicationSettings
WHERE SettingName = 'DependentOrderThreshold';

GO

--
-- Delete unused permissions
--
DELETE
FROM SecurityRole
WHERE SecurityRoleID = 'OrderEntry.OrderClose';

GO

--
-- Rename columns/constraints for SecurityGroup_Role table
--
EXEC sp_rename 'SecurityGroup_Role.SecurtyGroupID', 'SecurityGroupID', 'COLUMN';
GO

EXEC sp_rename 'FK_SecurtyGroup_Role_SecurityGroup', 'FK_SecurityGroup_Role_SecurityGroup';
GO

EXEC sp_rename 'FK_SecurtyGroup_Role_SecurityRole', 'FK_SecurityGroup_Role_SecurityRole';
GO

EXEC sp_rename 'PK_SecurtyGroup_Role', 'PK_SecurityGroup_Role';
GO

--
-- Rename ShipmentPackage.Notfications column
--
EXEC sp_rename 'ShipmentPackage.Notfications', 'NotificationEmails';
GO

--
-- Original Order Type - to be restored after split-join/found
--
ALTER TABLE dbo.[Order] ADD
    OriginalOrderType int NULL
GO

-- Set appropriate default for existing orders
-- Keep normal, external rework, internal rework.
-- Use 1 as default for quarantine, lost, and rework hold.
UPDATE [Order]
Set OriginalOrderType = CASE WHEN OrderType <= 4 THEN OrderType ELSE 1 END;
GO

-- Set OriginalOrderType column to not null w/o default
ALTER TABLE [Order]
ALTER COLUMN OriginalOrderType int NOT NULL;
GO

--
-- Show/Hide/Require Customer Address
--
INSERT INTO Fields (Name, Alias, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Address', NULL, 'Customer', 1, 1, 1, 0
),
(
    'Carrier Code', NULL, 'Customer', 1, 1, 1, 0
);
GO

--
-- Conditions for inspection questions
--
CREATE TABLE dbo.PartInspectionQuestionCondition
    (
    PartInspectionQuestionConditionID int NOT NULL IDENTITY (1, 1),
    MainPartInspectionQuestionID int NOT NULL,
    CheckPartInspectionQuestionID int NOT NULL,
    Operator nvarchar(50) NOT NULL,
    Value nvarchar(255) NOT NULL
    ) ON [PRIMARY]
GO
ALTER TABLE dbo.PartInspectionQuestionCondition ADD CONSTRAINT
    PK_PartInspectionQuestionCondition PRIMARY KEY CLUSTERED 
    (
    PartInspectionQuestionConditionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.PartInspectionQuestionCondition ADD CONSTRAINT
    FK_PartInspectionQuestionCondition_CheckPartInspectionQuestion FOREIGN KEY
    (
    CheckPartInspectionQuestionID
    ) REFERENCES dbo.PartInspectionQuestion
    (
    PartInspectionQuestionID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION
GO
ALTER TABLE dbo.PartInspectionQuestionCondition ADD CONSTRAINT
    FK_PartInspectionQuestionCondition_MainPartInspectionQuestion FOREIGN KEY
    (
    MainPartInspectionQuestionID
    ) REFERENCES dbo.PartInspectionQuestion
    (
    PartInspectionQuestionID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION
 GO

 --
 -- Create Order History record for dependent orders.
 --
DECLARE @currentDate datetime2 = GETDATE();
INSERT INTO OrderHistory(OrderID, Category, Description, UserName, Machine, DateCreated)
SELECT OrderID, 'Notice', 'Originally created from primary order ' + CAST(PrimaryOrderID as varchar(10)) + '.', 'Server', HOST_NAME(), @currentDate
FROM [Order]
WHERE [Order].PrimaryOrderID IS NOT NULL;

GO

-- Remove PrimaryOrderID column and relationship from Order table
ALTER TABLE [Order]
DROP CONSTRAINT FK_Order_Order;

GO

ALTER TABLE [Order]
DROP COLUMN PrimaryOrderID;

GO

--
-- Start/end of workday for 'lead time (hours)' scheduler
--
ALTER TABLE DayOfWeek ADD
    WorkdayStart time(7) NULL,
    WorkdayEnd time(7) NULL;
GO

UPDATE DayOfWeek SET
    WorkdayStart = '9:00:00AM',
    WorkdayEnd = '5:00:00PM';
GO

ALTER TABLE DayOfWeek
ALTER COLUMN
    WorkdayStart time(7) NOT NULL;

ALTER TABLE DayOfWeek
ALTER COLUMN
    WorkdayEnd time(7) NOT NULL;

GO

--
-- Per-process lead time
--
ALTER TABLE dbo.Process ADD
    LeadTimeHours decimal(7, 4) NULL,
    LeadTimeType nvarchar(5) NULL;
GO

ALTER TABLE dbo.PartProcess ADD
    LeadTimeHours decimal(7, 4) NULL,
    LeadTimeType nvarchar(5) NULL;
GO

--
-- Batch.SalesOrderID
--
ALTER TABLE dbo.Batch ADD
    SalesOrderID int NULL;
GO
ALTER TABLE dbo.Batch ADD CONSTRAINT
    FK_Batch_SalesOrder FOREIGN KEY
    (
    SalesOrderID
    ) REFERENCES dbo.SalesOrder
    (
    SalesOrderID
    ) ON UPDATE CASCADE
     ON DELETE SET NULL;
GO

--
-- Batch COC
--
CREATE TABLE dbo.BatchCOC
    (
    BatchCOCID int NOT NULL IDENTITY (1, 1),
    BatchID int NOT NULL,
    DateCertified date NOT NULL,
    QAUser int NOT NULL,
    COCInfo nvarchar(MAX) NOT NULL,
    IsCompressed bit NOT NULL
    )  ON [PRIMARY]
     TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE dbo.BatchCOC ADD CONSTRAINT
    PK_BatchCOC PRIMARY KEY CLUSTERED 
    (
    BatchCOCID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.BatchCOC ADD CONSTRAINT
    FK_BatchCOC_Batch FOREIGN KEY
    (
    BatchID
    ) REFERENCES dbo.Batch
    (
    BatchID
    ) ON UPDATE NO ACTION
     ON DELETE CASCADE
    
GO

ALTER TABLE dbo.BatchCOC ADD CONSTRAINT
    FK_BatchCOC_Users FOREIGN KEY
    (
    QAUser
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE NO ACTION
     ON DELETE NO ACTION
    
GO

-- BatchCOCOrder
CREATE TABLE dbo.BatchCOCOrder
    (
    BatchCOCOrderID int NOT NULL IDENTITY (1, 1),
    BatchCOCID int NOT NULL,
    OrderID int NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.BatchCOCOrder ADD CONSTRAINT
    PK_BatchCOCOrder PRIMARY KEY CLUSTERED 
    (
    BatchCOCOrderID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.BatchCOCOrder ADD CONSTRAINT
    FK_BatchCOCOrder_BatchCOC FOREIGN KEY
    (
    BatchCOCID
    ) REFERENCES dbo.BatchCOC
    (
    BatchCOCID
    ) ON UPDATE NO ACTION
     ON DELETE CASCADE
    
GO
ALTER TABLE dbo.BatchCOCOrder ADD CONSTRAINT
    FK_BatchCOCOrder_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE NO ACTION
     ON DELETE CASCADE
GO

-- BatchCOCNotification
CREATE TABLE [dbo].[BatchCOCNotification] (
    [BatchCOCNotificationID] [int] IDENTITY(1, 1) NOT NULL,
    [BatchCOCID] [int] NOT NULL,
    [ContactID] [int] NOT NULL,
    [NotificationSent] [datetime2](7) NULL,
    CONSTRAINT [PK_BatchCOCNotification] PRIMARY KEY CLUSTERED ([BatchCOCNotificationID] ASC) WITH (
        PAD_INDEX = OFF,
        STATISTICS_NORECOMPUTE = OFF,
        IGNORE_DUP_KEY = OFF,
        ALLOW_ROW_LOCKS = ON,
        ALLOW_PAGE_LOCKS = ON
        ) ON [PRIMARY]
    ) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BatchCOCNotification]
    ADD CONSTRAINT [FK_BatchCOCNotification_BatchCOC] FOREIGN KEY
    (
    [BatchCOCID]
    ) REFERENCES [dbo].[BatchCOC]
    (
    [BatchCOCID]
    ) ON UPDATE CASCADE
    ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchCOCNotification] ADD CONSTRAINT
    [FK_BatchCOCNotification_d_Contact] FOREIGN KEY
    (
    [ContactID]
    ) REFERENCES [dbo].[d_Contact]
    (
    [ContactID]
    ) ON UPDATE CASCADE
    ON DELETE CASCADE
GO

--
-- Fix issue with fnGetCurrentProcessDate.
--

-- Return datetime2; includes actual time.
ALTER FUNCTION [dbo].[fnGetCurrentProcessDate] 
(
    @orderID int
)
RETURNS datetime2(7)
AS
BEGIN
    -- FIND THE NEXT DEPARTMENT FOR THE ORDER
    DECLARE @dueDate datetime2(7)
    
    -- Find Next process that is not completed
    SET @dueDate = 
    (
        SELECT  TOP(1) EstEndDate FROM [OrderProcesses] 
        WHERE EndDate IS NULL AND OrderID = @orderID
        ORDER BY StepOrder
    )
    
    -- If no more processes left then return last processed date
    IF (@dueDate IS NULL)
        BEGIN
            SET @dueDate = 
                (
                    SELECT  TOP(1) EstEndDate FROM [OrderProcesses] 
                    WHERE EndDate IS NOT NULL AND OrderID = @orderID
                    ORDER BY StepOrder DESC
                )

            -- if date was set then subtract 100 years to let client know this is the last processed end date
            IF (@dueDate IS NOT NULL AND DATEPART(year, @dueDate) > 100)
                BEGIN
                    SET @dueDate = DATEADD(year, -100, @dueDate) 
                END
        END
            
    Return @dueDate

END
GO