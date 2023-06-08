-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.3.0'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO



----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Add new settings
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoicePartIemName', '')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceExportTokens', '<CUSTOMERID>,<CUSTOMERNAME>,<INVOICEID>,<WO>,<DATECREATED>,<DATEDUE>,<SHIPDATE>,<TERMS>,<PO>,<ARACCOUNT:400000010770>,<SALESACCOUNT>,<LINEITEM>,<ITEM>,<DESCRIPTION>,<QUANTITY>,<UNIT>,<UNITPRICE>,<EXTPRICE>,<SHIPPING>,<TRACKINGNUMBER>')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceExportType', 'CSV')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceCustomerWOField', 'Customer WO')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceTrackingNumberField', 'Tracking #')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceWorkOrderPrefix', 'WO-')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceMaxBatchExport', '250')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('InvoiceExportLevel', 'WorkOrder')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('ProcessLevelPricing', 'false')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('MinimumProcessPrice', '2')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('QBConnectionString', 'ApplicationName="DWOS"')
GO
INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('QBClass', '')
GO

-- Add new SalesOrder table

CREATE TABLE dbo.SalesOrder
	(
	SalesOrderID int NOT NULL IDENTITY (1, 1),
	OrderID int NOT NULL,
	CustomerID int NOT NULL,
	CustomerWO nvarchar(50) NULL,
	PurchaseOrder nvarchar(50) NULL,
	Invoice nvarchar(50) NULL,
	OrderDate smalldatetime NULL,
	EstShipDate date NULL,
	RequiredDate date NULL,
	CreatedBy int NULL,
	Status nvarchar(50) NULL,
	CompletedDate smalldatetime NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.SalesOrder ADD CONSTRAINT
	PK_SalesOrder PRIMARY KEY CLUSTERED 
	(
	SalesOrderID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.SalesOrder ADD CONSTRAINT
	FK_SalesOrder_Customer FOREIGN KEY
	(
	CustomerID
	) REFERENCES dbo.Customer
	(
	CustomerID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

--------------------------------------------------------------------------------------

ALTER TABLE dbo.SalesOrder ADD CONSTRAINT
	FK_SalesOrder_d_OrderStatus FOREIGN KEY
	(
	Status
	) REFERENCES dbo.d_OrderStatus
	(
	OrderStatusID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Add new column to Order table

ALTER TABLE dbo.[Order] ADD
	SalesOrderID int NULL
GO

ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_SalesOrder FOREIGN KEY
	(
	SalesOrderID
	) REFERENCES dbo.SalesOrder
	(
	SalesOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Add new SalesOrder_Media junction table

CREATE TABLE dbo.SalesOrder_Media
	(
	SalesOrderID int NOT NULL,
	MediaID int NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.SalesOrder_Media ADD CONSTRAINT
	PK_SalesOrder_Media PRIMARY KEY CLUSTERED 
	(
	SalesOrderID,
	MediaID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE dbo.SalesOrder_Media ADD CONSTRAINT
	FK_SalesOrder_Media_SalesOrder FOREIGN KEY
	(
	SalesOrderID
	) REFERENCES dbo.SalesOrder
	(
	SalesOrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE
GO

ALTER TABLE dbo.SalesOrder_Media ADD CONSTRAINT
	FK_SalesOrder_Media_Media FOREIGN KEY
	(
	MediaID
	) REFERENCES dbo.Media
	(
	MediaID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Stored procedure for deleting unused media (added SalesOrder_Media)

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
                                Labels lbl
                              WHERE
                                m.MediaID = lbl.MediaID)
				  AND
				NOT EXISTS (SELECT *
                              FROM
                                LabelType lt
                              WHERE
                                m.MediaID = lt.MediaID)
				  AND
				 NOT EXISTS (SELECT *
                              FROM
                                Receiving_Media rm
                              WHERE
                                m.MediaID = rm.MediaID)
				  AND
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
                                [SalesOrder_Media] so
                              WHERE
                                m.MediaID = so.MediaID)
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

--------------------------------------------------------------------------------------------------------------------------------------------
-- Batch Table

CREATE TABLE [dbo].[Batch](
	[BatchID] [int] IDENTITY(1,1) NOT NULL,
	[Active] [bit] NOT NULL,
	[OpenDate] [date] NOT NULL,
	[CloseDate] [date] NULL,
	[Fixture] [nvarchar](255) NULL,
	[WorkStatus] [nvarchar](50) NULL,
	[CurrentLocation] [nvarchar](50) NULL,
 CONSTRAINT [PK_Batch] PRIMARY KEY CLUSTERED 
(
	[BatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Batch]  WITH CHECK ADD  CONSTRAINT [FK_Batch_d_WorkStatus] FOREIGN KEY([WorkStatus])
REFERENCES [dbo].[d_WorkStatus] ([WorkStatusID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Batch] CHECK CONSTRAINT [FK_Batch_d_WorkStatus]
GO

--------------------------------------------------------------------------------------------------------------------------------------------
-- BatchOrder Table
CREATE TABLE [dbo].[BatchOrder](
	[BatchOrderID] [int] IDENTITY(1,1) NOT NULL,
	[BatchID] [int] NOT NULL,
	[OrderID] [int] NOT NULL,
	[PartQuantity] [int] NOT NULL,
 CONSTRAINT [PK_BatchOrder] PRIMARY KEY CLUSTERED 
(
	[BatchOrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BatchOrder]  WITH CHECK ADD  CONSTRAINT [FK_BatchOrder_Batch] FOREIGN KEY([BatchID])
REFERENCES [dbo].[Batch] ([BatchID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchOrder] CHECK CONSTRAINT [FK_BatchOrder_Batch]
GO

ALTER TABLE [dbo].[BatchOrder]  WITH CHECK ADD  CONSTRAINT [FK_BatchOrder_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchOrder] CHECK CONSTRAINT [FK_BatchOrder_Order]
GO


--------------------------------------------------------------------------------------------------------------------------------------------
-- Batch Process
CREATE TABLE [dbo].[BatchProcesses](
	[BatchProcessID] [int] IDENTITY(1,1) NOT NULL,
	[BatchID] [int] NOT NULL,
	[ProcessID] [int] NOT NULL,
	[StepOrder] [int] NOT NULL,
	[Department] [nvarchar](50) NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
 CONSTRAINT [PK_BatchProcesses] PRIMARY KEY CLUSTERED 
(
	[BatchProcessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[BatchProcesses]  WITH CHECK ADD  CONSTRAINT [FK_BatchProcesses_Batch] FOREIGN KEY([BatchID])
REFERENCES [dbo].[Batch] ([BatchID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[BatchProcesses] CHECK CONSTRAINT [FK_BatchProcesses_Batch]
GO

ALTER TABLE [dbo].[BatchProcesses]  WITH CHECK ADD  CONSTRAINT [FK_BatchProcesses_d_Department] FOREIGN KEY([Department])
REFERENCES [dbo].[d_Department] ([DepartmentID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[BatchProcesses] CHECK CONSTRAINT [FK_BatchProcesses_d_Department]
GO

ALTER TABLE [dbo].[BatchProcesses]  WITH CHECK ADD  CONSTRAINT [FK_BatchProcesses_Process] FOREIGN KEY([ProcessID])
REFERENCES [dbo].[Process] ([ProcessID])
GO

ALTER TABLE [dbo].[BatchProcesses] CHECK CONSTRAINT [FK_BatchProcesses_Process]
GO


--------------------------------------------------------------------------------------------------------------------------------------------

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create FUNCTION [dbo].[fnGetCurrentProcessBatch] 
(
	@batchID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @currentProcessID int
	DECLARE @currentProcessName nvarchar(50) = 'NA'

	-- Find Next Dept that is started but not completed
	SET @currentProcessID = 
	(
		SELECT  TOP(1) ProcessID FROM [BatchProcesses] 
		WHERE StartDate IS NOT NULL AND EndDate IS NULL AND BatchID = @batchID
		ORDER BY StepOrder
	)

	-- If didn't find the process started then show next process not started
	IF (@currentProcessID IS NULL)
		BEGIN
			SET @currentProcessID = 
				(
					SELECT  TOP(1) ProcessID FROM [BatchProcesses] 
					WHERE StartDate IS NULL AND EndDate IS NULL AND BatchID = @batchID
					ORDER BY StepOrder
				)
		END
	
	-- get the name of the process
	IF (@currentProcessID IS NOT NULL)
		BEGIN
			SET @currentProcessName = 
			(
				SELECT Name FROM Process WHERE ProcessID = @currentProcessID
			)
		END
	 
	 
	Return @currentProcessName

END
GO

--------------------------------------------------------------------------------------------------------------------------------------------

CREATE FUNCTION [dbo].[fnGetNextDeptBatch] 
(
	@batchID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @nextDept NVarChar(50)
	

	-- Find Next Dept that is started but not completed
	SET @nextDept = 
	(
		SELECT  TOP(1) Department FROM [BatchProcesses] 
		WHERE StartDate IS NULL AND EndDate IS NULL AND BatchID = @batchID
		ORDER BY StepOrder
	)

	-- IF NOT IN ORDER ANSWERS THEN GET FROM PART PROCESSES ITSELF
	IF (@nextDept IS NULL)
		BEGIN
			SET @nextDept = 
			(
				SELECT  TOP(1) Department FROM [BatchProcesses] 
				WHERE StartDate IS NULL AND BatchID = @batchID
				ORDER BY StepOrder
			)
		END
	 
	 IF (@nextDept IS NULL)
		SET @nextDept = 'None'
	Return @nextDept

END


GO


--------------------------------------------------------------------------------------------------------------------------------------------
ALTER FUNCTION [dbo].[fnGetInBatch]
(
	@orderID int
)
RETURNS bit
AS
BEGIN

DECLARE @inBatch bit = 0
			
IF ((SELECT COUNT(*) FROM Batch INNER JOIN BatchOrder ON Batch.BatchID = BatchOrder.BatchID WHERE (BatchOrder.OrderID = @orderID) AND (Batch.Active = 1)) > 0)
			BEGIN
				SET @inBatch = 1
			END

RETURN @inBatch

END
GO

--------------------------------------------------------------------------------------------------------------------------------------------
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Get_BatchStatus] 
AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID) AS NextDept, 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID) AS CurrentProcess, SUM(BatchOrder.PartQuantity) AS PartCount, COUNT(BatchOrder.BatchID) AS OrderCount, 
                         SUM(Part.SurfaceArea * BatchOrder.PartQuantity) AS TotalSurfaceArea, SUM(Part.Weight * BatchOrder.PartQuantity) AS TotalWeight
FROM            [Order] INNER JOIN
                         BatchOrder ON [Order].OrderID = BatchOrder.OrderID INNER JOIN
                         Part ON [Order].PartID = Part.PartID RIGHT OUTER JOIN
                         Batch ON BatchOrder.BatchID = Batch.BatchID
WHERE        (Batch.Active = 1)
GROUP BY Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID), 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID)

END

GO




--------------------------------------------------------------------------------------
-- Add SalesOrderID to WIP
ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate, [Order].Priority, [Order].WorkStatus, 
                         [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].OrderType, 
                         [Order].PartQuantity, [Order].Hold, dbo.fnGetInBatch([Order].OrderID) AS InBatch, [Order].PartQuantity * Part.SurfaceArea AS SurfaceArea, 
                         dbo.fnGetCurrentProcessRemainingTime([Order].OrderID) AS RemainingTime, dbo.fnGetCurrentProcessDate([Order].OrderID) AS CurrentProcessDue, 
                         [Order].SchedulePriority, dbo.fnGetCurrentProcessPartCount([Order].OrderID) AS PartProcessingCount, [Order].SalesOrderID
FROM            [Order] LEFT OUTER JOIN
                         Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
                         Part ON [Order].PartID = Part.PartID
WHERE        ([Order].Status = N'Open')
					 
END
GO

--------------------------------------------------------------------------------------
-- Add Description column to Part table
ALTER TABLE dbo.Part ADD
	Description nvarchar(MAX) NULL

GO

--------------------------------------------------------------------------------------
-- Add Accounting ID column to Customer table

ALTER TABLE dbo.Customer ADD
	AccountingID nvarchar(50) NULL
GO

--------------------------------------------------------------------------------------
-- Add Invoice Level column to Customer table

ALTER TABLE dbo.Customer ADD
	InvoiceLevelID nvarchar(50) NOT NULL CONSTRAINT DF_Customer_InvoiceLevel DEFAULT N'Default'
GO

--------------------------------------------------------------------------------------
-- Create d_FeeType table
CREATE TABLE dbo.d_FeeType
	(
	FeeTypeID nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.d_FeeType ADD CONSTRAINT
	PK_d_FeeType PRIMARY KEY CLUSTERED 
	(
	FeeTypeID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

--------------------------------------------------------------------------------------
-- Add default values of Fixed and Percentage types
INSERT INTO [dbo].[d_FeeType] ([FeeTypeID]) VALUES ('Fixed')
GO
INSERT INTO [dbo].[d_FeeType] ([FeeTypeID]) VALUES ('Percentage')
GO


--------------------------------------------------------------------------------------
-- OrderFeeType - Price column to decimal
ALTER TABLE dbo.OrderFeeType
ALTER COLUMN Price decimal(10, 2)
GO

--------------------------------------------------------------------------------------
-- OrderFees change column Charge(money)-toDecimal
ALTER TABLE dbo.OrderFees
ALTER COLUMN Charge decimal(10, 2)
GO

ALTER TRIGGER dbo.OrderFees_Modified ON dbo.OrderFees
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

 Begin Try 
 If Trigger_NestLevel(object_ID(N'[dbo].[OrderFees_Modified]')) > 1 Return;
 If (Update(Created) or Update(Modified)) AND Trigger_NestLevel() = 1
 Begin; Raiserror('Update failed.', 16, 1); Rollback;  Return; End;
 -- Update the Modified date
 UPDATE [dbo].[OrderFees]
 SET Modified = getdate(), 
        [RowVersion] = [OrderFees].[RowVersion] + 1 
   FROM [dbo].[OrderFees]
     JOIN Inserted
       ON [OrderFees].[OrderFeeID] = Inserted.[OrderFeeID] End Try 
 Begin Catch 
   Raiserror('error in [dbo].[OrderFees_modified] trigger', 16, 1 ) with log
 End Catch
GO
ALTER TRIGGER dbo.OrderFees_Audit_Insert ON dbo.OrderFees
 AFTER Insert
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, NewValue, RowVersion)
   SELECT  Inserted.Created, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderFees', 'i', Inserted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderFeeID]', Cast(Inserted.[OrderFeeID] as VARCHAR(50)), 1
          FROM Inserted
          WHERE Inserted.[OrderFeeID] is not null

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[OrderFees_audit_insert] trigger', 16, 1 ) with log
 End Catch
GO
ALTER TRIGGER dbo.OrderFees_Audit_Update ON dbo.OrderFees
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([OrderFeeID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderFees', 'u', Convert(VARCHAR(50), Inserted.[OrderFeeID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderFeeID]', Convert(VARCHAR(50), Deleted.[OrderFeeID]),  Convert(VARCHAR(50), Inserted.[OrderFeeID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderFeeID] = Deleted.[OrderFeeID]
               AND isnull(Inserted.[OrderFeeID],'') <> isnull(Deleted.[OrderFeeID],'')

 IF UPDATE([OrderID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderFees', 'u', Convert(VARCHAR(50), Inserted.[OrderFeeID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  Convert(VARCHAR(50), Inserted.[OrderID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderFeeID] = Deleted.[OrderFeeID]
               AND isnull(Inserted.[OrderID],'') <> isnull(Deleted.[OrderID],'')

 IF UPDATE([OrderFeeTypeID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderFees', 'u', Convert(VARCHAR(50), Inserted.[OrderFeeID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderFeeTypeID]', Convert(VARCHAR(50), Deleted.[OrderFeeTypeID]),  Convert(VARCHAR(50), Inserted.[OrderFeeTypeID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderFeeID] = Deleted.[OrderFeeID]
               AND isnull(Inserted.[OrderFeeTypeID],'') <> isnull(Deleted.[OrderFeeTypeID],'')

 IF UPDATE([Charge])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderFees', 'u', Convert(VARCHAR(50), Inserted.[OrderFeeID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Charge]', Convert(VARCHAR(50), Deleted.[Charge]),  Convert(VARCHAR(50), Inserted.[Charge]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderFeeID] = Deleted.[OrderFeeID]
               AND isnull(Inserted.[Charge],'') <> isnull(Deleted.[Charge],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[OrderFees_audit_update] trigger', 16, 1 ) with log
 End Catch
GO
ALTER TRIGGER dbo.OrderFees_Audit_Delete ON dbo.OrderFees
 AFTER Delete
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[OrderFeeID]', Convert(VARCHAR(50), Deleted.[OrderFeeID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[OrderFeeID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[OrderID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[OrderFeeTypeID]', Convert(VARCHAR(50), Deleted.[OrderFeeTypeID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[OrderFeeTypeID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Charge]', Convert(VARCHAR(50), Deleted.[Charge]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Charge] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Created]', Convert(VARCHAR(50), Deleted.[Created]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Created] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Modified]', Convert(VARCHAR(50), Deleted.[Modified]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Modified] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.OrderFees', 'd', deleted.[OrderFeeID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[RowVersion]', Convert(VARCHAR(50), Deleted.[RowVersion]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[RowVersion] is not null

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[OrderFees_audit_delete trigger', 16, 1 ) with log
 End Catch
GO


--------------------------------------------------------------------------------------
-- OrderProcesses add new BatchProcessID

ALTER TABLE dbo.OrderProcesses ADD
	BatchProcessID int NULL
GO


--------------------------------------------------------------------------------------
-- OrderProcesses - Relate BatchProcessID to parent table BatchProcesses

ALTER TABLE dbo.OrderProcesses ADD CONSTRAINT
	FK_OrderProcesses_BatchProcesses FOREIGN KEY
	(
	BatchProcessID
	) REFERENCES dbo.BatchProcesses
	(
	BatchProcessID
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 
	
GO

--------------------------------------------------------------------------------------
-- Add new d_InvoiceLevel table for customer invoicing

CREATE TABLE dbo.d_InvoiceLevel
	(
	InvoiceLevelID nvarchar(50) NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE dbo.d_InvoiceLevel ADD CONSTRAINT
	PK_d_InvoiceLevel PRIMARY KEY CLUSTERED 
	(
	InvoiceLevelID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

INSERT INTO [dbo].[d_InvoiceLevel] ([InvoiceLevelID]) VALUES ('Default')
GO
INSERT INTO [dbo].[d_InvoiceLevel] ([InvoiceLevelID]) VALUES ('SalesOrder')
GO
INSERT INTO [dbo].[d_InvoiceLevel] ([InvoiceLevelID]) VALUES ('WorkOrder')
GO


--------------------------------------------------------------------------------------
-- Add relationship for Customer to d_InvoiceLevel table

ALTER TABLE dbo.Customer ADD CONSTRAINT
	FK_Customer_d_InvoiceLevel FOREIGN KEY
	(
	InvoiceLevelID
	) REFERENCES dbo.d_InvoiceLevel
	(
	InvoiceLevelID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 

GO

--------------------------------------------------------------------------------------
-- Add Price column to Process table

ALTER TABLE dbo.Process ADD
	Price smallmoney NULL
GO

--------------------------------------------------------------------------------------
-- Add LotPrice and EachPrice columns to Part table

ALTER TABLE dbo.Part ADD
	LotPrice smallmoney NULL,
	EachPrice smallmoney NULL
GO

--------------------------------------------------------------------------------------
-- Add PartProcessPrice table
CREATE TABLE [dbo].[PartProcessPrice](
	[PartProcessPriceID] [int] IDENTITY(1,1) NOT NULL,
	[PartProcessID] [int] NOT NULL,
	[MinQty] [int] NOT NULL,
	[MaxQty] [int] NOT NULL,
	[Price] [smallmoney] NOT NULL,
	[PriceUnit] [nvarchar](50) NOT NULL,
	[IndexOrder] [int] NOT NULL,
 CONSTRAINT [PK_PartProcessPrice] PRIMARY KEY CLUSTERED 
(
	[PartProcessPriceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[PartProcessPrice]  WITH CHECK ADD  CONSTRAINT [FK_PartProcessPrice_PartProcess] FOREIGN KEY([PartProcessID])
REFERENCES [dbo].[PartProcess] ([PartProcessID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PartProcessPrice] CHECK CONSTRAINT [FK_PartProcessPrice_PartProcess]
GO

------------------------------------------------------------------------------------------------------------------------------------------------------------------


-- Remove this SP 
DROP PROCEDURE [dbo].[Delete_EmptyBatchLoad]
GO
-- Drop old Order Batch stuff
DROP TABLE [dbo].[OrderBatchItem]
GO
DROP TABLE [dbo].[OrderBatch]
GO




  UPDATE [Part] SET LotPrice = DefaultPrice WHERE PriceUnit = 'Lot'
  GO
  UPDATE [Part] SET EachPrice = DefaultPrice WHERE PriceUnit = 'Each'
  GO
  
 
/****** Object:  Index [IX_Part_CustomerActive]  Script Date: 10/2/2014 10:07:57 PM ******/
DROP INDEX [IX_Part_CustomerActive] ON [dbo].[Part]
GO

/****** Object:  Index [IX_Part_CustomerActive]  Script Date: 10/2/2014 10:07:57 PM ******/
CREATE NONCLUSTERED INDEX [IX_Part_CustomerActive] ON [dbo].[Part]
(
	[CustomerID] ASC,
	[Active] ASC
)
INCLUDE ( 	[PartID],
	[Name],
	[PartMarking]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [Part]  DROP COLUMN DefaultPrice
GO

ALTER TABLE [dbo].[Part] DROP CONSTRAINT [DF_Part_PriceUnit]
GO

ALTER TABLE [dbo].[Part] DROP CONSTRAINT [FK_Part_d_PriceUnit]
GO

ALTER TABLE [Part]  DROP COLUMN PriceUnit
GO

ALTER TRIGGER [dbo].[Part_Audit_Delete] ON [dbo].[Part]
 AFTER Delete
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[PartID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Name]', Convert(VARCHAR(50), Deleted.[Name]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Name] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[CustomerID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Material]', Convert(VARCHAR(50), Deleted.[Material]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Material] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[MaskingRequired]', Convert(VARCHAR(50), Deleted.[MaskingRequired]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[MaskingRequired] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Revision]', Convert(VARCHAR(50), Deleted.[Revision]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Revision] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Notes]', Convert(VARCHAR(50), Deleted.[Notes]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Notes] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[DateAdded]', Convert(VARCHAR(50), Deleted.[DateAdded]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[DateAdded] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[ManufacturerID]', Convert(VARCHAR(50), Deleted.[ManufacturerID]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[ManufacturerID] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Active]', Convert(VARCHAR(50), Deleted.[Active]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Active] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Created]', Convert(VARCHAR(50), Deleted.[Created]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Created] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[Modified]', Convert(VARCHAR(50), Deleted.[Modified]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[Modified] is not null

   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, Rowversion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(),'dbo.Part', 'd', deleted.[PartID],
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Oder Number for an Order Detail Line)
        '[RowVersion]', Convert(VARCHAR(50), Deleted.[RowVersion]),  deleted.Rowversion           FROM deleted
          WHERE deleted.[RowVersion] is not null

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Part_audit_delete trigger', 16, 1 ) with log
 End Catch
 GO

/****** Object:  Trigger [dbo].[Part_Audit_Update]    Script Date: 10/2/2014 10:12:50 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[Part_Audit_Update] ON [dbo].[Part]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Jul 22 2009 10:55PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([PartID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[PartID]', Convert(VARCHAR(50), Deleted.[PartID]),  Convert(VARCHAR(50), Inserted.[PartID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[PartID],'') <> isnull(Deleted.[PartID],'')

 IF UPDATE([Name])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Name]', Convert(VARCHAR(50), Deleted.[Name]),  Convert(VARCHAR(50), Inserted.[Name]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[Name],'') <> isnull(Deleted.[Name],'')

 IF UPDATE([CustomerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CustomerID]', Convert(VARCHAR(50), Deleted.[CustomerID]),  Convert(VARCHAR(50), Inserted.[CustomerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[CustomerID],'') <> isnull(Deleted.[CustomerID],'')

 IF UPDATE([Material])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Material]', Convert(VARCHAR(50), Deleted.[Material]),  Convert(VARCHAR(50), Inserted.[Material]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[Material],'') <> isnull(Deleted.[Material],'')

 IF UPDATE([MaskingRequired])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[MaskingRequired]', Convert(VARCHAR(50), Deleted.[MaskingRequired]),  Convert(VARCHAR(50), Inserted.[MaskingRequired]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[MaskingRequired],'') <> isnull(Deleted.[MaskingRequired],'')

 IF UPDATE([Revision])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Revision]', Convert(VARCHAR(50), Deleted.[Revision]),  Convert(VARCHAR(50), Inserted.[Revision]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[Revision],'') <> isnull(Deleted.[Revision],'')

 IF UPDATE([Notes])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Notes]', Convert(VARCHAR(50), Deleted.[Notes]),  Convert(VARCHAR(50), Inserted.[Notes]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[Notes],'') <> isnull(Deleted.[Notes],'')

 IF UPDATE([DateAdded])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[DateAdded]', Convert(VARCHAR(50), Deleted.[DateAdded]),  Convert(VARCHAR(50), Inserted.[DateAdded]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[DateAdded],'') <> isnull(Deleted.[DateAdded],'')

 IF UPDATE([ManufacturerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ManufacturerID]', Convert(VARCHAR(50), Deleted.[ManufacturerID]),  Convert(VARCHAR(50), Inserted.[ManufacturerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[ManufacturerID],'') <> isnull(Deleted.[ManufacturerID],'')

 IF UPDATE([Active])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.Part', 'u', Convert(VARCHAR(50), Inserted.[PartID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Active]', Convert(VARCHAR(50), Deleted.[Active]),  Convert(VARCHAR(50), Inserted.[Active]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[PartID] = Deleted.[PartID]
               AND isnull(Inserted.[Active],'') <> isnull(Deleted.[Active],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[Part_audit_update] trigger', 16, 1 ) with log
 End Catch
GO







CREATE NONCLUSTERED INDEX [IX_BatchOrder_OrderID] ON [dbo].[BatchOrder]
(
	[OrderID] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BatchOrder_BatchID] ON [dbo].[BatchOrder]
(
	[BatchID] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BatchActive] ON [dbo].[Batch]
(
	[Active] ASC
) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY  = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[SalesOrder] DROP COLUMN [OrderID]
GO


ALTER TABLE dbo.OrderFeeType ADD CONSTRAINT
	FK_OrderFeeType_d_FeeType FOREIGN KEY
	(
	FeeType
	) REFERENCES dbo.d_FeeType
	(
	FeeTypeID
	) ON UPDATE  CASCADE  
	 ON DELETE  CASCADE  
	
GO

-- Add new [ReportFields] table
CREATE TABLE [dbo].[ReportFields](
	[ReportFieldID] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Width] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[ReportName] [nvarchar](50) NOT NULL,
	[IsVisible] [bit] NOT NULL CONSTRAINT [DF_ReportFields_Visible]  DEFAULT ((1)),
	[IsCustom] [bit] NOT NULL CONSTRAINT [DF_ReportFields_IsCustom]  DEFAULT ((0)),
 CONSTRAINT [PK_ReportFields] PRIMARY KEY CLUSTERED 
(
	[ReportFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


------------------------------------------------------------------------------------------------------------------------------------------------
-- Update Order Fees to ensure that no order fee type exists in order fees that does not exist in [OrderFeeType]
-- Was BUG in client that allowed user to change [OrderFeeTypeID] before CASCADE update was set to on

UPDATE [OrderFees]
SET [OrderFees].[OrderFeeTypeID] = (SELECT TOP 1 [OrderFeeType].[OrderFeeTypeID] FROM [OrderFeeType])
WHERE  NOT EXISTS (SELECT [OrderFeeType].[OrderFeeTypeID] FROM [OrderFeeType] WHERE [OrderFeeType].[OrderFeeTypeID] = [OrderFees].[OrderFeeTypeID])
GO
--------------







-- Create function to get total price for an order
CREATE FUNCTION [dbo].[fnGetOrderTotalPrice] 
(
	@orderID int
)
RETURNS smallmoney
AS
BEGIN
	
	DECLARE @result smallmoney
	DECLARE @percentValue smallmoney
	DECLARE @fixedValue smallmoney	

    DECLARE @priceUnit nvarchar(50) = (SELECT [Order].PriceUnit FROM [Order] WHERE [Order].OrderID = @orderID)

	IF (@priceUnit = 'Each')
		BEGIN
			SET @result = (SELECT BasePrice * [Order].PartQuantity FROM [Order] WHERE [Order].OrderID = @orderID)
		END
	ELSE
		BEGIN
			SET @result = (SELECT BasePrice FROM [Order] WHERE [Order].OrderID = @orderID)
		END

	-- Get all fixed cost
	SET @fixedValue = (SELECT   SUM(OrderFees.Charge)
	FROM            OrderFees INNER JOIN
							OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
	WHERE OrderFeeType.FeeType = 'Fixed' AND OrderID = @orderID)


	-- Get all percent cost
	SET @percentValue = (SELECT   SUM((OrderFees.Charge / 100) * @result)
	FROM            OrderFees INNER JOIN
							 OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
	WHERE OrderFeeType.FeeType = 'Percentage' AND OrderID = @orderID AND OrderFees.Charge > 0)

	IF(@percentValue IS NULL)
		SET @percentValue = 0
	IF(@fixedValue IS NULL)
		SET @fixedValue = 0
	
	RETURN  (@result + @fixedValue + @percentValue)

END

GO


--------------------------------


CREATE FUNCTION fnGetOrderFeePrice
(
	@orderID int
)
RETURNS smallmoney
AS
BEGIN

	DECLARE @result smallmoney
	DECLARE @percentValue smallmoney
	DECLARE @fixedValue smallmoney	

	DECLARE @priceUnit nvarchar(50) = (SELECT [Order].PriceUnit FROM [Order] WHERE [Order].OrderID = @orderID)

	IF (@priceUnit = 'Each')
		BEGIN
			SET @result = (SELECT BasePrice * [Order].PartQuantity FROM [Order] WHERE [Order].OrderID = @orderID)
		END
	ELSE
		BEGIN
			SET @result = (SELECT BasePrice FROM [Order] WHERE [Order].OrderID = @orderID)
		END

	-- Get all fixed cost
	SET @fixedValue = (SELECT   SUM(OrderFees.Charge)
	FROM            OrderFees INNER JOIN
							OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
	WHERE OrderFeeType.FeeType = 'Fixed' AND OrderID = @orderID)


	-- Get all percent cost
	SET @percentValue = (SELECT   SUM((OrderFees.Charge / 100) * @result)
	FROM            OrderFees INNER JOIN
							 OrderFeeType ON OrderFees.OrderFeeTypeID = OrderFeeType.OrderFeeTypeID
	WHERE OrderFeeType.FeeType = 'Percentage' AND OrderID = @orderID AND OrderFees.Charge > 0)

	IF(@percentValue IS NULL)
		SET @percentValue = 0
	IF(@fixedValue IS NULL)
		SET @fixedValue = 0

	-- Return the result of the function
	RETURN  (@fixedValue + @percentValue)

END
GO

