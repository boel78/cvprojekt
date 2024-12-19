USE master
GO
DROP DATABASE IF EXISTS CV_DB;
GO
CREATE DATABASE CV_DB;
GO


USE CV_DB;
GO


CREATE TABLE [dbo].[Users] (
    [UserID]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    [Password]       NVARCHAR (100) NOT NULL,
    [IsPrivate]      BIT            DEFAULT ((0)) NOT NULL,
    [IsActive]       BIT            DEFAULT ((1)) NOT NULL,
    [CreatedDate]    DATETIME       DEFAULT (getdate()) NOT NULL,
    [ProfilePicture] NVARCHAR (100) DEFAULT ('ProfilePictureURL') NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);


CREATE TABLE [dbo].[Projects] (
    [ProjectID]   INT            IDENTITY (1, 1) NOT NULL,
    [Title]       NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [CreatedBy]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ProjectID] ASC),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([UserID])
);




CREATE TABLE [dbo].[UserProjects] (
    [UserID]    INT NOT NULL,
    [ProjectID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([UserID] ASC, [ProjectID] ASC),
    FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([UserID]),
    FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ProjectID])
);


CREATE TABLE [dbo].[CV] (
    [CVID]        INT            IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [Owner]       INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([CVID] ASC),
    FOREIGN KEY ([Owner]) REFERENCES [dbo].[Users] ([UserID])
);


CREATE TABLE [dbo].[CvProjects] (
    [CVID]      INT NOT NULL,
    [ProjectID] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([CVID] ASC, [ProjectID] ASC),
    FOREIGN KEY ([CVID]) REFERENCES [dbo].[CV] ([CVID]),
    FOREIGN KEY ([ProjectID]) REFERENCES [dbo].[Projects] ([ProjectID])
);


CREATE TABLE [dbo].[CvViews] (
    [CVID]      INT NOT NULL,
    [ViewCount] INT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([CVID] ASC),
    FOREIGN KEY ([CVID]) REFERENCES [dbo].[CV] ([CVID])
);

CREATE TABLE [dbo].[Skills] (
	[SID] INT IDENTITY (1, 1) NOT NULL,
	[Name] NVARCHAR(100) UNIQUE NOT NULL,
	PRIMARY KEY CLUSTERED ([SID] ASC)
)

CREATE TABLE [dbo].[Education](
	[EID] INT IDENTITY(1,1) NOT NULL,
	[Title] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(200) NOT NULL,
	[CVID] INT NOT NULL,
	PRIMARY KEY CLUSTERED ([EID] ASC),
	FOREIGN KEY ([CVID]) REFERENCES [dbo].[CV] ([CVID]),
)

CREATE TABLE [dbo].[EducationSkills] (
	[SID] INT NOT NULL,
	[EID] INT NOT NULL,
	PRIMARY KEY CLUSTERED ([EID] ASC, [SID] ASC),
	FOREIGN KEY ([EID]) REFERENCES [dbo].[Education] ([EID]),
	FOREIGN KEY ([SID]) REFERENCES [dbo].[Skills] ([SID])
)


CREATE TABLE [dbo].[Messages](
	[MID] INT IDENTITY (1,1) NOT NULL,
	[Sender] INT NOT NULL,
	[Reciever] INT NOT NULL,
	[Content] Nvarchar(300),
	[IsRead] bit,
	[TimeSent] date,
	PRIMARY KEY CLUSTERED ([MID] ASC),
	FOREIGN KEY ([Sender]) REFERENCES [dbo].[Users] ([UserID]),
	FOREIGN KEY ([Reciever]) REFERENCES [dbo].[Users] ([UserID])
)


INSERT INTO Users (Name, Email, Password)
	VALUES('Jonas Moll', 'Jonas.moll@oru.se', 'password');


