--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '22.1.1'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO



-- OrderReceiptNotification Table
CREATE TABLE dbo.OrderReceiptNotification
    (
    OrderReceiptNotificationID int NOT NULL IDENTITY (1, 1),
    OrderID int NOT NULL,
    ContactID int NOT NULL,
    UserID int NOT NULL,
    NotificationSent datetime2(7) NULL
    )  ON [PRIMARY];

GO


ALTER TABLE dbo.OrderReceiptNotification ADD CONSTRAINT
    PK_OrderReceiptNotification PRIMARY KEY CLUSTERED 
    (
    OrderReceiptNotificationID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

GO
ALTER TABLE dbo.OrderReceiptNotification ADD CONSTRAINT
    FK_OrderReceiptNotification_OrderReceipt FOREIGN KEY
    (
    OrderID
    ) REFERENCES dbo.[Order]
    (
    OrderID
    ) ON UPDATE CASCADE
    ON DELETE CASCADE;
    
GO
ALTER TABLE dbo.OrderReceiptNotification ADD CONSTRAINT
    FK_OrderReceiptNotification_d_Contact FOREIGN KEY
    (
    ContactID
    ) REFERENCES dbo.d_Contact
    (
    ContactID
    ) ON UPDATE CASCADE
    ON DELETE CASCADE;

GO
ALTER TABLE dbo.OrderReceiptNotification ADD CONSTRAINT
    FK_OrderReceiptNotification_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE CASCADE
    ON DELETE NO ACTION;

GO

--Add Column to d_contacts for OrderReceiptNotification
ALTER TABLE [dbo].[d_Contact] ADD OrderReceiptNotification BIT NOT NULL DEFAULT ((0));
GO

--Add Column to ContactAdditionalCustomer for IncludeInOrderReceiptNotification
ALTER TABLE [dbo].[ContactAdditionalCustomer] ADD IncludeInOrderReceiptNotification BIT NOT NULL DEFAULT ((0));
GO

--Update d_contacts to set emailaddress to all lowercase
  UPDATE [dbo].[d_Contact]
  SET EmailAddress = LOWER(EmailAddress)

--Add Template to Notification for OrderReceiptNotifications
INSERT INTO dbo.Templates(TemplateID, Template, Description, Tokens)
VALUES
(
'OrderReceiptNotification',
'
<!DOCTYPE html><html><head><title>%COMPANY% - Order Acknowledgment  Notification</title></head>
<body>
    <table style="width: 100%" >
        <tr style="font-family: Arial; color: #D00837b; font-size: 18px; font-weight: bold;">
            <td><a href="%COMPANY_URL%" style="color: #D00837b">%COMPANY%</a></td>
            
            <td style="text-align: right">Order Acknowledgment Notification</td>
        </tr>
        <tr>
            <td style="font-family: Verdana; font-size: 12px; font-weight: normal; padding: 3px; margin: auto;">
                %COMPANY_ADDRESS%<br>
                %COMPANY_CITY%, %COMPANY_STATE% %COMPANY_ZIP%<br>
                %COMPANY_PHONE%<br>
            </td>

			<td style="text-align: right">Date: %ORDER_DATE%</td>
        </tr>
    </table>
    <br>
    <table style="width: 100%;">
        <tr style="font-family: Verdana; font-size: 12px;">
            <td style="width: 50%">
				<span style="font-family: Verdana; color: #00837b; font-size: 14px; font-weight: bold;">Bill to:</span>
				<div style="border-width:1px;border-style:solid;padding:4px;margin-right:10px">
					
					<span>%CUSTOMER%</span><br>
					<span>%CUSTOMER_ADDRESS_1%</span><br>
					<span>%CUSTOMER_ADDRESS_2%</span><br>
					<span>%CUSTOMER_CITY%, %CUSTOMER_STATE% %CUSTOMER_ZIP%</span><br>
					<span>%CUSTOMER_COUNTRY%</span>
				</div>
			</td>
            <td>
				<span style="font-family: Verdana; color: #00837b; font-size: 14px; font-weight: bold;">Ship to:</span>
				<div style="border-width:1px;border-style:solid;padding:4px">
					<span>%SHIPPING_NAME%</span><br>
					<span>%SHIPPING_ADDRESS_1%</span><br>
					<span>%SHIPPING_ADDRESS_2%</span><br>
					<span>%SHIPPING_CITY%, %SHIPPING_STATE% %SHIPPING_ZIP%</span><br>
					<span>%SHIPPING_COUNTRY%</span>
				</div>
			</td>
        </tr>
       

    </table>
	 <br>
	<table style="width: 100%;" id="shipmentTable">
        <thead>
            <tr>
                <td colspan="5" style="font-family: Verdana; color: #00837b; font-size: 14px; font-weight: bold; height: 20px">Order</td>
            </tr>
        </thead>
        <tr style="font-family: Verdana; font-size: 12px; font-weight: normal; color: white; background-color: #00837b; text-align: center;">
            <td>P.O.</td>
			<td>Order#</td>
			<td></td>

			<td>Est. Ship Date</td>
			<td>Shipping Method</td>
        </tr>

        <tr style="font-family: Verdana; font-size: 12px; font-weight: normal; padding: 3px; margin: auto; text-align: center;" id="shipmentTemplate">
            <td>%PO%</td>
            <td>%WO%</td>
            <td></td>
            <td>%EST_SHIP%</td>
            <td>%SHIP_METHOD%</td>
        </tr>
    </table>
    <br>
    <table style="width: 100%;" id="shipmentTable">
        <thead>
            <tr>
                <td colspan="5" style="font-family: Verdana; color: #00837b; font-size: 14px; font-weight: bold; height: 20px">Parts:</td>
            </tr>
        </thead>
        <tr style="font-family: Verdana; font-size: 12px; font-weight: normal; color: white; background-color: #00837b; text-align: center;">
            <td>Quantity</td>
			<td>Part No.</td>
			<td>Rev.</td>
			<td>Description</td>
			<td>Price/Unit</td>
			<td>Fee</td>
			<td>Extention</td>
        </tr>

        <tr style="font-family: Verdana; font-size: 12px; font-weight: normal; padding: 3px; margin: auto; text-align: center;" id="shipmentTemplate">
            <td>%PART_QTY%</td>
            <td>%PART_NO%</td>
            <td>%PART_REV%</td>
            <td>%DESCRIPTION%</td>
			<td>%BASE_PRICE%/%PRICE_UNIT%</td>
			<td>%FEE%</td>
			<td>%TOTAL%</td>
        </tr>
    </table>
    <br>
    <div style="font-family: Verdana; font-size: 12px; font-weight: normal; padding: 3px; margin: 3px; width: 100%;">
        <p style="text-align: justify">Do not reply to this email address. This is an automated notification generated by %COMPANY% Notification Manager. Please direct all communications to <a href="mailto:%COMPANY_EMAIL%">Customer Service</a>. ITAR/EAR technical data is restricted by the Arms Export Control Act (Title 22, U.S.C., Sec 2751) or the Export Administration Act (Title 50, U.S.C., App. 2401), as amended. Violations of these export laws are subject to severe criminal penalties. The preceding information may be confidential or privileged. It only should be used or disseminated for the purpose of conducting business with %COMPANY%. If you are not an intended recipient, please delete the information from your system. Thank you for your cooperation. Delivery truck deliveries will be delivered on your next scheduled delivery date.</p>
    </div>
</body>
</html>
',
    'Email template to notify customer of order receipt.',
    '%ORDER%'
);

GO