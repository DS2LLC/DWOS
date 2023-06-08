--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '19.2.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Late Order Notifications
--

-- LateOrderNotification Table
CREATE TABLE dbo.LateOrderNotification
    (
    LateOrderNotificationID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    ContactID int NOT NULL,
    UserID int NOT NULL,
    NotificationSent datetime2(7) NULL
    )  ON [PRIMARY];

GO

ALTER TABLE dbo.LateOrderNotification ADD CONSTRAINT
    PK_LateOrderNotification PRIMARY KEY CLUSTERED 
    (
    LateOrderNotificationID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

GO
ALTER TABLE dbo.LateOrderNotification ADD CONSTRAINT
    FK_LateOrderNotification_LateOrder FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE CASCADE
    ON DELETE CASCADE;
    
GO
ALTER TABLE dbo.LateOrderNotification ADD CONSTRAINT
    FK_LateOrderNotification_d_Contact FOREIGN KEY
    (
    ContactID
    ) REFERENCES dbo.d_Contact
    (
    ContactID
    ) ON UPDATE CASCADE
    ON DELETE CASCADE;

GO
ALTER TABLE dbo.LateOrderNotification ADD CONSTRAINT
    FK_LateOrderNotification_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE CASCADE
    ON DELETE NO ACTION;

GO

-- Contact option for late order notifications
ALTER TABLE dbo.d_Contact ADD
    LateOrderNotification bit NOT NULL CONSTRAINT DF_d_Contact_LateOrderNotification DEFAULT 0;
GO

ALTER TABLE dbo.ContactAdditionalCustomer ADD
    IncludeInLateOrderNotifications bit NOT NULL CONSTRAINT DF_ContactAdditionalCustomer_IncludeInLateOrderNotifications DEFAULT 0;
GO

-- Template for late order notification emails
INSERT INTO dbo.Templates(TemplateID, Template, Description, Tokens)
VALUES
(
    'LateOrderNotification',
    '<!DOCTYPE html>
<html>
  <head>
    <title>%COMPANY% - Notice About Order %ORDER%</title>
  </head>
  <body>
    <div style="text-align: center;">
      <img alt="%COMPANY%" src="%LOGO%" width="200" />
    </div>
    <p style="text-align: center;">%COMPANY% will be late to fulfill Order %ORDER%.</p>
  </body>
</html>
',
    'Email template to notify customer that an order will be late.',
    ''
);

GO

-- Update [Get_IsUserUsed] to check LateOrderNotification
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

    -- If is in [LateOrderNotification]
    IF (SELECT COUNT(*) FROM LateOrderNotification WHERE LateOrderNotification.UserID = @userID) > 0
        RETURN 1

    SET @isUsed = 0
    RETURN 0
END

GO

--
-- Portal Site - Current Process
--

GO

-- Gets the start date for a Work Order's current process.
CREATE FUNCTION [dbo].[fnGetCurrentProcessStartDate]
(
    @orderID int
)
RETURNS datetime2(7)
AS
BEGIN
    DECLARE @startDate datetime2(7);

    -- Find Next Dept that is started but not completed
    SET @startDate = 
    (
        SELECT TOP(1) StartDate
        FROM [OrderProcesses]
        WHERE StartDate IS NOT NULL AND EndDate IS NULL AND OrderID = @orderID
        ORDER BY StepOrder
    )

    Return @startDate;
END

GO
-- Gets the customer-facing name of the Work Order's current process
CREATE FUNCTION [dbo].[fnGetCurrentProcessPortal]
(
    @orderID int
)
RETURNS nvarchar(255)
AS
BEGIN
    -- Find the Work Order's current process
    DECLARE @currentOrderProcessID int;
    DECLARE @currentProcessName nvarchar(255) = 'N/A';

    -- Find the process that is in-progress.
    SET @currentOrderProcessID = 
    (
        SELECT TOP(1) OrderProcessesID
        FROM [OrderProcesses] 
        WHERE StartDate IS NOT NULL AND EndDate IS NULL AND OrderID = @orderID
        ORDER BY StepOrder
    );

    -- Get the name of the process
    IF (@currentOrderProcessID IS NOT NULL)
        BEGIN
            SET @currentProcessName = 
            (
                SELECT TOP 1 COALESCE(CustomerProcessAlias.Name, ProcessAlias.Name)
                FROM OrderProcesses
                INNER JOIN [Order] ON OrderProcesses.OrderID = [Order].OrderID
                INNER JOIN ProcessAlias ON OrderProcesses.ProcessAliasID = ProcessAlias.ProcessAliasID
                LEFT OUTER JOIN CustomerProcessAlias ON CustomerProcessAlias.CustomerID = [Order].CustomerID
                    AND CustomerProcessAlias.ProcessAliasID = OrderProcesses.ProcessAliasID
                WHERE OrderProcesses.OrderProcessesID = @currentOrderProcessID
                ORDER BY CustomerProcessAlias.Name ASC, ProcessAlias.Name ASC
            );
        END

    RETURN @currentProcessName
END

GO

--
-- Make Tracking Number an optional field.
--
INSERT INTO Fields (Name, Category, IsRequired, IsSystem, IsVisible, IsCustomer)
VALUES
(
    'Tracking Number', 'Shipping', 0, 1, 1, 0
);
GO

--
-- Country for addresses
--

-- Country Table
CREATE TABLE dbo.Country
    (
    CountryID int NOT NULL IDENTITY (1, 1),
    Name nvarchar(50) NOT NULL,
    IsSystem bit NOT NULL
    )  ON [PRIMARY]
GO

ALTER TABLE dbo.Country ADD CONSTRAINT
    PK_Country PRIMARY KEY CLUSTERED 
    (
    CountryID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

-- Default values for Country
SET IDENTITY_INSERT dbo.Country ON;
GO

INSERT INTO dbo.Country(CountryID, Name, IsSystem)
VALUES
(0, 'Unknown', 1),
(1, 'United States of America', 1),
(2, 'Canada', 1),
(3, 'Mexico', 1);

SET IDENTITY_INSERT dbo.Country OFF;

GO

-- Country column for Customer
ALTER TABLE dbo.Customer ADD
    CountryID int NOT NULL CONSTRAINT DF_Customer_CountryID DEFAULT 0;
GO

ALTER TABLE dbo.Customer ADD CONSTRAINT
    FK_Customer_Country FOREIGN KEY
    (
    CountryID
    ) REFERENCES dbo.Country
    (
    CountryID
    ) ON UPDATE NO ACTION
      ON DELETE NO ACTION;
GO

-- Country column for CustomerAddress
ALTER TABLE dbo.CustomerAddress ADD
    CountryID int NOT NULL CONSTRAINT DF_CustomerAddress_CountryID DEFAULT 0;
GO

ALTER TABLE dbo.CustomerAddress ADD CONSTRAINT
    FK_CustomerAddress_Country FOREIGN KEY
    (
    CountryID
    ) REFERENCES dbo.Country
    (
    CountryID
    ) ON UPDATE NO ACTION
      ON DELETE NO ACTION;
GO

-- Create temp table that relates states/provinces/territories to countries.
CREATE TABLE #TEMP_StateCountryMap
(
    CountryID int NOT NULL,
    State nvarchar(2) NOT NULL
);

GO

INSERT INTO #TEMP_StateCountryMap
VALUES
(1, 'AL'),
(1, 'AK'),
(1, 'AS'),
(1, 'AZ'),
(1, 'AR'),
(1, 'CA'),
(1, 'CO'),
(1, 'CT'),
(1, 'DE'),
(1, 'DC'),
(1, 'FM'),
(1, 'FL'),
(1, 'GA'),
(1, 'GU'),
(1, 'HI'),
(1, 'ID'),
(1, 'IL'),
(1, 'IN'),
(1, 'IA'),
(1, 'KS'),
(1, 'KY'),
(1, 'LA'),
(1, 'ME'),
(1, 'MH'),
(1, 'MD'),
(1, 'MA'),
(1, 'MI'),
(1, 'MN'),
(1, 'MS'),
(1, 'MO'),
(1, 'MT'),
(1, 'NE'),
(1, 'NV'),
(1, 'NH'),
(1, 'NJ'),
(1, 'NM'),
(1, 'NY'),
(1, 'NC'),
(1, 'ND'),
(1, 'MP'),
(1, 'OH'),
(1, 'OK'),
(1, 'OR'),
(1, 'PW'),
(1, 'PA'),
(1, 'PR'),
(1, 'RI'),
(1, 'SC'),
(1, 'SD'),
(1, 'TN'),
(1, 'TX'),
(1, 'UT'),
(1, 'VT'),
(1, 'VI'),
(1, 'VA'),
(1, 'WA'),
(1, 'WV'),
(1, 'WI'),
(1, 'WY'),
(2, 'AB'),
(2, 'BC'),
(2, 'MB'),
(2, 'NB'),
(2, 'NL'),
(2, 'NT'),
(2, 'NS'),
(2, 'NU'),
(2, 'ON'),
(2, 'PE'),
(2, 'QC'),
(2, 'SK'),
(2, 'YT');

-- Set country for the company.

DECLARE @companyState nvarchar(MAX) =
    (SELECT TOP 1 Value FROM ApplicationSettings WHERE SettingName = 'Company State');

SET @companyState = UPPER(@companyState);

DECLARE @companyCountry nvarchar(MAX) =
    ISNULL((SELECT TOP 1 CountryID FROM #TEMP_StateCountryMap WHERE State = @companyState), 0);

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'Company Country')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('Company Country', @companyCountry)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @companyCountry WHERE  [SettingName] = 'Company Country'
GO

-- Set country for customers.

UPDATE Customer
SET CountryID = #TEMP_StateCountryMap.CountryID
FROM Customer
INNER JOIN #TEMP_StateCountryMap ON Customer.State = #TEMP_StateCountryMap.State;

GO

-- Set country for customer addresses.

UPDATE CustomerAddress
SET CountryID = #TEMP_StateCountryMap.CountryID
FROM CustomerAddress
INNER JOIN #TEMP_StateCountryMap ON CustomerAddress.State = #TEMP_StateCountryMap.State;

GO

-- Drop temp table for state-country mappings.
DROP TABLE #TEMP_StateCountryMap;

GO

--
-- Import/Export Workflow
--

-- Add row to d_WorkStatus table
INSERT INTO dbo.d_WorkStatus(WorkStatusID)
VALUES
('Pending Import/Export Review');

GO

-- Permission for reviews
INSERT INTO dbo.SecurityRole(SecurityRoleID, Description, SecurityRoleCategoryID)
VALUES
('OrderImportExportReview', 'Ability to import/export review an order.', 'Sales');

GO

-- OrderReviewType table and values.
CREATE TABLE dbo.OrderReviewType
    (
    OrderReviewTypeID int NOT NULL,
    Name nvarchar(25) NOT NULL
    ) ON [PRIMARY];
GO

ALTER TABLE dbo.OrderReviewType ADD CONSTRAINT
    PK_OrderReviewType PRIMARY KEY CLUSTERED 
    (
    OrderReviewTypeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];
GO

INSERT INTO dbo.OrderReviewType(OrderReviewTypeID, Name)
VALUES
(1, 'Order Review'),
(2, 'Import/Export Review');
GO

-- OrderReviewTypeID column for OrderReview table.
ALTER TABLE dbo.OrderReview ADD
    OrderReviewTypeID int NOT NULL CONSTRAINT DF_OrderReview_OrderReviewTypeID DEFAULT 1;
GO

ALTER TABLE dbo.OrderReview ADD CONSTRAINT
    FK_OrderReview_OrderReviewType FOREIGN KEY
    (
    OrderReviewTypeID
    ) REFERENCES dbo.OrderReviewType
    (
    OrderReviewTypeID
    ) ON UPDATE NO ACTION
      ON DELETE NO ACTION;
GO

--
-- Update Portal database view to include new process columns and shipping carrier.
--
GO

-- View of Work Orders; used by the Customer Portal site.
ALTER VIEW [dbo].[vw_OrderSummary]
AS
SELECT dbo.Part.Name AS PartName, dbo.Part.ManufacturerID, dbo.Customer.Name AS CustomerName, dbo.[Order].OrderID, dbo.[Order].OrderDate, dbo.[Order].RequiredDate,
       dbo.[Order].Status, dbo.[Order].CompletedDate, dbo.[Order].Priority, dbo.[Order].PartQuantity, dbo.[Order].CurrentLocation, dbo.[Order].EstShipDate,
       dbo.[Order].AdjustedEstShipDate, dbo.[Order].CustomerWO, dbo.[Order].PurchaseOrder, dbo.[Order].CustomerID, [Order].BasePrice, [Order].PriceUnit,
       dbo.[Order].Weight,
       UPPER(dbo.OrderShipment.TrackingNumber) AS TrackingNumber,
       dbo.OrderShipment.ShippingCarrierID,
       dbo.[Order].WorkStatus, dbo.fnGetCurrentProcessPortal([Order].OrderID) AS CurrentProcess, dbo.fnGetCurrentProcessStartDate([Order].OrderID) AS CurrentProcessStartDate
FROM dbo.[Order]
      INNER JOIN dbo.Customer ON dbo.[Order].CustomerID = dbo.Customer.CustomerID
      INNER JOIN dbo.Part ON dbo.[Order].PartID = dbo.Part.PartID
      LEFT OUTER JOIN dbo.OrderShipment ON dbo.[Order].OrderID = dbo.OrderShipment.OrderID;
GO
