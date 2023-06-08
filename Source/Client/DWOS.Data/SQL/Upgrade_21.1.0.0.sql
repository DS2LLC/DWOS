--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '21.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--Add QuotePartId to Orders
ALTER TABLE dbo.[Order] ADD
	QuotePartId int NULL
GO
ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO

--Add Instructions
ALTER TABLE dbo.PartProcessAnswer ADD
	Instruction nvarchar(1000) NULL
GO
ALTER TABLE dbo.PartProcessAnswer SET (LOCK_ESCALATION = TABLE)
GO

--Show process price on quote
ALTER TABLE dbo.QuotePartProcessPrice ADD
	ShowOnQuote bit NULL
GO
ALTER TABLE dbo.QuotePartProcessPrice SET (LOCK_ESCALATION = TABLE)
GO

--Show process on quote
ALTER TABLE dbo.QuotePart_Process ADD
	ShowOnQuote bit NULL
GO
ALTER TABLE dbo.QuotePart_Process SET (LOCK_ESCALATION = TABLE)
GO

--Add indexes for WIP screen

CREATE NONCLUSTERED INDEX BatchProcess_ID_ST_END_IX
ON dbo.BatchProcesses (BatchID,EndDate,StartDate)
GO

CREATE NONCLUSTERED INDEX IX_OrderWorkStatusHist_ID
ON dbo.OrderWorkStatusHistory (OrderId)
GO



