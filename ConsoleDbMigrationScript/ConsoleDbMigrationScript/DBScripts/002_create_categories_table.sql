-------------------- SCRIPT TO CHECK OF DbScriptMigrationSystem -------------------------------
DECLARE @MigrationName AS VARCHAR(1000) = '002_create_categories_table'

IF EXISTS(SELECT MigrationId FROM [DbScriptMigration] WHERE MigrationName = @MigrationName)
BEGIN 
    raiserror('MIGRATION ALREADY RUNNED ON THIS DB!!! STOP EXECUTION SCRIPT', 11, 0)
    SET NOEXEC ON
END

INSERT INTO [DbScriptMigration]
    (MigrationId, MigrationName, MigrationDate)
    VALUES
    (NEWID(), @MigrationName, GETDATE())
GO
PRINT 'Insert record into [DbScriptMigration]!'
-------------------- END SCRIPT TO CHECK OF DbScriptMigrationSystem ---------------------------

-------------------- SCRIPT TO CHECK PREREQUISITES OF DbScriptMigrationSystem -------------------------------
DECLARE @PrerequisiteMigrationName AS VARCHAR(1000) = '001_create_posts_table'
IF NOT EXISTS(SELECT MigrationId FROM [DbScriptMigration] WHERE MigrationName = @PrerequisiteMigrationName)
BEGIN 
    raiserror('YOU HAVET TO RUN SCRIPT %s ON THIS DB!!! STOP EXECUTION SCRIPT ', 11, 0, @PrerequisiteMigrationName)
    SET NOEXEC ON
END
-------------------- END SCRIPT TO CHECK PREREQUISITES OF DbScriptMigrationSystem ---------------------------

-- Create table Categories
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


-- INSERT some example data
INSERT INTO [dbo].[Categories]
           ([Name]
           ,[Description]
           ,[CreateDate])
     VALUES
           ('Category 1'
           ,'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
           ,GETDATE())
GO

INSERT INTO [dbo].[Categories]
           ([Name]
           ,[Description]
           ,[CreateDate])
     VALUES
           ('Category 2'
           ,'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
           ,GETDATE())
GO

-- insert column Category Id to table Posts
ALTER TABLE [dbo].[Posts]
    ADD CategoryId INTEGER,
    FOREIGN KEY(CategoryId) REFERENCES [dbo].[Categories](Id)
GO

-- Update some post set CategoryId = 1
UPDATE [dbo].[Posts] SET CategoryId = 1 WHERE Id IN (
select id from [dbo].[Posts] where ( Id % 2 ) = 0)


-- Update some post set CategoryId = 2
UPDATE [dbo].[Posts] SET CategoryId = 2 WHERE Id IN (
select id from[dbo].[Posts]  where ( Id % 2 ) = 1 )

---------------- FOOTER OF DbScriptMigrationSystem : REMEMBER TO INSERT -----------------------
SET NOEXEC OFF