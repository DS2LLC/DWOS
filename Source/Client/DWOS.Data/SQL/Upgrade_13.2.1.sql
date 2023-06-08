-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '13.2.1'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO


-- ************* Delete_EmptyBatchLoad **************
CREATE PROCEDURE Delete_EmptyBatchLoad
AS
BEGIN
	SET NOCOUNT ON;

   DELETE FROM OrderBatch WHERE OrderBatchID IN
	(
		Select ob.OrderBatchID FROM OrderBatch ob
		WHERE 
			-- No Orders using Part
			(SELECT COUNT(*) FROM [OrderBatchItem] obi WHERE obi.OrderBatchID = ob.OrderBatchID) = 0
		AND
			-- Not recently entered
			DATEDIFF(DAY, ob.OpenDate, GETDATE()) > 14
	)
	
	SELECT @@ROWCOUNT
END
GO


-- ************* Delete_LicenseActivationHistory **************


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_LicenseActivationHistory
AS
BEGIN
	
	DELETE FROM LicenseActivationHistory
	WHERE [TimeStamp] < DATEADD("D", -120, GETDATE())
	
	SELECT @@ROWCOUNT AS 'Rows Deleted'
END
GO

-- ************* Delete_UnusedOrderHistory **************

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_UnusedOrderHistory
AS
BEGIN
	
	DELETE FROM OrderHistory WHERE 
		(SELECT Count(*) FROM [Order] o WHERE o.OrderID =  OrderHistory.OrderID) = 0
	
	SELECT @@ROWCOUNT

END
GO


-- ************* Delete_UnusedRecieving **************

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE Delete_UnusedRecieving
AS
BEGIN

	DELETE FROM [Receiving] WHERE 
		(SELECT Count(*) FROM [Order] o WHERE o.OrderID =  [Receiving].OrderID) = 0
	AND
		-- Not recently entered
		DATEDIFF(DAY, [Receiving].Created, GETDATE()) > 30

	SELECT @@ROWCOUNT
END
GO


-- Add Columns
	ALTER TABLE d_InspectionType ADD
		PartInspectionTypeID int NOT NULL IDENTITY (1, 1),
		Name nvarchar(50) NULL
	GO

	-- Move Key Values to Name Column
	UPDATE d_InspectionType SET Name = InspectionTypeID
	GO

	-- Set Name to NOT NULL
	ALTER TABLE d_InspectionType ALTER COLUMN Name nvarchar(50) NOT NULL
	GO



-- UPDATE PartInspection Table
	ALTER TABLE PartInspection ADD PartInspectionTypeID int
	GO

	UPDATE PartInspection SET PartInspectionTypeID = (SELECT it.PartInspectionTypeID  FROM d_InspectionType it WHERE it.InspectionTypeID = PartInspection.InspectionTypeID)
	GO

	-- DROP Old Relation
	ALTER TABLE [dbo].[PartInspection] DROP CONSTRAINT [FK_PartInspection_d_InspectionType]
	GO

	-- Ensure has values then set column to not null
	UPDATE [PartInspection] SET PartInspectionTypeID = 1 WHERE PartInspectionTypeID IS NULL
	ALTER TABLE [PartInspection] ALTER COLUMN PartInspectionTypeID int NOT NULL
	
	ALTER TABLE [PartInspection] DROP COLUMN [ProcessAliasID]

-- UPDATE PartInspectionQuestion Table
	ALTER TABLE PartInspectionQuestion ADD PartInspectionTypeID int
	GO

	UPDATE PartInspectionQuestion SET PartInspectionTypeID = (SELECT it.PartInspectionTypeID  FROM d_InspectionType it WHERE it.InspectionTypeID = PartInspectionQuestion.InspectionTypeID)
	GO

	-- DROP Old Relation
	ALTER TABLE [dbo].[PartInspectionQuestion] DROP CONSTRAINT [FK_PartInspectionQuestion_d_InspectionType]
	GO

-- CREATE ProcessInspections
	CREATE TABLE dbo.ProcessInspections
		(
		ProcessInspectionID int NOT NULL IDENTITY (1, 1),
		PartInspectionTypeID int NOT NULL,
		StepOrder int NOT NULL,
		ProcessID int NOT NULL
		)  ON [PRIMARY]
	GO

	-- ADD Require COC data column
	ALTER TABLE dbo.ProcessInspections ADD COCData bit NOT NULL CONSTRAINT DF_ProcessInspections_COC DEFAULT 0
	GO

	ALTER TABLE dbo.ProcessInspections ADD CONSTRAINT
		PK_ProcessInspections PRIMARY KEY CLUSTERED 
		(
		ProcessInspectionID
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

	GO
	ALTER TABLE dbo.ProcessInspections ADD CONSTRAINT
		FK_ProcessInspections_Process FOREIGN KEY
		( ProcessID	) REFERENCES dbo.Process
		( ProcessID ) 
		ON UPDATE  NO ACTION 
		ON DELETE  CASCADE  
	GO


-- MOVE Inspection FROM ProcessSteps to ProcessInspections
INSERT INTO ProcessInspections(PartInspectionTypeID, StepOrder, ProcessID, COCData)
SELECT        d_InspectionType.PartInspectionTypeID, ProcessSteps.StepOrder, ProcessSteps.ProcessID, ProcessSteps.COCData
FROM            ProcessSteps INNER JOIN
                         d_InspectionType ON ProcessSteps.InspectionTypeID = d_InspectionType.InspectionTypeID
WHERE        (ProcessSteps.InspectionTypeID IS NOT NULL)
ORDER BY ProcessSteps.ProcessID, ProcessSteps.StepOrder
GO

	-- Delete OLD Inspection Process Steps
	DELETE FROM ProcessSteps 
	WHERE RequireQA = 1

	-- DROP Columns
	ALTER TABLE [ProcessSteps] DROP CONSTRAINT [FK_ProcessSteps_d_InspectionType]
	GO

	ALTER TABLE [ProcessSteps] DROP COLUMN InspectionTypeID
	GO

	ALTER TABLE [ProcessSteps] DROP CONSTRAINT DF_ProcessSteps_RequireQA
	GO

	ALTER TABLE [ProcessSteps] DROP COLUMN RequireQA
	GO

-- Drop OLD PK
ALTER TABLE [d_InspectionType] DROP CONSTRAINT [PK_d_InspectionType]
GO

ALTER TABLE [d_InspectionType] DROP COLUMN InspectionTypeID
GO

-- DROP Old Columns

ALTER TABLE PartInspection DROP COLUMN InspectionTypeID
ALTER TABLE PartInspectionQuestion DROP COLUMN InspectionTypeID

-- Add NEW PK
ALTER TABLE dbo.d_InspectionType ADD CONSTRAINT
	PK_d_InspectionType PRIMARY KEY CLUSTERED 
	(
	PartInspectionTypeID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO


	-- ADD PartInspection New Relation
	ALTER TABLE dbo.PartInspection ADD CONSTRAINT
		FK_PartInspection_InspectionType FOREIGN KEY
		( PartInspectionTypeID	) REFERENCES dbo.d_InspectionType
		( PartInspectionTypeID ) 
		ON UPDATE  NO ACTION 
		ON DELETE  CASCADE  
	GO

	-- ADD PartInspectionQuestion New Relation
	ALTER TABLE dbo.PartInspectionQuestion ADD CONSTRAINT
		FK_PartInspectionQuestion_InspectionType FOREIGN KEY
		( PartInspectionTypeID	) REFERENCES dbo.d_InspectionType
		( PartInspectionTypeID ) 
		ON UPDATE  NO ACTION 
		ON DELETE  CASCADE  
	GO

	-- ADD New Relation
	ALTER TABLE dbo.ProcessInspections ADD CONSTRAINT
		FK_ProcessInspections_InspectionType FOREIGN KEY
		( PartInspectionTypeID	) REFERENCES dbo.d_InspectionType
		( PartInspectionTypeID ) 
		ON UPDATE  NO ACTION 
		ON DELETE  CASCADE  
	GO



-- ************************************************************************************************************************
	-- DROP Relations no longer needed
	ALTER TABLE [dbo].[PartInspection] DROP CONSTRAINT FK_PartInspection_Process
	GO

	ALTER TABLE [dbo].[PartInspection] DROP CONSTRAINT FK_PartInspection_ProcessSteps
	GO

	ALTER TABLE PartInspection DROP COLUMN ProcessID
	ALTER TABLE PartInspection DROP COLUMN ProcessStepID
	GO

	EXEC [dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'PartInspection'
	GO

	EXEC [dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'ProcessSteps'
	GO

-- Rename Table
sp_RENAME 'd_InspectionType' , 'PartInspectionType'


-- Ensure Name is Not Null
UPDATE PartInspectionType SET [Name] = 'New Inspection' WHERE [Name] IS NULL
ALTER TABLE PartInspectionType ALTER COLUMN [Name] nvarchar(50) NOT NULL

-- Ensure PartInspectionTypeID is Not Null
UPDATE [PartInspectionQuestion] SET PartInspectionTypeID = 1 WHERE PartInspectionTypeID IS NULL
ALTER TABLE [PartInspectionQuestion] ALTER COLUMN PartInspectionTypeID int NOT NULL

-- Remove extra column not needed
ALTER TABLE OrderProcesses DROP COLUMN [Status]

EXEC [dbo].[pAutoAuditDrop] @SchemaName = N'dbo', @TableName = N'OrderProcesses'
GO

ALTER TABLE dbo.OrderProcessAnswer DROP COLUMN PartProcessID_OLD
GO

-- ************************************************************************************************************************

-- Add [d_OrderType]
CREATE TABLE [dbo].[d_OrderType](
	[OrderTypeID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_OrderType] PRIMARY KEY CLUSTERED 
(
	[OrderTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Add [d_OrderType] default values
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (1, 'Normal')
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (3, 'ReworkExt')
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (4, 'ReworkInt')
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (5, 'ReworkHold')
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (6, 'Lost')
INSERT INTO [d_OrderType] ([OrderTypeID],[Name]) VALUES  (7, 'Quarantine')
GO



-- Add OrderType to Order
ALTER TABLE dbo.[Order] ADD OrderType int NOT NULL CONSTRAINT DF_Order_OrderType DEFAULT 1
GO

ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_d_OrderType FOREIGN KEY
	(OrderType) REFERENCES dbo.d_OrderType
	(OrderTypeID) ON UPDATE  CASCADE ON DELETE  NO ACTION 
GO

-- Move to use new OrderType
UPDATE [Order] SET OrderType = 3 WHERE IsRework = 1
GO

ALTER TABLE [Order] DROP CONSTRAINT DF_Order_IsRework
GO
ALTER TABLE [Order] DROP COLUMN IsRework
GO
ALTER TABLE dbo.[Order] ADD
	Hold bit NOT NULL CONSTRAINT DF_Order_Hold DEFAULT 0
GO

-- Update Stored Procedure
ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN
	SET NOCOUNT ON;

SELECT        [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate, [Order].Priority, [Order].WorkStatus, 
                         [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].OrderType, 
                         [Order].PartQuantity, [Order].Hold
FROM            [Order] LEFT OUTER JOIN
                         Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
                         Part ON [Order].PartID = Part.PartID
WHERE        ([Order].Status = N'Open')
					 
END
GO



INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('OrderEntry.Hold'
           ,'Ability to place orders on hold.'
           ,'Sales')
GO



ALTER TABLE dbo.OrderProcesses ADD
	COCData bit NOT NULL CONSTRAINT DF_OrderProcesses_COCData DEFAULT 1,
	OrderProcessType int NOT NULL CONSTRAINT DF_OrderProcesses_OrderProcessType DEFAULT 1

GO





-- ************************************************************************************************************************

CREATE TABLE [dbo].[d_HoldReason](
	[HoldReasonID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_HoldReason] PRIMARY KEY CLUSTERED 
(
	[HoldReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

  INSERT INTO [dbo].[d_HoldReason] ([Name]) VALUES ('Requirements not clear')
  INSERT INTO [dbo].[d_HoldReason] ([Name]) VALUES ('Out of stock')
  INSERT INTO [dbo].[d_HoldReason] ([Name]) VALUES ('PO discrepancies')
  INSERT INTO [dbo].[d_HoldReason] ([Name]) VALUES ('Missing PO')
  INSERT INTO [dbo].[d_HoldReason] ([Name]) VALUES ('Waiting on customer approval')
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OrderHold](
	[OrderHoldID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[HoldReasonID] [int] NOT NULL,
	[TimeIn] [smalldatetime] NOT NULL,
	[TimeInUser] [int] NOT NULL,
	[TimeOut] [smalldatetime] NULL,
	[TimeOutUser] [int] NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_OrderHold] PRIMARY KEY CLUSTERED 
(
	[OrderHoldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OrderHold]  WITH CHECK ADD  CONSTRAINT [FK_OrderHold_Order] FOREIGN KEY([OrderID])
REFERENCES [dbo].[Order] ([OrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[OrderHold] CHECK CONSTRAINT [FK_OrderHold_Order]
GO

/****** Object:  Trigger [dbo].[OrderProcessAnswer_Audit_Update]    Script Date: 6/24/2013 4:02:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[OrderProcessAnswer_Audit_Update] ON [dbo].[OrderProcessAnswer]
 AFTER Update
 NOT FOR REPLICATION AS
 SET NoCount On 
 -- generated by AutoAudit on Aug 30 2009  7:59PM
 -- created by Paul Nielsen 
 -- www.SQLServerBible.com 

DECLARE @AuditTime DATETIME
SET @AuditTime = GetDate()

 Begin Try 
 IF UPDATE([OrderProcesserAnswerID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderProcesserAnswerID]', Convert(VARCHAR(50), Deleted.[OrderProcesserAnswerID]),  Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[OrderProcesserAnswerID],'') <> isnull(Deleted.[OrderProcesserAnswerID],'')

 IF UPDATE([ProcessQuestionID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[ProcessQuestionID]', Convert(VARCHAR(50), Deleted.[ProcessQuestionID]),  Convert(VARCHAR(50), Inserted.[ProcessQuestionID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[ProcessQuestionID],'') <> isnull(Deleted.[ProcessQuestionID],'')

 IF UPDATE([OrderProcessesID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderProcessesID]', Convert(VARCHAR(50), Deleted.[OrderProcessesID]),  Convert(VARCHAR(50), Inserted.[OrderProcessesID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[OrderProcessesID],'') <> isnull(Deleted.[OrderProcessesID],'')

 IF UPDATE([OrderID])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[OrderID]', Convert(VARCHAR(50), Deleted.[OrderID]),  Convert(VARCHAR(50), Inserted.[OrderID]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[OrderID],'') <> isnull(Deleted.[OrderID],'')

 IF UPDATE([Answer])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Answer]', Convert(VARCHAR(50), Deleted.[Answer]),  Convert(VARCHAR(50), Inserted.[Answer]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[Answer],'') <> isnull(Deleted.[Answer],'')

 IF UPDATE([Completed])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[Completed]', Convert(VARCHAR(50), Deleted.[Completed]),  Convert(VARCHAR(50), Inserted.[Completed]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[Completed],'') <> isnull(Deleted.[Completed],'')

 IF UPDATE([CompletedBy])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CompletedBy]', Convert(VARCHAR(50), Deleted.[CompletedBy]),  Convert(VARCHAR(50), Inserted.[CompletedBy]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[CompletedBy],'') <> isnull(Deleted.[CompletedBy],'')

 IF UPDATE([CompletedData])
   INSERT dbo.Audit (AuditDate, SysUser, Application, HostName, TableName, Operation, PrimaryKey, RowDescription, SecondaryRow, ColumnName, OldValue, NewValue, RowVersion)
   SELECT  @AuditTime, suser_sname(), APP_NAME(), Host_Name(), 'dbo.OrderProcessAnswer', 'u', Convert(VARCHAR(50), Inserted.[OrderProcesserAnswerID]),
        NULL,     -- Row Description (e.g. Order Number)
        NULL,     -- Secondary Row Value (e.g. Order Number for an Order Detail Line)
        '[CompletedData]', Convert(VARCHAR(50), Deleted.[CompletedData]),  Convert(VARCHAR(50), Inserted.[CompletedData]),
 DELETED.Rowversion + 1
          FROM Inserted
             JOIN Deleted
               ON Inserted.[OrderProcesserAnswerID] = Deleted.[OrderProcesserAnswerID]
               AND isnull(Inserted.[CompletedData],'') <> isnull(Deleted.[CompletedData],'')

 End Try 
 Begin Catch 
   Raiserror('error in [dbo].[OrderProcessAnswer_audit_update] trigger', 16, 1 ) with log
 End Catch 
  GO
 
 DROP TRIGGER OrderProcessAnswer_Audit_Delete
 GO
 -- ************************************************************************************************************************

CREATE TABLE [dbo].[d_ReworkReason](
	[ReworkReasonID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ReworkCategory] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_ReworkReason] PRIMARY KEY CLUSTERED 
(
	[ReworkReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[d_ReworkReason] ADD  CONSTRAINT [DF_d_ReworkReason_ReworkCategory]  DEFAULT (N'Rework') FOR [ReworkCategory]
GO


INSERT INTO [dbo].[d_ReworkReason]([Name]) VALUES ('Other')
GO



CREATE TABLE [dbo].[d_HoldLocation](
	[HoldLocation] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_HoldLocation] PRIMARY KEY CLUSTERED 
(
	[HoldLocation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [dbo].[d_HoldLocation] ([HoldLocation]) VALUES ('Chem-Process Hold')
GO

INSERT INTO [dbo].[d_WorkStatus] ([WorkStatusID]) VALUES ('Hold')
INSERT INTO [dbo].[d_WorkStatus] ([WorkStatusID]) VALUES ('Pending Join')
GO


/****** Object:  Table [dbo].[InternalRework]    Script Date: 6/24/2013 8:59:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[InternalRework](
	[InternalReworkID] [int] IDENTITY(1,1) NOT NULL,
	[OriginalOrderID] [int] NOT NULL,
	[ReworkOrderID] [int] NULL,
	[ReworkType] [nvarchar](50) NOT NULL,
	[HoldLocationID] [nvarchar](50) NULL,
	[ReworkReasonID] [int] NULL,
	[Active] [bit] NOT NULL,
	[Notes] [nvarchar](255) NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
 CONSTRAINT [PK_InternalRework] PRIMARY KEY CLUSTERED 
(
	[InternalReworkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InternalRework] ADD  CONSTRAINT [DF_InternalRework_Active]  DEFAULT ((1)) FOR [Active]
GO

ALTER TABLE [dbo].[InternalRework] ADD  CONSTRAINT [DF_InternalRework_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

ALTER TABLE [dbo].[InternalRework] ADD  CONSTRAINT [DF_InternalRework_CreatedBy]  DEFAULT ((1)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[InternalRework]  WITH CHECK ADD  CONSTRAINT [FK_InternalRework_d_HoldLocation] FOREIGN KEY([HoldLocationID])
REFERENCES [dbo].[d_HoldLocation] ([HoldLocation])
ON UPDATE CASCADE
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[InternalRework] CHECK CONSTRAINT [FK_InternalRework_d_HoldLocation]
GO

ALTER TABLE [dbo].[InternalRework]  WITH CHECK ADD  CONSTRAINT [FK_InternalRework_d_ReworkReason] FOREIGN KEY([ReworkReasonID])
REFERENCES [dbo].[d_ReworkReason] ([ReworkReasonID])
ON DELETE SET NULL
GO

ALTER TABLE [dbo].[InternalRework] CHECK CONSTRAINT [FK_InternalRework_d_ReworkReason]
GO

ALTER TABLE [dbo].[InternalRework]  WITH CHECK ADD  CONSTRAINT [FK_InternalRework_Order] FOREIGN KEY([OriginalOrderID])
REFERENCES [dbo].[Order] ([OrderID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[InternalRework] CHECK CONSTRAINT [FK_InternalRework_Order]
GO

ALTER TABLE dbo.d_Contact ADD Active bit NOT NULL CONSTRAINT DF_d_Contact_Active DEFAULT 1
GO

INSERT INTO [dbo].[d_WorkStatus] ([WorkStatusID]) VALUES ('Pending Rework Planning')
GO

-- ************************************************************************************************************************
CREATE TABLE [dbo].[d_OrderChangeType](
	[OrderChangeType] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_OrderChangeType] PRIMARY KEY CLUSTERED 
(
	[OrderChangeType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[d_OrderChangeType] ([OrderChangeType],[Name]) VALUES (1,'External Rework')
GO

INSERT INTO [dbo].[d_OrderChangeType] ([OrderChangeType],[Name]) VALUES (2,'Split')
GO


CREATE TABLE [dbo].[d_OrderChangeReason](
	[OrderChangeReasonID] [int] IDENTITY(1,1) NOT NULL,
	[ChangeType] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_OrderChangeReason] PRIMARY KEY CLUSTERED 
(
	[OrderChangeReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[OrderChange](
	[OrderChangeID] [int] IDENTITY(1,1) NOT NULL,
	[ChangeType] [int] NOT NULL,
	[ReasonCode] [int] NULL,
	[ParentOrderID] [int] NOT NULL,
	[ChildOrderID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[DateCreated] [smalldatetime] NOT NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_OrderChange] PRIMARY KEY CLUSTERED 
(
	[OrderChangeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Index [FK_ParentOrderID]    Script Date: 7/5/2013 2:59:54 PM ******/
CREATE NONCLUSTERED INDEX [FK_IX_ParentOrderID] ON [dbo].[OrderChange]
(
	[ParentOrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

/****** Object:  Index [FK_IX_ChildOrderID]    Script Date: 7/5/2013 3:00:08 PM ******/
CREATE NONCLUSTERED INDEX [FK_IX_ChildOrderID] ON [dbo].[OrderChange]
(
	[ChildOrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrderChange]  WITH CHECK ADD  CONSTRAINT [FK_OrderChange_d_OrderChangeType] FOREIGN KEY([ChangeType])
REFERENCES [dbo].[d_OrderChangeType] ([OrderChangeType])
GO

ALTER TABLE [dbo].[OrderChange] CHECK CONSTRAINT [FK_OrderChange_d_OrderChangeType]
GO

-- Move from Order Rework
INSERT INTO [dbo].[OrderChange]
           ([ChangeType]
           ,[ReasonCode]
           ,[ParentOrderID]
           ,[ChildOrderID]
           ,[UserID]
           ,[DateCreated]
           ,[Notes])
    SELECT 1, 1, [OriginalOrderID], [ReworkedOrderID], 1, GetDate(), SUBSTRING([Reason], 0, 254)
	FROM [dbo].[OrderRework]

GO

DROP TABLE [dbo].[OrderRework]
GO

-- Add Height to Part
ALTER TABLE dbo.[Part] ADD Height float NOT NULL DEFAULT 0
GO

-- Add ShapeType to Part
ALTER TABLE dbo.[Part] ADD ShapeType nvarchar(255)
GO

-- Add Height to QuotePart
ALTER TABLE dbo.[QuotePart] ADD Height float DEFAULT 0
GO

-- Add ShapeType to QuotePart
ALTER TABLE dbo.[QuotePart] ADD ShapeType nvarchar(255)
GO


-- ************************************************************************************************************************

ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN
	SET NOCOUNT ON;

SELECT        [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate, [Order].Priority, [Order].WorkStatus, 
                         [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].OrderType, 
                         [Order].PartQuantity, [Order].Hold, dbo.fnGetInBatch([Order].OrderID) AS InBatch
FROM            [Order] LEFT OUTER JOIN
                         Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
                         Part ON [Order].PartID = Part.PartID
WHERE        ([Order].Status = N'Open')
					 
END
GO

CREATE FUNCTION fnGetInBatch
(
	@orderID int
)
RETURNS bit
AS
BEGIN

DECLARE @inBatch bit = 0
			
IF (SELECT WorkStatus from [order] WHERE OrderID = @orderID) = 'In Process' AND  EXISTS(SELECT        1
			FROM            OrderBatchItem INNER JOIN
									 OrderBatch ON OrderBatchItem.OrderBatchID = OrderBatch.OrderBatchID INNER JOIN
									 OrderProcesses ON OrderBatchItem.OrderProcessID = OrderProcesses.OrderProcessesID
			WHERE        (OrderBatch.Active = 1) AND (OrderProcesses.OrderID = @orderID))
			
			BEGIN
				SET @inBatch = 1
			END

RETURN @inBatch

END
GO

-- ************************************************************************************************************************

/****** Object:  StoredProcedure [dbo].[MANAGE_DefragIndexes]    Script Date: 7/11/2013 4:54:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
-- =============================================
-- This stored procedure checks all indexes in the current
-- database and performs either offline or online defragmentation
-- according to the specified thresholds.
-- The stored procedure also updates statistics for indexes in which the last update
-- time is older than the specified threshold.
-- Parameters:
--	@onlineDefragThreshold specifies minimum percentage of fragmentation 
--	to perform online defragmentation (default 10%).
--	@offlineDefragThreshold specifies minimum percentage of fragmentation 
--	to perform offline defragmentation (default 30%).
--	@updateStatsThreshold specifies the number of days since the last statistics update
--	which should trigger updating statistics (default 7 days).
-- =============================================
ALTER PROCEDURE [dbo].[MANAGE_DefragIndexes] 
(
	@databaseName sysname = null,
	@onlineDefragThreshold float = 10.0,
	@offlineDefragThreshold float = 30.0,
	@updateStatsThreshold int = 7
)
	
AS
BEGIN
 
IF @databasename is null
BEGIN
	RETURN;
END
 
DECLARE @SQL nvarchar(max)
SET @SQL = 'USE '+ @databasename +'
 
	set nocount on
	DECLARE @objectid int
	DECLARE @indexid int
	DECLARE @frag float
	DECLARE @command varchar(8000)
	DECLARE @schemaname sysname
	DECLARE @objectname sysname
	DECLARE @indexname sysname
 
	declare @AllIndexes table (objectid int, indexid int, fragmentation float)
 
	declare @currentDdbId int
	select @currentDdbId = DB_ID()
	
	insert into @AllIndexes
	SELECT 
		object_id, index_id, avg_fragmentation_in_percent 
	FROM sys.dm_db_index_physical_stats (@currentDdbId, NULL, NULL , NULL, ''LIMITED'')
	WHERE index_id > 0
 
	DECLARE indexesToDefrag CURSOR FOR SELECT * FROM @AllIndexes
 
	OPEN indexesToDefrag;
 
	-- Loop through the partitions.
	FETCH NEXT
	   FROM indexesToDefrag
	   INTO @objectid, @indexid, @frag;
 
	WHILE @@FETCH_STATUS = 0
		BEGIN
 
		SELECT @schemaname = s.name
		FROM sys.objects AS o
		JOIN sys.schemas as s ON s.schema_id = o.schema_id
		WHERE o.object_id = @objectid
 
		SELECT @indexname = name 
		FROM sys.indexes
		WHERE  object_id = @objectid AND index_id = @indexid
 
		IF @frag > @onlineDefragThreshold
		BEGIN 
			IF @frag < @offlineDefragThreshold
				BEGIN;
					SELECT @command = ''ALTER INDEX '' + @indexname + '' ON '' + 
							@schemaname + ''.['' + object_name(@objectid) + 
							''] REORGANIZE''
					EXEC (@command)
				END
 
			IF @frag >= @offlineDefragThreshold
				BEGIN;
					SELECT @command = ''ALTER INDEX '' + 
							@indexname +'' ON '' + @schemaname + ''.['' + 
							object_name(@objectid) + ''] REBUILD''
					EXEC (@command)
				END;
			PRINT ''Executed '' + @command
		END
 
		IF STATS_DATE(@objectid, @indexid) < DATEADD(dd, -@updateStatsThreshold, getdate())
		BEGIN
			SELECT @command = ''UPDATE STATISTICS '' + @schemaname + ''.['' + object_name(@objectid) + 
					''] '' + @indexname +'' WITH RESAMPLE''
			EXEC (@command)
 
			PRINT ''Executed '' + @command
		END
 
		FETCH NEXT FROM indexesToDefrag INTO @objectid, @indexid, @frag
 
	END
 
	CLOSE indexesToDefrag;
	DEALLOCATE indexesToDefrag;'
 
DECLARE @Params nvarchar(max)
SET @Params = N'
	@onlineDefragThreshold float,
	@offlineDefragThreshold float,
	@updateStatsThreshold int'
 
EXECUTE sp_executesql @SQL, 
		@Params,
		@onlineDefragThreshold=@onlineDefragThreshold,
		@offlineDefragThreshold=@offlineDefragThreshold,
		@updateStatsThreshold=@updateStatsThreshold;
END

GO



ALTER PROCEDURE [dbo].[MANAGE_ShrinkDatabase]
	@DatabaseName sysname = null,
	@FreeSpace int = 0
AS
BEGIN
	SET NOCOUNT ON;

   EXEC ('DBCC SHRINKDATABASE('''+@DatabaseName+''', '+@FreeSpace+')')
END
GO


CREATE NONCLUSTERED INDEX IX_OrderBatchItem_ID
ON [dbo].[OrderBatchItem] ([OrderProcessID])
INCLUDE ([OrderBatchID])
GO

CREATE NONCLUSTERED INDEX IX_Part_CustomerActive
ON [dbo].[Part] ([CustomerID],[Active])
INCLUDE ([PartID],[Name],[DefaultPrice],[PriceUnit],[PartMarking])
GO





-- ************************************************************************************************************************
-- After RC 13.2.1.X release

ALTER TABLE dbo.InternalRework ADD
	ProcessAliasID int NULL
GO


INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('InternalRework.Assessment'
           ,'Ability to do an internal rework assessment on an order.'
           ,'Quality')
GO

INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('InternalRework.Planning'
           ,'Ability to do rework planning on an order.'
           ,'Quality')
GO

INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('InternalRework.Join'
           ,'Ability to join an internal rework back with its parent order.'
           ,'Quality')
GO

DELETE FROM [SecurityRole] WHERE [SecurityRoleID] = 'InternalRework'
GO

-- [d_ProcessCategory]

CREATE TABLE [dbo].[d_ProcessCategory](
	[ProcessCategory] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_d_ProcessCategory] PRIMARY KEY CLUSTERED 
(
	[ProcessCategory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO [d_ProcessCategory] VALUES ('Default')
GO

-- [ApplicationAnalytics]

CREATE TABLE [dbo].[ApplicationAnalytics](
	[ToolName] [nvarchar](50) NOT NULL,
	[Day] [date] NOT NULL,
	[UsageCount] [int] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApplicationAnalytics] ADD  CONSTRAINT [DF_ApplicationAnalytics_UsageCount]  DEFAULT ((0)) FOR [UsageCount]
GO

-- Update process table with new column
GO
ALTER TABLE dbo.Process ADD
	Category nvarchar(50) NOT NULL CONSTRAINT DF_Process_Category DEFAULT N'Default'
GO
ALTER TABLE dbo.Process ADD CONSTRAINT
	FK_Process_d_ProcessCategory FOREIGN KEY
	(
	Category
	) REFERENCES dbo.d_ProcessCategory
	(
	ProcessCategory
	) ON UPDATE  CASCADE 
	 ON DELETE  NO ACTION 
	
GO


-- ************************************************************************************************************************
-- After RC 13.2.1.3 release 09/09/2013
--
--
-- ************************************************************************************************************************


/****** Object:  UserDefinedFunction [dbo].[fn_WorkDays]    Script Date: 9/10/2013 6:23:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


  CREATE FUNCTION [dbo].[fn_WorkDays]
--Presets
--Define the input parameters (OK if reversed by mistake).
(
    @StartDate DATETIME,
    @EndDate   DATETIME = NULL --@EndDate replaced by @StartDate when DEFAULTed
)

--Define the output data type.
RETURNS INT

AS
--Calculate the RETURN of the function.
BEGIN
    --Declare local variables
    --Temporarily holds @EndDate during date reversal.
    DECLARE @Swap DATETIME

    --If the Start Date is null, return a NULL and exit.
    IF @StartDate IS NULL
        RETURN NULL

    --If the End Date is null, populate with Start Date value so will have two dates (required by DATEDIFF below).
     IF @EndDate IS NULL
        SELECT @EndDate = @StartDate

    --Strip the time element from both dates (just to be safe) by converting to whole days and back to a date.
    --Usually faster than CONVERT.
    --0 is a date (01/01/1900 00:00:00.000)
     SELECT @StartDate = DATEADD(dd,DATEDIFF(dd,0,@StartDate), 0),
            @EndDate   = DATEADD(dd,DATEDIFF(dd,0,@EndDate)  , 0)

    --If the inputs are in the wrong order, reverse them.
     IF @StartDate > @EndDate
        SELECT @Swap      = @EndDate,
               @EndDate   = @StartDate,
               @StartDate = @Swap

    --Calculate and return the number of workdays using the input parameters.
    --This is the meat of the function.
    --This is really just one formula with a couple of parts that are listed on separate lines for documentation purposes.
     RETURN (SELECT CASE WHEN @Swap IS NULL THEN 1 ELSE -1 END) * (
        SELECT
        --Start with total number of days including weekends
        (DATEDIFF(dd,@StartDate, @EndDate)+1)
        --Subtact 2 days for each full weekend
        -(DATEDIFF(wk,@StartDate, @EndDate)*2)
        --If StartDate is a Sunday, Subtract 1
        -(CASE WHEN DATENAME(dw, @StartDate) = 'Sunday'
            THEN 1
            ELSE 0
        END)
        --If EndDate is a Saturday, Subtract 1
        -(CASE WHEN DATENAME(dw, @EndDate) = 'Saturday'
            THEN 1
            ELSE 0
        END)
        )
    END

GO