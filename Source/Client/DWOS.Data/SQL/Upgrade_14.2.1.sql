-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '14.2.1'


IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
	INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
	UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO


-- Create new User to Customer Junction Table

CREATE TABLE [dbo].[User_Customers](
	[UserId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_User_Customers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[User_Customers]  WITH CHECK ADD  CONSTRAINT [FK_User_Customers_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[User_Customers] CHECK CONSTRAINT [FK_User_Customers_Customer]
GO

ALTER TABLE [dbo].[User_Customers]  WITH CHECK ADD  CONSTRAINT [FK_User_Customers_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[User_Customers] CHECK CONSTRAINT [FK_User_Customers_Users]
GO

-- Update Priority 'First' to remove extra space
UPDATE [dbo].[d_Priority]    SET [PriorityID] = 'First'   WHERE  [PriorityID] = 'First '
GO

-- Add Weight to Part Table
ALTER TABLE dbo.Part ADD Weight decimal(6, 2) NULL
GO

-- Add Weight to list of optional fields
INSERT INTO [dbo].[Fields]
           ([Name],[Category],[IsRequired],[IsSystem],[IsVisible])
     VALUES
           ('Weight', 'Part', 1, 1, 1)
GO

-- Add Weight to Quote part also
ALTER TABLE dbo.QuotePart ADD 	Weight decimal(6, 2) NULL
GO

-- Update get current process date
ALTER FUNCTION [dbo].[fnGetCurrentProcessDate] 
(
	@orderID int
)
RETURNS Date
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @dueDate Date
	
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
			IF (@dueDate IS NOT NULL)
				BEGIN
					SET @dueDate = DATEADD(year, -100, @dueDate) 
				END
		END
			
	Return @dueDate

END
GO


INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('AddOrderNote',	'Ability to add a note to an order within the work order summary.',	'Order Processing')
GO

INSERT INTO [dbo].[SecurityRole]
           ([SecurityRoleID]
           ,[Description]
           ,[SecurityRoleCategoryID])
     VALUES
           ('OrderHold',	'Ability to the hold on an order within the work order summary',	'Order Processing')
GO


/*** Update order process table to change datetime fields to date fields ***/

DROP INDEX [IX_OP_START_END] ON [dbo].[OrderProcesses]
GO
DROP INDEX [IX_OPEndDate] ON [dbo].[OrderProcesses]
GO

ALTER TABLE OrderProcesses ALTER COLUMN StartDate date null
ALTER TABLE OrderProcesses ALTER COLUMN EndDate date null
ALTER TABLE OrderProcesses ALTER COLUMN EstEndDate date null
GO

CREATE NONCLUSTERED INDEX [IX_OP_START_END] ON [dbo].[OrderProcesses]
(
	[OrderID] ASC,
	[StartDate] ASC,
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_OPEndDate] ON [dbo].[OrderProcesses]
(
	[EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/*** Update function to work with the new Date field ***/
ALTER FUNCTION [dbo].[fnGetCurrentProcessRemainingTime] 
(
	@orderID int
)
RETURNS nvarchar(50)
AS
BEGIN
	-- FIND THE NEXT DEPARTMENT FOR THE ORDER
	DECLARE @currentOrderProcessID int
	DECLARE @constrainedProcessID int
	DECLARE @currentProcessTime int
	DECLARE @requisiteHours decimal(9,2)

	-- Find Next Dept that is started but not completed
	SET @currentOrderProcessID = 
	(
		SELECT  TOP(1) OrderProcessesID FROM [OrderProcesses] 
		WHERE EndDate IS NULL AND OrderID = @orderID AND RequisiteHours IS NOT NULL
		ORDER BY StepOrder
	)
		
	-- get the name of the process
	IF (@currentOrderProcessID IS NOT NULL)
		BEGIN
			SET @constrainedProcessID = 
			(
				SELECT OrderProcessesID FROM OrderProcesses WHERE OrderID = @orderID AND EndDate IS NOT NULL AND ProcessID = (SELECT o.RequisiteProcessID FROM OrderProcesses o WHERE o.OrderProcessesID = @currentOrderProcessID)
			)
		END
	 
	IF (@constrainedProcessID IS NOT NULL)
	 BEGIN
			SET @requisiteHours = (SELECT (o.RequisiteHours * 60) FROM OrderProcesses o WHERE o.OrderProcessesID = @currentOrderProcessID)
			SET @currentProcessTime = 
			(
				(SELECT DATEDIFF (minute, GetDate(), DATEADD(minute, @requisiteHours, CAST(EndDate as DATETIME))) FROM OrderProcesses WHERE OrderProcessesID = @constrainedProcessID) 
			)
		END

	RETURN @currentProcessTime

END
GO

-- Add Quote Part Fees

CREATE TABLE [dbo].[QuotePartFees](
	[QuotePartFeeID] [int] IDENTITY(1,1) NOT NULL,
	[QuotePartID] [int] NOT NULL,
	[FeeType] [nvarchar](50) NOT NULL,
	[Charge] [smallmoney] NOT NULL,
 CONSTRAINT [PK_QuotePartFees] PRIMARY KEY CLUSTERED 
(
	[QuotePartFeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[QuotePartFees]  WITH CHECK ADD  CONSTRAINT [FK_QuotePartFees_QuotePart] FOREIGN KEY([QuotePartID])
REFERENCES [dbo].[QuotePart] ([QuotePartID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QuotePartFees] CHECK CONSTRAINT [FK_QuotePartFees_QuotePart]
GO