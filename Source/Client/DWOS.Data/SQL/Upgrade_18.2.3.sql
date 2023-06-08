-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.2.3'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Add 'Require COC' option to customer
ALTER TABLE dbo.Customer ADD
    RequireCocByDefault bit NOT NULL CONSTRAINT DF_Customer_RequireCocByDefault DEFAULT 0
GO

-- Update process's price fields to support values up to $9999.99999
ALTER TABLE dbo.Process
ALTER COLUMN Price decimal(9,5) NULL;
GO

ALTER TABLE dbo.Process
ALTER COLUMN MinPrice decimal(9,5) NULL
GO

-- Fix recursion issue in stored procedure

-- =============================================
-- Description: Get the orders price history from uses of its part and all
--              of its ancestors.
-- =============================================
ALTER PROCEDURE [dbo].[Get_OrderPriceHistory]
    @PartID int
AS
BEGIN
    SET NOCOUNT ON;

    -- Get all parts that are parents of this part
    WITH PartHistory (PartID, ParentID)
    AS
    (
        -- Anchor member definition
        SELECT PartID, ParentID FROM Part
        WHERE PartID = @PartID
        UNION ALL

        -- Recursive member definition
        SELECT p2.PartID, p2.ParentID FROM Part p2, PartHistory ph
        WHERE p2.PartID = ph.ParentID
        AND p2.PartID != ph.PartID
    )

    -- Return price of all orders in this parts history
    SELECT OrderID, OrderDate, BasePrice, PriceUnit, PartQuantity FROM [Order] WHERE PartID IN (
    SELECT PartID FROM PartHistory )
    OPTION (MAXRECURSION 32767)
END

