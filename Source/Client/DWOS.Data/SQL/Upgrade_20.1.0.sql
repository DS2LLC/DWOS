--
-- Update Database Version
--
DECLARE @currentVersion nvarchar(50) = '20.1.0'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- Part-level Custom Fields
--

-- PartLevelCustomField table
CREATE TABLE [dbo].[PartLevelCustomField](
    [PartLevelCustomFieldID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](255) NULL,
    [DisplayOnTraveler] [bit] NOT NULL,
    [DisplayOnCOC] [bit] NOT NULL,
    [CustomerID] [int] NOT NULL,
    [DefaultValue] [nvarchar](255) NULL,
    [IsVisible] [bit] NOT NULL,
    [ListID] [int] NULL,
 CONSTRAINT [PK_PartLevelCustomField] PRIMARY KEY CLUSTERED 
(
    [PartLevelCustomFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PartLevelCustomField] ADD  CONSTRAINT [DF_PartLevelCustomField_DisplayOnTraveler]  DEFAULT ((0)) FOR [DisplayOnTraveler]
GO

ALTER TABLE [dbo].[PartLevelCustomField] ADD  CONSTRAINT [DF_PartLevelCustomField_DisplayOnCOC]  DEFAULT ((0)) FOR [DisplayOnCOC]
GO

ALTER TABLE [dbo].[PartLevelCustomField] ADD  CONSTRAINT [DF_PartLevelCustomField_IsVisible]  DEFAULT ((1)) FOR [IsVisible]
GO

ALTER TABLE [dbo].[PartLevelCustomField]  WITH CHECK ADD  CONSTRAINT [FK_PartLevelCustomField_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PartLevelCustomField] CHECK CONSTRAINT [FK_PartLevelCustomField_Customer]
GO

ALTER TABLE [dbo].[PartLevelCustomField]  WITH CHECK ADD CONSTRAINT [FK_PartLevelCustomField_Lists] FOREIGN KEY([ListID])
REFERENCES [dbo].[Lists] ([ListID])
GO

ALTER TABLE [dbo].[PartLevelCustomField] CHECK CONSTRAINT [FK_PartLevelCustomField_Lists]
GO

-- PartCustomFields table

CREATE TABLE [dbo].[PartCustomFields](
    [PartID] [int] NOT NULL,
    [PartLevelCustomFieldID] [int] NOT NULL,
    [Value] [nvarchar](255) NULL,
 CONSTRAINT [PK_PartCustomFields] PRIMARY KEY CLUSTERED 
(
    [PartID] ASC,
    [PartLevelCustomFieldID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PartCustomFields]  WITH CHECK ADD  CONSTRAINT [FK_PartCustomFields_Part] FOREIGN KEY([PartID])
REFERENCES [dbo].[Part] ([PartID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PartCustomFields]  WITH CHECK ADD  CONSTRAINT [FK_PartCustomFields_PartLevelCustomField] FOREIGN KEY([PartLevelCustomFieldID])
REFERENCES [dbo].[PartLevelCustomField] ([PartLevelCustomFieldID])
ON DELETE NO ACTION
GO


ALTER TABLE [dbo].[PartCustomFields] CHECK CONSTRAINT [FK_PartCustomFields_Part]
GO

