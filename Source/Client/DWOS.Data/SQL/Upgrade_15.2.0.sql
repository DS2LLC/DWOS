-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '15.2.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO


--
-- Fix SQL SELECT bug in fnGetCurrentProcessRemainingTime.
--
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
                SELECT TOP 1 OrderProcessesID
                FROM OrderProcesses
                WHERE OrderID = @orderID
                    AND EndDate IS NOT NULL
                    AND ProcessID = (SELECT o.RequisiteProcessID FROM OrderProcesses o WHERE o.OrderProcessesID = @currentOrderProcessID)
                ORDER BY EndDate DESC
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

--
-- Change OrderProcesses date rows to use datetime2.
--
DROP INDEX [IX_OP_START_END] ON [dbo].[OrderProcesses];
DROP INDEX [IX_OPEndDate] ON [dbo].[OrderProcesses];

ALTER TABLE [OrderProcesses]
ALTER COLUMN StartDate datetime2 null;

ALTER TABLE [OrderProcesses]
ALTER COLUMN EndDate datetime2 null;

ALTER TABLE [OrderProcesses]
ALTER COLUMN EstEndDate datetime2 null;

CREATE NONCLUSTERED INDEX [IX_OP_START_END] ON [dbo].[OrderProcesses]
(
    [OrderID] ASC,
    [StartDate] ASC,
    [EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

CREATE NONCLUSTERED INDEX [IX_OPEndDate] ON [dbo].[OrderProcesses]
(
    [EndDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO

--
-- fnGetPartRevisions - Function to return all revisions (past and present)
-- of a part. Created for use in the Part History report.
--
CREATE FUNCTION [dbo].[fnGetPartRevisions]
    (@partID INT)
RETURNS table
AS
RETURN
(
    -- Self & revisions that this came from
    WITH PartParentLookup AS
    (
        SELECT PartID, ParentID
        FROM Part
        WHERE PartID = @partID

        UNION ALL

        SELECT Part.PartID, Part.ParentID
        FROM PartParentLookup
        INNER JOIN Part ON PartParentLookup.ParentID = Part.PartID
    ),

    -- Revisions after the current one
    PartChildLookup AS (
        SELECT PartID, ParentID
        FROM Part
        WHERE ParentID = @partID

        UNION ALL

        SELECT Part.PartID, Part.ParentID
        FROM PartChildLookup
        INNER JOIN Part ON PartChildLookup.PartID = Part.ParentID
    )

    SELECT PartID
    From PartParentLookup

    UNION

    SELECT PartID
    FROM PartChildLookup
);
GO
--
-- PartHistoryReport permission.
--
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('PartHistoryReport', 'Ability to run the Part History Report.', 'Reports')
GO

--
-- Add 'fee calculation type'  column to QuotePartFees
--
ALTER TABLE QuotePartFees
    ADD FeeCalculationType nvarchar(50) not null DEFAULT 'Fixed'
GO

--
-- Order Weight changes
-- 
ALTER TABLE [Order]
    ADD [Weight] decimal(8,2) null
GO

ALTER TABLE [d_PriceUnit] ADD
    [DisplayName] nvarchar(50) not null DEFAULT '',
    [MinWeight] decimal(8,2) null,
    [MaxWeight] decimal(8,2) null,
	[Active] bit not null DEFAULT 1
GO

UPDATE [d_PriceUnit] SET
    [DisplayName] = [PriceUnitID]
GO

INSERT INTO [d_PriceUnit]([PriceUnitID], [DisplayName], [MinQuantity], [MaxQuantity], [MinWeight], [MaxWeight], [Active]) VALUES
    ('EachByWeight', 'Each By Weight', 0, 0, 30, 999999.99, 0),
    ('LotByWeight', 'Lot By Weight', 0, 0, 0, 30, 0)
GO

--- Update functions to support new pricing units
ALTER FUNCTION [dbo].[fnGetOrderTotalPrice] 
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
	ELSE IF (@priceUnit = 'Lot' OR @priceUnit = 'LotByWeight')
		BEGIN
			SET @result = (SELECT BasePrice FROM [Order] WHERE [Order].OrderID = @orderID)
		END
	ELSE IF (@priceUnit = 'EachByWeight')
		BEGIN
			SET @result = (SELECT [Order].BasePrice * IsNull([Order].Weight, 0) FROM [Order] WHERE [Order].OrderID = @orderID)
		END
	ELSE
		BEGIN
			SET @result = 0
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

-- Add Weight column for OrderTemplate
ALTER TABLE [OrderTemplate]
    ADD [Weight] decimal(8,2) null
GO

-- Update Get_BatchStatus to use [Order].Weight
ALTER PROCEDURE [dbo].[Get_BatchStatus] 
AS
BEGIN
	SET NOCOUNT ON;
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

SELECT        Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID) AS NextDept, 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID) AS CurrentProcess, SUM(BatchOrder.PartQuantity) AS PartCount, COUNT(BatchOrder.BatchID) AS OrderCount, 
                         SUM(Part.SurfaceArea * BatchOrder.PartQuantity) AS TotalSurfaceArea, SUM(ISNULL([Order].Weight, Part.Weight * BatchOrder.PartQuantity)) AS TotalWeight
FROM            [Order] INNER JOIN
                         BatchOrder ON [Order].OrderID = BatchOrder.OrderID INNER JOIN
                         Part ON [Order].PartID = Part.PartID RIGHT OUTER JOIN
                         Batch ON BatchOrder.BatchID = Batch.BatchID
WHERE        (Batch.Active = 1)
GROUP BY Batch.BatchID, Batch.OpenDate, Batch.Fixture, Batch.WorkStatus, Batch.CurrentLocation, dbo.fnGetNextDeptBatch(Batch.BatchID), 
                         dbo.fnGetCurrentProcessBatch(Batch.BatchID)

END
GO

-- Alter report table to allow NULL customer ID, which will be the default values for the company
ALTER TABLE Report ALTER COLUMN CustomerID INT  NULL
GO

-- Add default Packing Slip report
  INSERT INTO dbo.[Report] ([ReportType],[ReportName]) VALUES (1, 'Packing Slip')
  DECLARE @reportId int
  SET @reportId = SCOPE_IDENTITY();

  INSERT INTO dbo.[ReportFields] ([ReportID],[FieldName],[DisplayName],[Width],[DisplayOrder],[IsCustomField]) 
  VALUES (@reportId, 'OrderID', 'Work Order', 100, 1, 0)

  INSERT INTO dbo.[ReportFields] ([ReportID],[FieldName],[DisplayName],[Width],[DisplayOrder],[IsCustomField]) 
  VALUES (@reportId, 'CustomerWO', 'Customer WO', 100, 2, 0)

  INSERT INTO dbo.[ReportFields] ([ReportID],[FieldName],[DisplayName],[Width],[DisplayOrder],[IsCustomField]) 
  VALUES (@reportId, 'PurchaseOrder', 'Purchase Order', 100, 3, 0)

  INSERT INTO dbo.[ReportFields] ([ReportID],[FieldName],[DisplayName],[Width],[DisplayOrder],[IsCustomField]) 
  VALUES (@reportId, 'PartID', 'Part', 100, 4, 0)

  INSERT INTO dbo.[ReportFields] ([ReportID],[FieldName],[DisplayName],[Width],[DisplayOrder],[IsCustomField]) 
  VALUES (@reportId, 'PartQuantity', 'Quantity', 100, 5, 0)		
  
  GO