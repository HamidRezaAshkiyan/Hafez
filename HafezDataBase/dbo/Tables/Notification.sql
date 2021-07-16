CREATE TABLE [dbo].[Notification] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [GroupId]          INT            NULL,
    [SortId]           INT            DEFAULT ((0)) NULL,
    [Name]             NVARCHAR (250) NULL,
    [Description]      NVARCHAR (MAX) NOT NULL,
    [NotificationType] NVARCHAR (50)  NOT NULL,
    [CreatedAt]        DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [CreatedBy]        INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([Id]),
    FOREIGN KEY ([GroupId]) REFERENCES [dbo].[NotificationGroup] ([Id])
);









