-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '17.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

ALTER TABLE dbo.Process ADD
    ProcessCode nvarchar(20) NULL;

GO

--
-- SYSPRO invoice monitoring
--
CREATE TABLE dbo.SysproInvoice
    (
    SysproInvoiceId int NOT NULL IDENTITY (1, 1),
    TransmissionReference nvarchar(14) NOT NULL,
    FileName nvarchar(255) NOT NULL,
    Status nvarchar(10) NOT NULL,
    Message nvarchar(255) NULL,
    Created datetime2(7) NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.SysproInvoice ADD CONSTRAINT
    DF_SysproInvoice_Created DEFAULT getdate() FOR Created
GO
ALTER TABLE dbo.SysproInvoice ADD CONSTRAINT
    PK_SysproInvoice PRIMARY KEY CLUSTERED 
    (
    SysproInvoiceId
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO

CREATE TABLE dbo.SysproInvoiceOrder
    (
    SysproInvoiceId int NOT NULL,
    OrderId int NOT NULL
    )  ON [PRIMARY]
GO
ALTER TABLE dbo.SysproInvoiceOrder ADD CONSTRAINT
    FK_SysproInvoiceOrder_Order FOREIGN KEY
    (
    OrderId
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE  NO ACTION 
    ON DELETE  CASCADE 
GO

ALTER TABLE dbo.SysproInvoiceOrder ADD CONSTRAINT
    FK_SysproInvoiceOrder_SysproInvoice FOREIGN KEY
    (
    SysproInvoiceId
    ) REFERENCES dbo.SysproInvoice
    (
    SysproInvoiceId
    ) ON UPDATE  NO ACTION 
    ON DELETE  CASCADE 
GO

--
-- Saving batch amperage
--
ALTER TABLE dbo.Batch ADD
    AmpsPerSquareFoot float(53) NULL
GO

--
-- Make Serial Number an optional field.
--
INSERT INTO Fields
     (Name, Alias, Category, IsRequired, IsSystem, IsVisible)
VALUES
    ('Serial Number', NULL, 'Order', 0, 1, 0);

GO