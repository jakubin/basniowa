:on error exit
SET XACT_ABORT ON;

IF NOT EXISTS (SELECT * FROM [Basniowa].[dbo].[_SchemaVersion] WHERE [SchemaVersion] = 0)
BEGIN
	SET NOEXEC ON
	RETURN
END

BEGIN TRANSACTION

UPDATE [dbo].[_SchemaVersion] SET [SchemaVersion] = 1
GO

-- =============================================
-- SCHEMA [shows]
-- Schema for module maintaining shows information.
-- =============================================
CREATE SCHEMA [shows];
GO

-- =============================================
-- SCHEMA [repertoire]
-- Schema for module maintaining repertoire.
-- =============================================
CREATE SCHEMA [repertoire];
GO

-- =============================================
-- TABLE [dbo].[UniqueId]
-- Used for generating unique identifiers
-- =============================================
CREATE TABLE [dbo].[UniqueId]
(
	[NextId] bigint NOT NULL
)
GO

INSERT INTO [dbo].[UniqueId] ([NextId]) VALUES (1)
GO

-- =============================================
-- PROCEDURE [dbo].[GetNextIds]
-- Generates a specified number of unique identifiers
-- =============================================
CREATE PROCEDURE [dbo].[GetNextIds]
	@Count BIGINT,
	@RangeFrom BIGINT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

    DECLARE @Range TABLE
	(
		[RangeFrom] BIGINT
	)

	UPDATE [dbo].[UniqueId] SET [NextId] = [NextId] + @Count
	OUTPUT DELETED.[NextId] AS [RangeFrom] INTO @Range

	SELECT @RangeFrom=[RangeFrom] FROM @Range
END
GO

-- =============================================
-- TABLE [dbo].[Shows]
-- Stores information about shows
-- =============================================
CREATE TABLE [shows].[Shows] (
	[Id] bigint NOT NULL,
	[Title] nvarchar(200) NOT NULL,
	[Description] nvarchar(max) NOT NULL,
	[Subtitle] nvarchar(500) NULL,
	[CreatedUtc] datetimeoffset NOT NULL,
	[CreatedBy] nvarchar(50) NOT NULL,
	[ModifiedUtc] datetimeoffset NOT NULL,
	[ModifiedBy] nvarchar(50) NOT NULL,
	[IsDeleted] bit NOT NULL,
	CONSTRAINT [PK_Shows] PRIMARY KEY CLUSTERED ([Id] ASC) 
)
GO

-- =============================================
-- TABLE [shows].[ShowProperties]
-- Stores information about additional properties of shows
-- =============================================
CREATE TABLE [shows].[ShowProperties](
	[Id] bigint NOT NULL,
	[ShowId] bigint NOT NULL,
	[Name] nvarchar(200) NOT NULL,
	[Value] nvarchar(500) NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_ShowProperties] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ShowProperties_Shows] FOREIGN KEY([ShowId]) REFERENCES [shows].[Shows] ([Id]),
	CONSTRAINT [IX_ShowProperties_Name] UNIQUE NONCLUSTERED (ShowId, Name)
)
GO

CREATE TABLE [shows].[ShowPictures](
	[Id] bigint NOT NULL,
	[ShowId] bigint NOT NULL,
	[Title] nvarchar(200) NULL,
	[ImagePath] nvarchar(250) NOT NULL,
	[ThumbPath] nvarchar(250) NOT NULL,
	[CreatedUtc] datetimeoffset NOT NULL,
	[CreatedBy] nvarchar(50) NOT NULL,
	[ModifiedUtc] datetimeoffset NOT NULL,
	[ModifiedBy] nvarchar(50) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	CONSTRAINT [PK_ShowPictures] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_ShowPictures_Shows] FOREIGN KEY([ShowId]) REFERENCES [shows].[Shows] ([Id]),
)

ALTER TABLE [shows].[Shows] ADD 
	[MainShowPictureId] bigint NULL,
	CONSTRAINT [FK_Shows_ShowPictures] FOREIGN KEY([MainShowPictureId]) REFERENCES [shows].[ShowPictures] ([Id])


COMMIT

SET NOEXEC OFF