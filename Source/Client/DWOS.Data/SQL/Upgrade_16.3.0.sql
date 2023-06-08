-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.3.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

-- Customer Field - Default Value
ALTER TABLE CustomField
ADD DefaultValue nvarchar(255) NULL;

GO

-- Fix fnGetOrderTotalPrice overflow issue
--
-- Scale = 38
-- Maximum value for scale.
--
-- Precision = 13
-- Default value for precision - using anything else could cause discrepancies
ALTER FUNCTION [dbo].[fnGetOrderTotalPrice] 
(
    @orderID int
)
RETURNS decimal(38,13)
AS
BEGIN
    
    DECLARE @result decimal(38,13)
    DECLARE @percentValue decimal(38,13)
    DECLARE @fixedValue decimal(38,13)

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

-- Update fnGetOrderFeePrice w/ EachByWeight and overflow issue fix
ALTER FUNCTION [dbo].[fnGetOrderFeePrice]
(
    @orderID int
)
RETURNS decimal(38,13)
AS
BEGIN

    DECLARE @result decimal(38,13)
    DECLARE @percentValue decimal(38,13)
    DECLARE @fixedValue decimal(38,13)

    DECLARE @priceUnit nvarchar(50) = (SELECT [Order].PriceUnit FROM [Order] WHERE [Order].OrderID = @orderID)

    IF (@priceUnit = 'Each')
        BEGIN
            SET @result = (SELECT BasePrice * [Order].PartQuantity FROM [Order] WHERE [Order].OrderID = @orderID)
        END
    ELSE IF (@priceUnit = 'EachByWeight')
        BEGIN
            SET @result = (SELECT [Order].BasePrice * IsNull([Order].Weight, 0) FROM [Order] WHERE [Order].OrderID = @orderID)
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

-- Make Order Required Date an optional field.
INSERT INTO Fields
     (Name, Alias, Category, IsRequired, IsSystem, IsVisible)
VALUES
    ('Required Date', NULL, 'Order', 1, 1, 1);

GO

-- Rename permission for employee resource center
UPDATE SecurityRole
SET SecurityRoleID = 'UserManager.ResourceCenter',
    Description = 'Ability to access the employee resource center.'
WHERE SecurityRoleID = 'UserManager.Salary';

GO

-- Cleanup - Remove unused Name column from DocumentLink
ALTER TABLE DocumentLink
DROP COLUMN Name;

GO

--
-- Track labor time independent of process
-- Not implemented as part of 16.3.0.0 - may be implemented at a
-- later date.
--

-- Order
CREATE TABLE dbo.OrderOperator
    (
    OrderOperatorID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    UserID int NOT NULL,
    Status nvarchar(8) NOT NULL
    )
GO

ALTER TABLE dbo.OrderOperator ADD CONSTRAINT
    PK_OrderOperator PRIMARY KEY CLUSTERED 
    (
    OrderOperatorID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

ALTER TABLE dbo.OrderOperator ADD CONSTRAINT
    FK_OrderOperator_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.OrderOperator ADD CONSTRAINT
    FK_OrderOperator_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION

GO

CREATE TABLE dbo.OrderOperatorTime
    (
    OrderOperatorTimeID int NOT NULL IDENTITY (1, 1),
    OrderOperatorID int NULL,
    StartTime datetime2(7) NOT NULL,
    EndTime datetime2(7) NULL,
    WorkStatus nvarchar(50) NOT NULL
    )
GO
ALTER TABLE dbo.OrderOperatorTime ADD CONSTRAINT
    PK_OrderOperatorTime PRIMARY KEY CLUSTERED 
    (
    OrderOperatorTimeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE dbo.OrderOperatorTime ADD CONSTRAINT
    FK_OrderOperatorTime_OrderOperator FOREIGN KEY
    (
    OrderOperatorID
    ) REFERENCES dbo.OrderOperator
    (
    OrderOperatorID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

-- Batch
CREATE TABLE dbo.BatchOperator
    (
    BatchOperatorID int NOT NULL IDENTITY (1, 1),
    BatchID int NOT NULL,
    UserID int NOT NULL,
    Status nvarchar(8) NOT NULL
    )
GO

ALTER TABLE dbo.BatchOperator ADD CONSTRAINT
    PK_BatchOperator PRIMARY KEY CLUSTERED 
    (
    BatchOperatorID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

ALTER TABLE dbo.BatchOperator ADD CONSTRAINT
    FK_BatchOperator_Batch FOREIGN KEY
    (
    BatchID
    ) REFERENCES dbo.[Batch]
    (
    BatchID
    ) ON UPDATE  NO ACTION
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.BatchOperator ADD CONSTRAINT
    FK_BatchOperator_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  NO ACTION
     ON DELETE  NO ACTION

GO

CREATE TABLE dbo.BatchOperatorTime
    (
    BatchOperatorTimeID int NOT NULL IDENTITY (1, 1),
    BatchOperatorID int NULL,
    StartTime datetime2(7) NOT NULL,
    EndTime datetime2(7) NULL,
    WorkStatus nvarchar(50) NOT NULL
    )
GO
ALTER TABLE dbo.BatchOperatorTime ADD CONSTRAINT
    PK_BatchOperatorTime PRIMARY KEY CLUSTERED 
    (
    BatchOperatorTimeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO
ALTER TABLE dbo.BatchOperatorTime ADD CONSTRAINT
    FK_BatchOperatorTime_BatchOperator FOREIGN KEY
    (
    BatchOperatorID
    ) REFERENCES dbo.BatchOperator
    (
    BatchOperatorID
    ) ON UPDATE  NO ACTION 
     ON DELETE  CASCADE 
    
GO

--
-- Decimal Difference input types
--
INSERT INTO d_InputType(InputType, AllowInInspection)
VALUES
(
    'DecimalBefore', 0
),
(
    'DecimalAfter', 0
),
(
    'DecimalDifference', 0
);

GO

---
--- Process Question Tolerance
---
ALTER TABLE ProcessQuestion
ADD Tolerance nvarchar(50) NULL;

GO

--
-- Custom Fields for Processing
--

-- Rename CustomField.PartMarkToken column to TokenName
EXEC sp_rename 'CustomField.PartMarkToken', 'TokenName', 'COLUMN';

GO

-- Add ProcessUnique to CustomField

ALTER TABLE CustomField
ADD ProcessUnique bit NOT NULL DEFAULT 0

GO

-- Fields for ProcessQuestion
CREATE TABLE dbo.ProcessQuestionField (
    ProcessQuestionFieldID INT NOT NULL IDENTITY(1, 1),
    ProcessQuestionID INT NOT NULL,
    FieldName NVARCHAR(10) NOT NULL,
    TokenName NVARCHAR(50) NOT NULL
    ) ON [PRIMARY]
GO

ALTER TABLE dbo.ProcessQuestionField ADD CONSTRAINT
    PK_ProcessQuestionField PRIMARY KEY CLUSTERED 
    (
    ProcessQuestionFieldID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

ALTER TABLE dbo.ProcessQuestionField ADD CONSTRAINT
    FK_ProcessQuestionField_ProcessQuestion FOREIGN KEY
    (
    ProcessQuestionID
    ) REFERENCES dbo.ProcessQuestion
    (
    ProcessQuestionID
    ) ON UPDATE  NO ACTION 
    ON DELETE  CASCADE 

GO

--
-- Add fnGetHasPartMark function for convenient access to part marking info.
--
CREATE FUNCTION [dbo].[fnGetHasPartMark]
(
    @orderID int
)
RETURNS bit
AS
BEGIN
RETURN
(
    SELECT CASE WHEN Count(*) > 0 THEN 1 ELSE 0 END
    FROM OrderPartMark
    WHERE OrderID = @orderID
)
END
GO

--
-- Statement of Repairs
--

-- Require Repair Statement for Customer Addresses
ALTER TABLE dbo.CustomerAddress ADD
    RequireRepairStatement bit NOT NULL DEFAULT 0
GO

-- Imported Price for Orders
ALTER TABLE dbo.[Order] ADD
    ImportedPrice decimal(11, 5) NULL
GO

--
-- Serial Numbers for Work Orders
--
CREATE TABLE dbo.OrderSerialNumber
    (
    OrderSerialNumberID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    PartOrder int NOT NULL,
    Number nvarchar(10) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderSerialNumber ADD CONSTRAINT
    PK_OrderSerialNumber PRIMARY KEY CLUSTERED 
    (
    OrderSerialNumberID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderSerialNumber ADD CONSTRAINT
    FK_OrderSerialNumber_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION 
    ON DELETE  CASCADE 
GO
