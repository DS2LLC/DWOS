-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '19.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Additional contacts for customers
--
CREATE TABLE dbo.ContactAdditionalCustomer
    (
    ContactAdditionalCustomerID int NOT NULL IDENTITY (1, 1),
    ContactID int NOT NULL,
    CustomerID int NOT NULL,
    IncludeInPortal bit NOT NULL,
    IncludeInShippingNotifications bit NOT NULL,
    IncludeInCocNotifications bit NOT NULL,
    IncludeInAutomatedNotifications bit NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.ContactAdditionalCustomer ADD CONSTRAINT
    PK_ContactAdditionalCustomer PRIMARY KEY CLUSTERED 
    (
    ContactAdditionalCustomerID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ContactAdditionalCustomer ADD CONSTRAINT
    FK_ContactAdditionalCustomer_d_Contact FOREIGN KEY
    (
    ContactID
    ) REFERENCES dbo.d_Contact
    (
    ContactID
    ) ON UPDATE  CASCADE
      ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.ContactAdditionalCustomer ADD CONSTRAINT
    FK_ContactAdditionalCustomer_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  CASCADE
      ON DELETE  CASCADE
GO

-- Create new ContactAdditionalCustomer rows from CustomerRelationship
INSERT INTO ContactAdditionalCustomer (
    ContactID,
    CustomerID,
    IncludeInPortal,
    IncludeInShippingNotifications,
    IncludeInCocNotifications,
    IncludeInAutomatedNotifications
    )
SELECT d_Contact.ContactID,
    CustomerRelationship.CustomerB AS CustomerID,
    d_contact.PortalAuthorized,
    d_contact.ShippingNotification,
    d_Contact.COCNotification,
    1 AS AutomatedNotification
FROM CustomerRelationship
INNER JOIN d_Contact ON CustomerRelationship.CustomerA = d_Contact.CustomerID

UNION

SELECT d_Contact.ContactID,
    CustomerRelationship.CustomerA AS CustomerID,
    d_contact.PortalAuthorized,
    d_contact.ShippingNotification,
    d_Contact.COCNotification,
    1 AS AutomatedNotification
FROM CustomerRelationship
INNER JOIN d_Contact ON CustomerRelationship.CustomerB = d_Contact.CustomerID;

GO

-- Delete CustomerRelationship table - no longer used
DROP TABLE CustomerRelationship;

GO

--
-- Order Approvals
--

-- Department email
ALTER TABLE dbo.d_Department ADD
    EmailAddress nvarchar(50) NULL;
GO

-- Product Class email
ALTER TABLE dbo.ProductClass ADD
    EmailAddress nvarchar(50) NULL;
GO

-- Contact option for order approval emails
ALTER TABLE dbo.d_Contact ADD
    ApprovalNotification bit NOT NULL CONSTRAINT DF_d_Contact_ApprovalNotification DEFAULT 0;
GO

ALTER TABLE dbo.ContactAdditionalCustomer ADD
    IncludeInApprovalNotifications bit NOT NULL CONSTRAINT DF_ContactAdditionalCustomer_IncludeInApprovalNotifications DEFAULT 0;
GO

-- OrderApprovalTerms table
CREATE TABLE dbo.OrderApprovalTerm
    (
    OrderApprovalTermID int NOT NULL IDENTITY (1, 1),
    Name nvarchar(50) NOT NULL,
    Terms nvarchar(255) NOT NULL,
    Active bit NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderApprovalTerm ADD CONSTRAINT
    DF_OrderApprovalTerm_Active DEFAULT 1 FOR Active
GO
ALTER TABLE dbo.OrderApprovalTerm ADD CONSTRAINT
    PK_OrderApprovalTerm PRIMARY KEY CLUSTERED 
    (
    OrderApprovalTermID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

-- OrderApproval table
CREATE TABLE dbo.OrderApproval
    (
    OrderApprovalID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    Status nvarchar(9) NOT NULL,
    UserID int NOT NULL,
    MediaID int NULL,
    OrderApprovalTermID int NULL,
    Notes nvarchar(255) NULL,
    DateCreated datetime2(7) NOT NULL,
    InitialEmailSent datetime2(7) NULL,
    ReminderSent datetime2(7) NULL,
    ContactID int NULL,
    ContactNotes nvarchar(255) NULL,
    ModifiedByContact datetime2(7) NULL
    )  ON [PRIMARY];
GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    DF_OrderApproval_DateCreated DEFAULT GETDATE() FOR DateCreated;
GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    PK_OrderApproval PRIMARY KEY CLUSTERED 
    (
    OrderApprovalID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    FK_OrderApproval_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  CASCADE
     ON DELETE  NO ACTION;

GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    FK_OrderApproval_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE;

GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    FK_OrderApproval_Media FOREIGN KEY
    (
    MediaID
    ) REFERENCES dbo.Media
    (
    MediaID
    ) ON UPDATE  CASCADE
     ON DELETE  NO ACTION;

GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    FK_OrderApproval_OrderApprovalTerm FOREIGN KEY
    (
    OrderApprovalTermID
    ) REFERENCES dbo.OrderApprovalTerm
    (
    OrderApprovalTermID
    ) ON UPDATE  CASCADE
     ON DELETE  NO ACTION

GO

ALTER TABLE dbo.OrderApproval ADD CONSTRAINT
    FK_OrderApproval_d_Contact FOREIGN KEY
    (
    ContactID
    ) REFERENCES dbo.d_Contact
    (
    ContactID
    ) ON UPDATE  CASCADE
     ON DELETE  NO ACTION
    
GO

-- Stored procedure for deleting unused media (added OrderApproval.MediaID)
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
                                Labels lbl
                              WHERE
                                m.MediaID = lbl.MediaID)
                  AND
                NOT EXISTS (SELECT *
                              FROM
                                LabelType lt
                              WHERE
                                m.MediaID = lt.MediaID)
                  AND
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
                                [SalesOrder_Media] so
                              WHERE
                                m.MediaID = so.MediaID)
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
                                m.MediaID = u.MediaID
                                OR m.MediaID = u.SignatureMediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [ProductClassLabels] pcl
                              WHERE
                                m.MediaID = pcl.MediaID)
                  AND
                  NOT EXISTS (SELECT *
                              FROM
                                [BillOfLadingMedia] bol
                              WHERE
                                m.MediaID = bol.MediaID)
                 AND
                 NOT EXISTS (SELECT *
                             FROM
                               [OrderApproval] orderApproval
                             WHERE
                               m.MediaID = orderApproval.MediaID))

  SELECT @rows - (SELECT count(*)
                  FROM
                    Media)
END

GO

-- Email template for order approval request.
INSERT INTO dbo.Templates(TemplateID, Template, Description, Tokens)
VALUES
(
    'OrderApprovalNotification',
    '<!DOCTYPE html>
<html>
  <head>
    <title>%COMPANY% - Request to Approve Order %ORDER%</title>
  </head>
  <body>
    <div style="text-align: center;">
      <img alt="%COMPANY%" src="%LOGO%" width="200" />
    </div>
    <p style="text-align: center;">%COMPANY% has requested your approval for Order %ORDER%. You may accept or reject this request on the <a href="%PORTAL_URL%">%COMPANY% Customer Portal</a>.</p>
  </body>
</html>
',
    'Email template to notify customer that an order needs approval.',
    '%ORDER%'
);

GO

-- Permission for creating order approval
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID],[Description],[SecurityRoleCategoryID])
     VALUES ('OrderEntry.OrderApproval','Ability to create approvals for work orders.', 'Sales');

GO

--
-- Default fees for customers
--
CREATE TABLE dbo.CustomerFee
    (
    CustomerFeeID int NOT NULL IDENTITY (1, 1),
    CustomerID int NOT NULL,
    OrderFeeTypeID nvarchar(50) NOT NULL,
    Charge decimal(13, 5) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.CustomerFee ADD CONSTRAINT
    PK_CustomerFee PRIMARY KEY CLUSTERED 
    (
    CustomerFeeID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.CustomerFee ADD CONSTRAINT
    FK_CustomerFee_Customer FOREIGN KEY
    (
    CustomerID
    ) REFERENCES dbo.Customer
    (
    CustomerID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.CustomerFee ADD CONSTRAINT
    FK_CustomerFee_OrderFeeType FOREIGN KEY
    (
    OrderFeeTypeID
    ) REFERENCES dbo.OrderFeeType
    (
    OrderFeeTypeID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

--
-- Description of Work
--
CREATE TABLE dbo.WorkDescription
    (
    WorkDescriptionID int NOT NULL IDENTITY (1, 1),
    Description nvarchar(255) NOT NULL,
    IsDefault bit NOT NULL CONSTRAINT DF_WorkDescription_IsDefault DEFAULT 0
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.WorkDescription ADD CONSTRAINT
    PK_WorkDescription PRIMARY KEY CLUSTERED 
    (
    WorkDescriptionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE dbo.OrderWorkDescription
    (
    OrderWorkDescriptionID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    WorkDescriptionID int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderWorkDescription ADD CONSTRAINT
    PK_OrderWorkDescription PRIMARY KEY CLUSTERED 
    (
    OrderWorkDescriptionID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderWorkDescription ADD CONSTRAINT
    FK_OrderWorkDescription_Order FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  CASCADE 
     ON DELETE  CASCADE 
    
GO
ALTER TABLE dbo.OrderWorkDescription ADD CONSTRAINT
    FK_OrderWorkDescription_WorkDescription FOREIGN KEY
    (
    WorkDescriptionID
    ) REFERENCES dbo.WorkDescription
    (
    WorkDescriptionID
    ) ON UPDATE  NO ACTION 
     ON DELETE  NO ACTION 
    
GO

--
-- Address for Product Class
--
ALTER TABLE dbo.ProductClass ADD
    Address1 nvarchar(MAX) NULL,
    Address2 nvarchar(MAX) NULL,
    City nvarchar(50) NULL,
    State nvarchar(50) NULL,
    Zip nvarchar(50) NULL
GO

--
-- Hold Notifications
--

-- HoldNotification Table
CREATE TABLE dbo.OrderHoldNotification
    (
    OrderHoldNotificationID int NOT NULL IDENTITY (1, 1),
    OrderHoldID int NOT NULL,
    ContactID int NOT NULL,
    NotificationSent datetime2(7) NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.OrderHoldNotification ADD CONSTRAINT
    PK_OrderHoldNotification PRIMARY KEY CLUSTERED 
    (
    OrderHoldNotificationID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.OrderHoldNotification ADD CONSTRAINT
    FK_OrderHoldNotification_OrderHold FOREIGN KEY
    (
    OrderHoldID
    ) REFERENCES dbo.OrderHold
    (
    OrderHoldID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO
ALTER TABLE dbo.OrderHoldNotification ADD CONSTRAINT
    FK_OrderHoldNotification_d_Contact FOREIGN KEY
    (
    ContactID
    ) REFERENCES dbo.d_Contact
    (
    ContactID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE
    
GO

-- Contact option for hold notifications
ALTER TABLE dbo.d_Contact ADD
    HoldNotification bit NOT NULL CONSTRAINT DF_d_Contact_HoldNotification DEFAULT 0;
GO

ALTER TABLE dbo.ContactAdditionalCustomer ADD
    IncludeInHoldNotifications bit NOT NULL CONSTRAINT DF_ContactAdditionalCustomer_IncludeInHoldlNotifications DEFAULT 0;
GO

-- Template for hold notification emails
INSERT INTO dbo.Templates(TemplateID, Template, Description, Tokens)
VALUES
(
    'HoldNotification',
    '<!DOCTYPE html>
<html>
  <head>
    <title>%COMPANY% - Order %ORDER% is On Hold</title>
  </head>
  <body>
    <div style="text-align: center;">
      <img alt="%COMPANY%" src="%LOGO%" width="200" />
    </div>
    <p style="text-align: center;">%COMPANY% has placed Order %ORDER% on hold.</p>
  </body>
</html>
',
    'Email template to notify customer that an order is on hold.',
    '%ORDER%, %HOLDREASON%'
);

GO
