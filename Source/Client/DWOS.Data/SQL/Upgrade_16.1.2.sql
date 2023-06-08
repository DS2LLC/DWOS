-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.1.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Time Data Collection
--

-- Remove unused columns
ALTER TABLE OrderProcesses
DROP COLUMN StartTime;

ALTER TABLE OrderProcesses
DROP COLUMN RunTime;

GO

-- Process columns - enables edits to process time total
ALTER TABLE OrderProcesses
ADD ProcessDurationMinutes int NULL;

ALTER TABLE BatchProcesses
ADD ProcessDurationMinutes int NULL;

GO

-- Tables to store process time

CREATE TABLE dbo.OrderProcessesTime
    (
    OrderProcessesTimeID int NOT NULL IDENTITY (1, 1),
    OrderProcessesID int NOT NULL,
    StartTime datetime2(7) NOT NULL,
    EndTime datetime2(7) NULL,
    WorkStatus nvarchar(50) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderProcessesTime ADD CONSTRAINT
    PK_OrderProcessesTime PRIMARY KEY CLUSTERED 
    (
    OrderProcessesTimeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderProcessesTime ADD CONSTRAINT
    FK_OrderProcessesTime_OrderProcesses FOREIGN KEY
    (
    OrderProcessesID
    ) REFERENCES dbo.OrderProcesses
    (
    OrderProcessesID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

CREATE TABLE dbo.BatchProcessesTime
    (
    BatchProcessesTimeID int NOT NULL IDENTITY (1, 1),
    BatchProcessID int NOT NULL,
    StartTime datetime2(7) NOT NULL,
    EndTime datetime2(7) NULL,
    WorkStatus nvarchar(50) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.BatchProcessesTime ADD CONSTRAINT
    PK_BatchProcessesTime PRIMARY KEY CLUSTERED 
    (
    BatchProcessesTimeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BatchProcessesTime ADD CONSTRAINT
    FK_BatchProcessesTime_BatchProcesses FOREIGN KEY
    (
    BatchProcessID
    ) REFERENCES dbo.BatchProcesses
    (
    BatchProcessID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO

-- Tables to store Process-User relation
-- Mostly used for persisting Active/Inactive status for operators.

CREATE TABLE dbo.OrderProcessesOperator
    (
    OrderProcessesOperatorID int NOT NULL IDENTITY (1, 1),
    OrderProcessesID int NOT NULL,
    UserID int NOT NULL,
    Status nvarchar(8) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderProcessesOperator ADD CONSTRAINT
    PK_OrderProcessesOperator PRIMARY KEY CLUSTERED 
    (
    OrderProcessesOperatorID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderProcessesOperator ADD CONSTRAINT
    FK_OrderProcessesOperator_OrderProcesses FOREIGN KEY
    (
    OrderProcessesID
    ) REFERENCES dbo.OrderProcesses
    (
    OrderProcessesID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.OrderProcessesOperator ADD CONSTRAINT
    FK_OrderProcessesOperator_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

CREATE TABLE dbo.BatchProcessesOperator
    (
    BatchProcessesOperatorID int NOT NULL IDENTITY (1, 1),
    BatchProcessID int NOT NULL,
    UserID int NOT NULL,
    Status nvarchar(8) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.BatchProcessesOperator ADD CONSTRAINT
    PK_BatchProcessesOperator PRIMARY KEY CLUSTERED 
    (
    BatchProcessesOperatorID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.BatchProcessesOperator ADD CONSTRAINT
    FK_BatchProcessesOperator_BatchProcesses FOREIGN KEY
    (
    BatchProcessID
    ) REFERENCES dbo.BatchProcesses
    (
    BatchProcessID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.BatchProcessesOperator ADD CONSTRAINT
    FK_BatchProcessesOperator_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

-- Tables to store labor time

CREATE TABLE dbo.LaborTime
    (
    LaborTimeID int NOT NULL IDENTITY (1, 1),
    OrderProcessesOperatorID int NULL,
    BatchProcessesOperatorID int NULL,
    StartTime datetime2(7) NOT NULL,
    EndTime datetime2(7) NULL,
    WorkStatus nvarchar(50) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.LaborTime ADD CONSTRAINT
    PK_LaborTime PRIMARY KEY CLUSTERED 
    (
    LaborTimeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.LaborTime ADD CONSTRAINT
    FK_LaborTime_OrderProcessesOperator FOREIGN KEY
    (
    OrderProcessesOperatorID
    ) REFERENCES dbo.OrderProcessesOperator
    (
    OrderProcessesOperatorID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.LaborTime ADD CONSTRAINT
    FK_LaborTime_BatchProcessesOperator FOREIGN KEY
    (
    BatchProcessesOperatorID
    ) REFERENCES dbo.BatchProcessesOperator
    (
    BatchProcessesOperatorID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

-- Function for retrieving the number of active timers (WO)
CREATE FUNCTION [dbo].[fnGetActiveTimerCount]
    (@orderID int)
RETURNS int
AS
BEGIN
    DECLARE @timerCount int;

    SET @timerCount =
    (
        SELECT COUNT(*)
        FROM OrderProcessesTime
        INNER JOIN OrderProcesses ON OrderProcessesTime.OrderProcessesID = OrderProcesses.OrderProcessesID
        WHERE OrderProcesses.OrderID = @orderID
        AND OrderProcessesTime.EndTime IS NULL
    );

    RETURN @timerCount;
END

GO

-- Function for retrieving the number of active timers (Batch)
CREATE FUNCTION [dbo].[fnGetActiveTimerBatchCount]
    (@batchID int)
RETURNS int
AS
BEGIN
    DECLARE @timerCount int;

    SET @timerCount =
    (
        SELECT COUNT(*)
        FROM BatchProcessesTime
        INNER JOIN BatchProcesses ON BatchProcessesTime.BatchProcessID = BatchProcesses.BatchProcessID
        WHERE BatchProcesses.BatchID = @batchID
        AND BatchProcessesTime.EndTime IS NULL
    );

    RETURN @timerCount;
END

GO

-- Update Get_BatchStatus to include active timer count
ALTER PROCEDURE [dbo].[Get_BatchStatus] 
AS
BEGIN
    SET NOCOUNT ON;
    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID) AS NextDept, 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID) AS CurrentProcess, SUM(BatchOrder.PartQuantity) AS PartCount, COUNT(BatchOrder.BatchID) AS OrderCount, 
                         SUM(Part.SurfaceArea * BatchOrder.PartQuantity) AS TotalSurfaceArea, SUM(ISNULL([Order].Weight, Part.Weight * BatchOrder.PartQuantity)) AS TotalWeight,
              dbo.fnGetActiveTimerBatchCount(Batch.BatchID) AS ActiveTimerCount
FROM            [Order] INNER JOIN
                         BatchOrder ON [Order].OrderID = BatchOrder.OrderID INNER JOIN
                         Part ON [Order].PartID = Part.PartID RIGHT OUTER JOIN
                         Batch ON BatchOrder.BatchID = Batch.BatchID
WHERE        (Batch.Active = 1)
GROUP BY Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID), 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID)

END
GO

-- Permission for editing process/labor time
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('OrderProcess.EditTime', 'Ability to edit time for order processes in order entry.', 'Sales')
GO

-- Permission for Time Data report
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('TimeTrackingReport', 'Ability to run the Time Tracking Report.', 'Reports')
GO

 -- Permission for starting/stopping labor timers for others.
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('TimeTrackingManager', 'Ability to start/stop labor timers for other users.', 'Managers')
GO

--
-- Sync d_Department with ApplicationSettings
--

-- Outside Processing
IF EXISTS (
        SELECT 1
        FROM ApplicationSettings
        WHERE SettingName = 'DepartmentOutsideProcessing'
        )
    UPDATE ApplicationSettings
    SET Value = (
            SELECT TOP 1 DepartmentID
            FROM d_Department
            WHERE SystemName = 'Outside Processing'
            )
    WHERE SettingName = 'DepartmentOutsideProcessing'
GO

-- Part Marking
IF EXISTS (
        SELECT 1
        FROM ApplicationSettings
        WHERE SettingName = 'DepartmentPartMarking'
        )
    UPDATE ApplicationSettings
    SET Value = (
            SELECT TOP 1 DepartmentID
            FROM d_Department
            WHERE SystemName = 'Part Marking'
            )
    WHERE SettingName = 'DepartmentPartMarking'
GO

-- QA
IF EXISTS (
        SELECT 1
        FROM ApplicationSettings
        WHERE SettingName = 'DepartmentQA'
        )
    UPDATE ApplicationSettings
    SET Value = (
            SELECT TOP 1 DepartmentID
            FROM d_Department
            WHERE SystemName = 'QA'
            )
    WHERE SettingName = 'DepartmentQA'
GO

-- Sales
IF EXISTS (
        SELECT 1
        FROM ApplicationSettings
        WHERE SettingName = 'DepartmentSales'
        )
    UPDATE ApplicationSettings
    SET Value = (
            SELECT TOP 1 DepartmentID
            FROM d_Department
            WHERE SystemName = 'Sales'
            )
    WHERE SettingName = 'DepartmentSales'
GO

-- Shipping
IF EXISTS (
        SELECT 1
        FROM ApplicationSettings
        WHERE SettingName = 'DepartmentShipping'
        )
    UPDATE ApplicationSettings
    SET Value = (
            SELECT TOP 1 DepartmentID
            FROM d_Department
            WHERE SystemName = 'Shipping'
            )
    WHERE SettingName = 'DepartmentShipping'
GO

--
-- Invoice by Package
--
INSERT INTO d_InvoiceLevel (InvoiceLevelID)
VALUES ('Package');

GO

---
--- Fix for TFS #7962
--- Try to use the actual ID for Document Administrator group.
---

DECLARE @documentAdministratorSecurityGroupID INT;

SET @documentAdministratorSecurityGroupID = (
        SELECT TOP 1 SecurityGroupID
        FROM SecurityGroup
        WHERE NAME LIKE '%Document Administrator%'
        );

IF (@documentAdministratorSecurityGroupID IS NOT NULL)
BEGIN
    IF NOT EXISTS (SELECT 1 FROM ApplicationSettings WHERE SettingName = 'DocumentAdministratorSecurityGroupId')
        INSERT INTO ApplicationSettings (SettingName, Value)
        VALUES ('DocumentAdministratorSecurityGroupId', @documentAdministratorSecurityGroupID);
    ELSE
        UPDATE ApplicationSettings
        SET Value = @documentAdministratorSecurityGroupID
        WHERE SettingName = 'DocumentAdministratorSecurityGroupId'
END

GO

--
-- Dependent orders
--
ALTER TABLE dbo.[Order] ADD
    PrimaryOrderID int NULL
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
    FK_Order_Order FOREIGN KEY
    (
    PrimaryOrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION;
GO

-- Fix Order Update Trigger
-- Adds new safeguards to ensure that ISNULL(...) does not error on BasePrice.

ALTER TRIGGER [dbo].[Order_Audit_Update] ON [dbo].[Order]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- originally generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([OrderID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  Convert(VARCHAR(50), Inserted.[OrderID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderID],'') <> isnull(Deleted.[OrderID],'')

 IF UPDATE([CustomerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  Convert(VARCHAR(50), Inserted.[CustomerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CustomerID],'') <> isnull(Deleted.[CustomerID],'')

 IF UPDATE([OrderDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderDate]', Convert(VARCHAR(50), Deleted.[OrderDate]),  Convert(VARCHAR(50), Inserted.[OrderDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[OrderDate],'') <> isnull(Deleted.[OrderDate],'')

 IF UPDATE([RequiredDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[RequiredDate]', Convert(VARCHAR(50), Deleted.[RequiredDate]),  Convert(VARCHAR(50), Inserted.[RequiredDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[RequiredDate],'') <> isnull(Deleted.[RequiredDate],'')

 IF UPDATE([Status])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Status]', Convert(VARCHAR(50), Deleted.[Status]),  Convert(VARCHAR(50), Inserted.[Status]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Status],'') <> isnull(Deleted.[Status],'')

 IF UPDATE([CompletedDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CompletedDate]', Convert(VARCHAR(50), Deleted.[CompletedDate]),  Convert(VARCHAR(50), Inserted.[CompletedDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CompletedDate],'') <> isnull(Deleted.[CompletedDate],'')

 IF UPDATE([Priority])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Priority]', Convert(VARCHAR(50), Deleted.[Priority]),  Convert(VARCHAR(50), Inserted.[Priority]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Priority],'') <> isnull(Deleted.[Priority],'')

 IF UPDATE([PurchaseOrder])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PurchaseOrder]', Convert(VARCHAR(50), Deleted.[PurchaseOrder]),  Convert(VARCHAR(50), Inserted.[PurchaseOrder]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PurchaseOrder],'') <> isnull(Deleted.[PurchaseOrder],'')

 IF UPDATE([CreatedBy])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CreatedBy]', Convert(VARCHAR(50), Deleted.[CreatedBy]),  Convert(VARCHAR(50), Inserted.[CreatedBy]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CreatedBy],'') <> isnull(Deleted.[CreatedBy],'')

 IF UPDATE([Invoice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Invoice]', Convert(VARCHAR(50), Deleted.[Invoice]),  Convert(VARCHAR(50), Inserted.[Invoice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[Invoice],'') <> isnull(Deleted.[Invoice],'')

 IF UPDATE([ContractReviewed])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ContractReviewed]', Convert(VARCHAR(50), Deleted.[ContractReviewed]),  Convert(VARCHAR(50), Inserted.[ContractReviewed]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ContractReviewed],'') <> isnull(Deleted.[ContractReviewed],'')

 IF UPDATE([PartID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  Convert(VARCHAR(50), Inserted.[PartID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartID],'') <> isnull(Deleted.[PartID],'')

 IF UPDATE([PartQuantity])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartQuantity]', Convert(VARCHAR(50), Deleted.[PartQuantity]),  Convert(VARCHAR(50), Inserted.[PartQuantity]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PartQuantity],'') <> isnull(Deleted.[PartQuantity],'')

 IF UPDATE([WorkStatus])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[WorkStatus]', Convert(VARCHAR(50), Deleted.[WorkStatus]),  Convert(VARCHAR(50), Inserted.[WorkStatus]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[WorkStatus],'') <> isnull(Deleted.[WorkStatus],'')

 IF UPDATE([CurrentLocation])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CurrentLocation]', Convert(VARCHAR(50), Deleted.[CurrentLocation]),  Convert(VARCHAR(50), Inserted.[CurrentLocation]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[CurrentLocation],'') <> isnull(Deleted.[CurrentLocation],'')

 IF UPDATE([BasePrice])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[BasePrice]', Convert(VARCHAR(50), Deleted.[BasePrice]),  Convert(VARCHAR(50), Inserted.[BasePrice]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(CAST(Inserted.[BasePrice] as varchar),'') <> isnull(CAST(Deleted.[BasePrice] as varchar),'')

 IF UPDATE([PriceUnit])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PriceUnit]', Convert(VARCHAR(50), Deleted.[PriceUnit]),  Convert(VARCHAR(50), Inserted.[PriceUnit]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[PriceUnit],'') <> isnull(Deleted.[PriceUnit],'')

 IF UPDATE([ShippingMethod])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ShippingMethod]', Convert(VARCHAR(50), Deleted.[ShippingMethod]),  Convert(VARCHAR(50), Inserted.[ShippingMethod]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[ShippingMethod],'') <> isnull(Deleted.[ShippingMethod],'')

 IF UPDATE([EstShipDate])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Order', 'u', Convert(VARCHAR(50), Inserted.[OrderID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[EstShipDate]', Convert(VARCHAR(50), Deleted.[EstShipDate]),  Convert(VARCHAR(50), Inserted.[EstShipDate]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderID] = Deleted.[OrderID]
               AND isnull(Inserted.[EstShipDate],'') <> isnull(Deleted.[EstShipDate],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Order_audit_update] trigger', 16, 1 ) with log
 End Catch

 GO

-- Initial Part Quantity
ALTER TABLE [Order]
ADD InitialPartQuantity int NULL;

GO

-- Permission for closing primary orders
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('OrderEntry.OrderClose', 'Ability to close orders in order entry that have dependent orders.', 'Sales')
GO

 --
 -- Permission for editing Blanket PO
 --
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('BlanketPOManager.Edit', 'Ability to edit Blanket POs.', 'Blanket PO')
GO

-- Add permission to any groups with BlanketPOManager
-- Check for groups with BlanketPOManager.Edit - this allows development
-- systems to use the script.
INSERT INTO SecurityGroup_Role (SecurtyGroupID, SecurityRoleID)
SELECT SecurtyGroupID,
    'BlanketPOManager.Edit'
FROM SecurityGroup_Role
WHERE SecurityRoleID = 'BlanketPOManager'
    AND SecurtyGroupID NOT IN (
        SELECT SecurtyGroupID
        FROM SecurityGroup_Role
        WHERE SecurityRoleID = 'BlanketPOManager.Edit'
        )

GO

--
-- Include decimal places in quoted rate
--
ALTER TABLE QuotePartPrice
ALTER COLUMN Rate decimal(12,2) NOT NULL;

GO

--
-- Target price for quotes
--
ALTER TABLE QuotePartPrice
ADD TargetPrice decimal(11, 5) NULL;

GO

--
-- Bulk COC
--
CREATE TABLE dbo.BulkCOC
    (
    BulkCOCID int NOT NULL IDENTITY (1, 1),
    DateCertified date NOT NULL,
    QAUser int NOT NULL,
    ShipmentPackageID int NOT NULL
    )  ON [PRIMARY]

GO
ALTER TABLE dbo.BulkCOC ADD CONSTRAINT
    PK_BulkCOC PRIMARY KEY CLUSTERED 
    (
    BulkCOCID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.BulkCOC ADD CONSTRAINT
    FK_BulkCOC_Users FOREIGN KEY
    (
    QAUser
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 

GO

ALTER TABLE dbo.BulkCOC ADD CONSTRAINT
    FK_BulkCOC_ShipmentPackage FOREIGN KEY
    (
    ShipmentPackageID
    ) REFERENCES dbo.ShipmentPackage
    (
    ShipmentPackageID
    ) ON UPDATE NO ACTION
    ON DELETE CASCADE

GO

CREATE TABLE dbo.BulkCOCOrder
    (
    BulkCOCOrderID int NOT NULL IDENTITY (1, 1),
    BulkCOCID int NOT NULL,
    OrderID int NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.BulkCOCOrder ADD CONSTRAINT
    PK_BulkCOCOrder PRIMARY KEY CLUSTERED
    (
    BulkCOCOrderID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.BulkCOCOrder ADD CONSTRAINT
    FK_BulkCOCOrder_BulkCOC FOREIGN KEY
    (
    BulkCOCID
    ) REFERENCES dbo.BulkCOC
    (
    BulkCOCID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

ALTER TABLE dbo.BulkCOCOrder ADD CONSTRAINT
    FK_BulkCOCOrder_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

--
-- Update Get_IsUserUsed
-- Include operator tables and Bulk COC
--
ALTER PROCEDURE [dbo].[Get_IsUserUsed]
    @userID int,
    @isUsed bit output
AS
BEGIN
    SET NOCOUNT ON;

    SET @isUsed = 1
    
    -- If is in COC
    IF (SELECT COUNT(*) FROM [COC] WHERE COC.QAUser = @userID) > 0
        RETURN 1
        
    -- If is in Order
    IF (SELECT COUNT(*) FROM [Order] WHERE [Order].CreatedBy = @userID) > 0
        RETURN 1

    -- If is in [CustomerCommunication]
    IF (SELECT COUNT(*) FROM [CustomerCommunication] WHERE [CustomerCommunication].UserID = @userID) > 0
        RETURN 1

    -- If is in [OrderReview]
    IF (SELECT COUNT(*) FROM [OrderReview] WHERE [OrderReview].ReviewedBy = @userID) > 0
        RETURN 1

    -- If is in [[OrderShipment]]
    IF (SELECT COUNT(*) FROM [OrderShipment] WHERE [OrderShipment].ShippingUserID = @userID) > 0
        RETURN 1
    
    -- If is in [PartInspection]
    IF (SELECT COUNT(*) FROM [PartInspection] WHERE [PartInspection].QAUserID = @userID) > 0
        RETURN 1

    -- If is in [Quote]
    IF (SELECT COUNT(*) FROM [Quote] WHERE [Quote].UserID = @userID) > 0
        RETURN 1
    
    -- If is in [OrderProcessesOperator]
    IF (SELECT COUNT(*) FROM OrderProcessesOperator WHERE OrderProcessesOperator.UserID = @userID) > 0
        RETURN 1

    -- If is in [BatchProcessesOperator]
    IF (SELECT COUNT(*) FROM BatchProcessesOperator WHERE BatchProcessesOperator.UserID = @userID) > 0
        RETURN 1

    -- If is in [BulkCOC]
    IF (SELECT COUNT(*) FROM BulkCOC WHERE BulkCOC.QAUser = @userID) > 0
        RETURN 1

    SET @isUsed = 0
    RETURN 0
END

GO

--
-- Load Capacity
--

-- Process
ALTER TABLE dbo.Process ADD
    LoadCapacity decimal(18, 8) NULL,
    LoadCapacityType nvarchar(8) NULL,
    LoadCapacityVariance decimal(4, 3) NULL
GO

-- Part Process
ALTER TABLE dbo.PartProcess ADD
    LoadCapacityWeight decimal(18, 8) NULL,
    LoadCapacityQuantity int NULL
GO

-- Order Process
ALTER TABLE dbo.OrderProcesses ADD
    LoadCapacityWeight decimal(18, 8) NULL,
    LoadCapacityQuantity int NULL,
    LoadCapacityVariance decimal(4, 3) NULL
GO

--
-- Quote Part Notes
--
ALTER TABLE dbo.QuotePart ADD
    Notes nvarchar(MAX) NULL
GO

--
-- COC Data Template
--
INSERT INTO [dbo].[Templates] (
    [TemplateID],
    [Template],
    [Description],
    [Tokens]
    )
VALUES (
    'COCData',
    '%PROCESSTEXT%<br/>%ACCEPTEDTEXT%%REJECTEDTEXT%%TOTALTEXT%<br/>',
    'Default text to use for COC data.',
    '%ACCEPTEDTEXT%, %REJECTEDTEXT%, %TOTALTEXT%, %PROCESSTEXT%'
    )

GO
