-- Update Database Version
DECLARE @currentVersion nvarchar(50) = '16.2.2'

IF NOT EXISTS (SELECT * FROM [ApplicationSettings] WHERE [SettingName] = 'DatabaseVersion')
    INSERT INTO [dbo].[ApplicationSettings] ([SettingName],[Value]) VALUES ('DatabaseVersion',@currentVersion)
ELSE
    UPDATE [dbo].[ApplicationSettings] SET [Value] = @currentVersion WHERE  [SettingName] = 'DatabaseVersion'
GO

--
-- User Salary
--

CREATE TABLE dbo.UserSalary
(
    UserSalaryID int NOT NULL IDENTITY (1, 1),
    UserID int NOT NULL,
    EffectiveDate date NOT NULL,
    Salary decimal(11,5) NOT NULL,
    Burden decimal(11,5) NOT NULL
);

GO

ALTER TABLE dbo.UserSalary ADD CONSTRAINT
    PK_UserSalaryID PRIMARY KEY CLUSTERED 
    (
    UserSalaryID
    ) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);

GO

ALTER TABLE dbo.UserSalary ADD CONSTRAINT
    FK_UserSalary_Users FOREIGN KEY
    (
    UserID
    ) REFERENCES dbo.Users
    (
    UserID
    ) ON UPDATE  CASCADE
     ON DELETE  CASCADE;

GO

 -- Permission for editing Salary info
INSERT INTO [dbo].[SecurityRole]([SecurityRoleID], [Description], [SecurityRoleCategoryID])
    VALUES ('UserManager.Salary', 'Ability to manage user salaries.', 'Managers')
GO

--
-- Remove unused ProcessLevelPricing setting.
-- DWOS uses the 'PartPricingType' setting instead.
--
DELETE FROM ApplicationSettings
WHERE SettingName = 'ProcessLevelPricing';

GO

--
-- Material Cost
--

-- Unit
ALTER TABLE dbo.Process
ADD
    MaterialUnitCost decimal(8, 5) NULL,
    MaterialUnit nvarchar(3) NULL;

GO

-- Total
ALTER TABLE dbo.OrderProcesses
ADD MaterialCost decimal(11, 5) NULL;

GO

-- Hide some Input Types from being selected for inspection questions
ALTER TABLE dbo.d_InputType
ADD AllowInInspection bit NOT NULL DEFAULT(1);

GO

-- Add Pre-Process and Post-Process Weight Inputs
INSERT INTO dbo.d_InputType(InputType, AllowInInspection)
VALUES
(
    'PreProcessWeight', 0
),
(
    'PostProcessWeight', 0
);

GO

-- Unit Type for pounds
IF NOT EXISTS (
        SELECT 1
        FROM dbo.d_UnitType
        WHERE [UnitType] = 'lbs'
        )
    INSERT INTO dbo.d_UnitType (UnitType)
    VALUES ('lbs');

GO