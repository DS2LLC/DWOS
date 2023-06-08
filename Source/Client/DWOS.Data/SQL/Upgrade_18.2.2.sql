-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '18.2.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Update [Get_IsUserUsed] to check BatchCOC
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

    -- If is in [BatchCOC]
    IF (SELECT COUNT(*) FROM BatchCOC WHERE BatchCOC.QAUser = @userID) > 0
        RETURN 1

    SET @isUsed = 0
    RETURN 0
END

GO

--
-- Include date and time in BatchCOC.DateCertified
--
ALTER TABLE BatchCOC
ALTER COLUMN DateCertified datetime2(7) NOT NULL;

GO

--
-- Quote by weight
--

-- QuotePart columns for pricing by weight
ALTER TABLE dbo.QuotePart ADD
    PriceBy nvarchar(8) NOT NULL CONSTRAINT DF_QuotePart_PriceBy DEFAULT N'Quantity',
    TotalWeight decimal(14, 8) NULL
GO

--
-- Allow quote processes to be priced by weight
--
ALTER TABLE QuotePartProcessPrice
ADD MinValue nvarchar(12) NULL,
    MaxValue nvarchar(12) NULL;

GO

UPDATE QuotePartProcessPrice
SET MinValue = CAST(MinQuantity as nvarchar(12)),
MaxValue = CAST(MaxQuantity AS nvarchar(12));

GO

ALTER TABLE QuotePartProcessPrice
DROP COLUMN MinQuantity, MaxQuantity;

GO

--
-- Remove text limit for alias traveler notes
--
ALTER TABLE ProcessAlias
ALTER COLUMN TravelerNotes nvarchar(MAX) NULL;

GO

--
-- Accounting code for product class
--
ALTER TABLE dbo.ProductClass ADD
    AccountingCode nvarchar(50) NULL;

GO

--
-- CustomerAddress - Active field
--
ALTER TABLE dbo.CustomerAddress ADD
    Active bit NOT NULL CONSTRAINT DF_CustomerAddress_Active DEFAULT 1
GO

--
-- CustomerShipping - Active field
--
ALTER TABLE dbo.CustomerShipping ADD
    Active bit NOT NULL CONSTRAINT DF_CustomerShipping_Active DEFAULT 1
GO
