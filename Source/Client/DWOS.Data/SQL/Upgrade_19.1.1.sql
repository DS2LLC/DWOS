--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '19.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Adjusted Ship Date for Work Orders
--
ALTER TABLE [Order]
ADD AdjustedEstShipDate date NULL;
GO

ALTER TABLE SalesOrder
ADD AdjustedEstShipDate date NULL;
GO

INSERT INTO Fields (Name, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Adjusted Est. Ship Date', 'Order', 0, 1, 0, 0
);
GO

-- Update view used by Portal site
ALTER VIEW [dbo].[vw_OrderSummary]
AS
SELECT dbo.Part.Name AS PartName, dbo.Part.ManufacturerID, dbo.Customer.Name AS CustomerName, dbo.[Order].OrderID, dbo.[Order].OrderDate, dbo.[Order].RequiredDate,
       dbo.[Order].Status, dbo.[Order].CompletedDate, dbo.[Order].Priority, dbo.[Order].PartQuantity, dbo.[Order].CurrentLocation, dbo.[Order].EstShipDate,
       dbo.[Order].AdjustedEstShipDate, dbo.[Order].CustomerWO, dbo.[Order].PurchaseOrder, dbo.[Order].CustomerID,
       UPPER(dbo.OrderShipment.TrackingNumber) AS TrackingNumber
FROM dbo.[Order]
      INNER JOIN dbo.Customer ON dbo.[Order].CustomerID = dbo.Customer.CustomerID
      INNER JOIN dbo.Part ON dbo.[Order].PartID = dbo.Part.PartID
      LEFT OUTER JOIN dbo.OrderShipment ON dbo.[Order].OrderID = dbo.OrderShipment.OrderID;
GO
