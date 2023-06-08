-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.2.0'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Update Workschedule TO Department relation to cascade delete
ALTER TABLE dbo.WorkSchedule
	DROP CONSTRAINT FK_WorkSchedule_d_Department
GO

ALTER TABLE dbo.WorkSchedule ADD CONSTRAINT
	FK_WorkSchedule_d_Department FOREIGN KEY
	(
	DepartmentID
	) REFERENCES dbo.d_Department
	(
	DepartmentID
	) ON UPDATE  CASCADE 
	 ON DELETE  CASCADE 
	
GO

-- Remove Department AUDIT Info
EXEC	[dbo].[pAutoAuditDrop]
		@SchemaName = N'dbo',
		@TableName = N'd_Department'

GO

-- Update process category to include lead time
ALTER TABLE dbo.d_ProcessCategory ADD
	LeadTime numeric(3, 1) NULL
GO


-- Update Order Status - Strip Time off ship date, Add surface area, Add est time remaining, Add order process time
ALTER PROCEDURE [dbo].[Get_OrderStatus] 
AS
BEGIN

	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	SELECT        [Order].OrderID AS WO, [Order].PurchaseOrder AS PO, Customer.Name AS Customer, Part.Name AS Part, [Order].EstShipDate as EstShipDate, [Order].Priority, [Order].WorkStatus, 
							 [Order].CurrentLocation, dbo.fnGetNextDept([Order].OrderID) AS NextDept, dbo.fnGetCurrentProcess([Order].OrderID) AS CurrentProcess, [Order].OrderType, 
							 [Order].PartQuantity, [Order].Hold, dbo.fnGetInBatch([Order].OrderID) AS InBatch, [Order].PartQuantity * Part.SurfaceArea AS SurfaceArea, dbo.[fnGetCurrentProcessRemainingTime]([Order].OrderID) AS RemainingTime, dbo.[fnGetCurrentProcessDate]([Order].OrderID) AS CurrentProcessDue, [Order].SchedulePriority
	FROM            [Order] LEFT OUTER JOIN
							 Customer ON [Order].CustomerID = Customer.CustomerID LEFT OUTER JOIN
							 Part ON [Order].PartID = Part.PartID
	WHERE        ([Order].Status = N'Open')
					 			 
END
GO

-- Update ORDER table to change some datetimes to just date
ALTER TABLE [Order] ALTER COLUMN RequiredDate date
ALTER TABLE [Order] ALTER COLUMN EstShipDate date
ALTER TABLE [Order] ALTER COLUMN InitialEstShipDate date
GO

-- Create new function to get order process date
CREATE FUNCTION [dbo].[fnGetCurrentProcessDate] 
(
	@orderID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @dueDate Date
	
	-- Find Next Dept that is started but not completed
	SET @dueDate = 
	(
		SELECT  TOP(1) EstEndDate FROM [OrderProcesses] 
		WHERE EndDate IS NULL AND OrderID = @orderID
		ORDER BY StepOrder
	)
		
	Return @dueDate

END
GO

-- Add new receiving to media table
CREATE TABLE [dbo].[Receiving_Media](
	[ReceivingID] [int] NOT NULL,
	[MediaID] [int] NOT NULL,
 CONSTRAINT [PK_Receiving_Media] PRIMARY KEY CLUSTERED 
(
	[ReceivingID] ASC,
	[MediaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Receiving_Media]  WITH CHECK ADD  CONSTRAINT [FK_Receiving_Media_Media] FOREIGN KEY([MediaID])
REFERENCES [dbo].[Media] ([MediaID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Receiving_Media] CHECK CONSTRAINT [FK_Receiving_Media_Media]
GO

ALTER TABLE [dbo].[Receiving_Media]  WITH CHECK ADD  CONSTRAINT [FK_Receiving_Media_Receiving] FOREIGN KEY([ReceivingID])
REFERENCES [dbo].[Receiving] ([ReceivingID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Receiving_Media] CHECK CONSTRAINT [FK_Receiving_Media_Receiving]
GO



-- Update Delete Unused Media
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


CREATE TABLE [dbo].[ProcessStepCondition](
	[ProcessStepConditionId] [int] IDENTITY(1,1) NOT NULL,
	[ProcessStepId] [int] NOT NULL,
	[InputType] [nvarchar](50) NOT NULL,
	[ProcessQuestionId] [int] NULL,
	[Operator] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[StepOrder] [int] NOT NULL,
 CONSTRAINT [PK_ProcessStepCondition] PRIMARY KEY CLUSTERED 
(
	[ProcessStepConditionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProcessStepCondition]  WITH CHECK ADD  CONSTRAINT [FK_ProcessStepCondition_ProcessSteps] FOREIGN KEY([ProcessStepId])
REFERENCES [dbo].[ProcessSteps] ([ProcessStepID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProcessStepCondition] CHECK CONSTRAINT [FK_ProcessStepCondition_ProcessSteps]
GO