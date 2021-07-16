CREATE TABLE [dbo].[PersonalDua] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [DuaId]       INT            NOT NULL,
    [FarazId]     INT            NOT NULL,
    [ArabicText]  NVARCHAR (MAX) NOT NULL,
    [PersianText] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PersonalDua_ToPersonalDuaList] FOREIGN KEY ([DuaId]) REFERENCES [dbo].[PersonalDuaList] ([DuaId])
);

