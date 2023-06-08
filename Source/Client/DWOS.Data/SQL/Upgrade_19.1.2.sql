--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '19.1.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Add Quantity to Fields table
--
INSERT INTO Fields (Name, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Quantity', 'Quote', 1, 1, 1, 0
);
GO

-- Set Quantity to nullable in QuotePart table
ALTER TABLE [dbo].[QuotePart] 
ALTER COLUMN [Quantity] INT NULL;
GO
--
-- Update Customer Portal view to include pricing info and weight
--
ALTER VIEW [dbo].[vw_OrderSummary]
AS
SELECT dbo.Part.Name AS PartName, dbo.Part.ManufacturerID, dbo.Customer.Name AS CustomerName, dbo.[Order].OrderID, dbo.[Order].OrderDate, dbo.[Order].RequiredDate,
       dbo.[Order].Status, dbo.[Order].CompletedDate, dbo.[Order].Priority, dbo.[Order].PartQuantity, dbo.[Order].CurrentLocation, dbo.[Order].EstShipDate,
       dbo.[Order].AdjustedEstShipDate, dbo.[Order].CustomerWO, dbo.[Order].PurchaseOrder, dbo.[Order].CustomerID, [Order].BasePrice, [Order].PriceUnit,
       dbo.[Order].Weight,
       UPPER(dbo.OrderShipment.TrackingNumber) AS TrackingNumber
FROM dbo.[Order]
      INNER JOIN dbo.Customer ON dbo.[Order].CustomerID = dbo.Customer.CustomerID
      INNER JOIN dbo.Part ON dbo.[Order].PartID = dbo.Part.PartID
      LEFT OUTER JOIN dbo.OrderShipment ON dbo.[Order].OrderID = dbo.OrderShipment.OrderID;
GO
