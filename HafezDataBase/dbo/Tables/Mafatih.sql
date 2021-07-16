CREATE TABLE [dbo].[Mafatih] (
    [Id]          INT            NOT NULL,
    [DuaId]       INT            NOT NULL,
    [FarazId]     INT            NOT NULL,
    [ArabicText]  NVARCHAR (MAX) NOT NULL,
    [PersianText] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Mafatih_ToMafatihList] FOREIGN KEY ([DuaId]) REFERENCES [dbo].[MafatihList] ([DuaId])
);



