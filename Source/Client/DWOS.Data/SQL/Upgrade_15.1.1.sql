-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Update default CRON Expression for Order Reciepts
UPDATE [dbo].[ReportType]
 SET [DefaultSchedule] = '0 */10 * ? * MON-FRI *'
 WHERE ReportTypeID = 3
GO

-- Update CRON Expression for all existing Order Reciepts Tasks
UPDATE [dbo].[ReportTask]
   SET [Schedule] = '0 */10 * ? * MON-FRI *'
   WHERE ReportTypeID = 3
GO

-- Create new plugin table
CREATE TABLE [dbo].[PlugIn](
	[PlugInID] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](500) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[SecurityRole] [nvarchar](50) NULL,
	[Image] [varbinary](max) NULL,
	[FileZip] [varbinary](max) NULL,
	[ExternalExecution] [bit] NOT NULL CONSTRAINT [DF_PlugIn_External]  DEFAULT ((1)),
	[PluginFileName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PlugIn] PRIMARY KEY CLUSTERED 
(
	[PlugInID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- Add Weight to the Order Containers table
ALTER TABLE dbo.OrderContainers ADD
	Weight decimal(9, 2) NULL
GO


-- Table [OrderWorkStatusHistory]
CREATE TABLE [dbo].[OrderWorkStatusHistory](
	[OrderWorkStatusId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[TimeIn] [smalldatetime] NULL,
	[TimeOut] [smalldatetime] NULL,
	[WorkStatus] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OrderWorkStatusHistory] PRIMARY KEY CLUSTERED 
(
	[OrderWorkStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- SP [Update_OrderWorkStatusHistory]
CREATE PROCEDURE [dbo].[Update_OrderWorkStatusHistory] 
	@orderId int, 
	@workStatus nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @orderWorkStatusId int, @timeIn smallDateTime, @timeOut smallDateTime, @currentWorkStatus nvarchar(50)
	
	-- get the last work status entry
    SELECT        TOP (1) @orderWorkStatusId = OrderWorkStatusId, @timeIn = TimeIn, @timeOut = TimeOut, @currentWorkStatus = WorkStatus
	FROM            OrderWorkStatusHistory
	WHERE        (OrderId = @orderId)
	ORDER BY OrderWorkStatusId DESC
	
	--PRINT 'work status ' + @workStatus
	--PRINT 'current work status ' + @currentWorkStatus
	--PRINT 'current @orderWorkStatusIds ' + CAST(@orderWorkStatusId AS nvarchar(50)) 
	--PRINT 'current @timeOut ' + CAST(@timeOut AS nvarchar(50)) 

	-- if current row exists and is not same work status, then update timeout if has not been set
	IF (@orderWorkStatusId > 1 AND @currentWorkStatus <> @workStatus AND @timeOut IS NULL)
		BEGIN
			--PRINT 'updating time out'
			UPDATE [dbo].[OrderWorkStatusHistory] SET [TimeOut] = GETDATE() WHERE OrderWorkStatusId = @orderWorkStatusId
		END

	-- No work status for this order OR current work status does not match
	IF((@orderWorkStatusId IS NULL) OR (@orderWorkStatusId < 1) OR (@currentWorkStatus <> @workStatus)) 
		BEGIN
			INSERT INTO [dbo].[OrderWorkStatusHistory] ([OrderId],[TimeIn],[WorkStatus]) VALUES (@orderId, GETDATE(), @workStatus)
		END
END

GO

-- TRIGGER Order_UpdateWorkStatusHistory
CREATE TRIGGER [dbo].[Order_UpdateWorkStatusHistory] 
   ON  [dbo].[Order] 
   AFTER INSERT, UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	DECLARE @t_ordeId int, @t_workStatus nvarchar(50)
    	
	-- if the work status column was updated or inserted
	IF  (UPDATE (WorkStatus))
		BEGIN
			SELECT @t_workStatus = inserted.WorkStatus, @t_ordeId = inserted.OrderID FROM inserted
		END

	--PRINT 'work status ' + @t_workStatus
	--PRINT 'order id ' + CAST(@t_ordeId as NVARCHAR(50))

	IF (@t_ordeId > 1 AND @t_workStatus IS NOT NULL)
		BEGIN
			--PRINT 'updating status ' + @t_workStatus
			EXEC [dbo].[Update_OrderWorkStatusHistory] @orderId = @t_ordeId, @workStatus = @t_workStatus
		END
END

GO

-- fnGetOrderWorkStatusDuration
CREATE FUNCTION fnGetOrderWorkStatusDuration
(
	@orderId int, @workStatus nvarchar(50)
)
RETURNS int
AS
BEGIN
	DECLARE @timeIn smallDateTime, @timeOut smallDateTime, @currentWorkStatus nvarchar(50), @duration int = 0
	
	-- get the last work status entry
	SELECT        TOP (1) @timeIn = TimeIn, @timeOut = TimeOut, @currentWorkStatus = WorkStatus
	FROM            OrderWorkStatusHistory
	WHERE        (OrderId = @orderId)
	ORDER BY OrderWorkStatusId DESC

	-- if same work status and still going
	IF( @currentWorkStatus = @workStatus) AND (@timeOut IS NULL)
		SET @duration = (DATEDIFF(MINUTE, @timeIn, GETDATE()))
	
	RETURN @duration

END
GO

-- [Get_OrderStatus] - Add WorkStatusDuration
ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate, [Order].Priority, [Order].WorkStatus, 
                         [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].OrderType, 
                         [Order].PartQuantity, [Order].Hold, dbo.fnGetInBatch([Order].OrderID) AS InBatch, [Order].PartQuantity * Part.SurfaceArea AS SurfaceArea, 
                         dbo.fnGetCurrentProcessRemainingTime([Order].OrderID) AS RemainingTime, dbo.fnGetCurrentProcessDate([Order].OrderID) AS CurrentProcessDue, 
                         [Order].SchedulePriority, dbo.fnGetCurrentProcessPartCount([Order].OrderID) AS PartProcessingCount, [Order].SalesOrderID, dbo.fnGetOrderWorkStatusDuration ([Order].OrderID, [Order].WorkStatus) AS WorkStatusDuration
FROM            [Order] LEFT OUTER JOIN
                         Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
                         Part ON [Order].PartID = Part.PartID
WHERE        ([Order].Status = N'Open')
					 
END
