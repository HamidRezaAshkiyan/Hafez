CREATE TABLE [dbo].[Users] (
    [Id]       INT           IDENTITY (1, 1) NOT NULL,
    [UserId]   NVARCHAR (20) NOT NULL,
    [Password] NVARCHAR (20) NOT NULL,
    [Name]     NVARCHAR (50) NOT NULL,
    [Statue]   CHAR (1)      NOT NULL,
    [UserType] CHAR (1)      DEFAULT ('U') NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([UserId] ASC)
);



